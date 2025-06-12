using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;
using SGA.Domain.Constants;
using SGA.Domain.Entities;
using SGA.Infrastructure.Repositories;

namespace SGA.Application.Services
{    public interface ISolicitudAscensoService
    {
        Task<SolicitudAscenso> GetSolicitudByIdAsync(int id);
        Task<IEnumerable<SolicitudAscenso>> GetSolicitudesByDocenteIdAsync(int docenteId);
        Task<IEnumerable<SolicitudAscenso>> GetSolicitudesPendientesAsync();
        Task<SolicitudAscenso> CrearSolicitudAscensoAsync(int docenteId, List<int> documentosIds);
        Task<SolicitudAscenso> ProcesarSolicitudAsync(int solicitudId, int estadoSolicitudId, string? motivoRechazo = null, int revisorId = 0);
        Task<SolicitudAscenso> RevisarSolicitudAsync(int solicitudId, int revisorId, int estadoSolicitudId, string? motivoRechazo, string? observaciones);
    }

    public class SolicitudAscensoService : ISolicitudAscensoService
    {
        private readonly ISolicitudAscensoRepository _solicitudRepository;
        private readonly IDocenteRepository _docenteRepository;
        private readonly IDocumentoRepository _documentoRepository;

        public SolicitudAscensoService(
            ISolicitudAscensoRepository solicitudRepository,
            IDocenteRepository docenteRepository,
            IDocumentoRepository documentoRepository)
        {
            _solicitudRepository = solicitudRepository;
            _docenteRepository = docenteRepository;
            _documentoRepository = documentoRepository;
        }

        public async Task<SolicitudAscenso> GetSolicitudByIdAsync(int id)
        {
            return await _solicitudRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<SolicitudAscenso>> GetSolicitudesByDocenteIdAsync(int docenteId)
        {
            return await _solicitudRepository.GetByDocenteIdAsync(docenteId);
        }

        public async Task<IEnumerable<SolicitudAscenso>> GetSolicitudesPendientesAsync()
        {
            return await _solicitudRepository.GetAllPendientesAsync();
        }        public async Task<SolicitudAscenso> CrearSolicitudAscensoAsync(int docenteId, List<int> documentosIds)
        {
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            
            var docente = await _docenteRepository.GetByIdAsync(docenteId);
            if (docente == null)
                throw new Exception("Docente no encontrado");

            // Verificar si el docente tiene indicadores
            if (docente.Indicadores == null)
                throw new Exception("El docente no tiene indicadores registrados");

            // Verificar si el docente tiene solicitudes pendientes
            var solicitudesPendientes = await _solicitudRepository.GetByDocenteIdAsync(docenteId);
            if (solicitudesPendientes.Any(s => s.EstadoSolicitudId == 1 || s.EstadoSolicitudId == 2)) // 1=Enviada, 2=EnProceso
                throw new Exception("Ya tiene una solicitud de ascenso pendiente");

            // Validar requisitos usando los indicadores
            var requisito = Array.Find(ReglasAscenso.Requisitos, r => r.NivelActual == docente.NivelActual);
            if (requisito == null)
                throw new Exception("No se encontraron requisitos para el nivel actual");

            var indicadores = docente.Indicadores;
            if (indicadores.TiempoEnRolActual < requisito.TiempoMinimo)
                throw new Exception($"No cumple con el tiempo mínimo requerido ({requisito.TiempoMinimo} años)");
                
            if (indicadores.NumeroObras < requisito.ObrasMinimas)
                throw new Exception($"No cumple con el número mínimo de obras ({requisito.ObrasMinimas})");
                
            if (indicadores.PuntajeEvaluacion < requisito.PuntajeEvaluacionMinimo)
                throw new Exception($"No cumple con el puntaje mínimo de evaluación ({requisito.PuntajeEvaluacionMinimo}%)");
                
            if (indicadores.HorasCapacitacion < requisito.HorasCapacitacionMinimas)
                throw new Exception($"No cumple con las horas mínimas de capacitación ({requisito.HorasCapacitacionMinimas} horas)");
                
            if (indicadores.TiempoInvestigacion < requisito.TiempoInvestigacionMinimo)
                throw new Exception($"No cumple con el tiempo mínimo de investigación ({requisito.TiempoInvestigacionMinimo} meses)");

            // Crear la solicitud
            var solicitud = new SolicitudAscenso
            {
                DocenteId = docenteId,
                Docente = docente,
                EstadoSolicitudId = 1, // 1 = Enviada
                EstadoSolicitud = null!, // Se asignará por Entity Framework
                FechaSolicitud = DateTime.Now,
                NivelActual = docente.NivelActual,
                NivelSolicitado = docente.NivelActual + 1,
                TiempoEnRol = indicadores.TiempoEnRolActual,
                NumeroObras = indicadores.NumeroObras,
                PuntajeEvaluacion = indicadores.PuntajeEvaluacion,
                HorasCapacitacion = indicadores.HorasCapacitacion,
                TiempoInvestigacion = indicadores.TiempoInvestigacion,
                Documentos = new List<DocumentoSolicitud>()
            };

            // Adjuntar los documentos
            foreach (var documentoId in documentosIds)
            {
                var documento = await _documentoRepository.GetByIdAsync(documentoId);
                if (documento == null || documento.DocenteId != docenteId)
                    throw new Exception($"Documento no encontrado o no pertenece al docente: {documentoId}");
                
                solicitud.Documentos.Add(new DocumentoSolicitud
                {
                    DocumentoId = documentoId,
                    Documento = null!, // Se asignará por Entity Framework
                    Solicitud = solicitud
                });
            }

            await _solicitudRepository.AddAsync(solicitud);
            scope.Complete();
            
            return solicitud;
        }        public async Task<SolicitudAscenso> ProcesarSolicitudAsync(int solicitudId, int estadoSolicitudId, string? motivoRechazo = null, int revisorId = 0)
        {
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            
            var solicitud = await _solicitudRepository.GetByIdAsync(solicitudId);
            if (solicitud == null)
                throw new Exception("Solicitud no encontrada");

            if (solicitud.EstadoSolicitudId == 3 || solicitud.EstadoSolicitudId == 4) // 3=Aprobada, 4=Rechazada
                throw new Exception("Esta solicitud ya fue procesada");

            solicitud.EstadoSolicitudId = estadoSolicitudId;
            solicitud.FechaRevision = DateTime.Now;
            solicitud.RevisorId = revisorId;

            if (estadoSolicitudId == 4) // Rechazada
            {
                if (string.IsNullOrWhiteSpace(motivoRechazo))
                    throw new Exception("Debe especificar el motivo del rechazo");
                    
                solicitud.MotivoRechazo = motivoRechazo;
            }
            
            if (estadoSolicitudId == 3) // Aprobada
            {
                // Actualizar nivel del docente
                var docente = await _docenteRepository.GetByIdAsync(solicitud.DocenteId);
                if (docente != null)
                {
                    docente.NivelActual = solicitud.NivelSolicitado;
                    docente.FechaIngresoNivelActual = DateTime.Now;
                    
                    // Reiniciar contadores en los indicadores
                    if (docente.Indicadores != null)
                    {
                        docente.Indicadores.NumeroObras = 0;
                        docente.Indicadores.HorasCapacitacion = 0;
                        docente.Indicadores.TiempoInvestigacion = 0;
                        docente.Indicadores.FechaActualizacion = DateTime.Now;
                    }
                    
                    await _docenteRepository.UpdateAsync(docente);
                }
            }

            await _solicitudRepository.UpdateAsync(solicitud);
            scope.Complete();
              return solicitud;
        }

        public async Task<SolicitudAscenso> RevisarSolicitudAsync(int solicitudId, int revisorId, int estadoSolicitudId, string? motivoRechazo, string? observaciones)
        {
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            
            var solicitud = await _solicitudRepository.GetByIdAsync(solicitudId);
            if (solicitud == null)
                throw new Exception("Solicitud no encontrada");

            if (solicitud.EstadoSolicitudId == 3 || solicitud.EstadoSolicitudId == 4) // 3=Aprobada, 4=Rechazada
                throw new Exception("Esta solicitud ya fue procesada");

            solicitud.EstadoSolicitudId = estadoSolicitudId;
            solicitud.FechaRevision = DateTime.Now;
            solicitud.RevisorId = revisorId;
            solicitud.ObservacionesRevisor = observaciones;

            if (estadoSolicitudId == 4) // Rechazada
            {
                if (string.IsNullOrWhiteSpace(motivoRechazo))
                    throw new Exception("Debe especificar el motivo del rechazo");
                    
                solicitud.MotivoRechazo = motivoRechazo;
            }
            
            if (estadoSolicitudId == 3) // Aprobada
            {
                // Actualizar nivel del docente
                var docente = await _docenteRepository.GetByIdAsync(solicitud.DocenteId);
                if (docente != null)
                {
                    docente.NivelActual = solicitud.NivelSolicitado;
                    docente.FechaIngresoNivelActual = DateTime.Now;
                    
                    // Reiniciar contadores en los indicadores
                    if (docente.Indicadores != null)
                    {
                        docente.Indicadores.NumeroObras = 0;
                        docente.Indicadores.HorasCapacitacion = 0;
                        docente.Indicadores.TiempoInvestigacion = 0;
                        docente.Indicadores.FechaActualizacion = DateTime.Now;
                    }
                    
                    await _docenteRepository.UpdateAsync(docente);
                }
            }

            await _solicitudRepository.UpdateAsync(solicitud);
            scope.Complete();
            
            return solicitud;
        }
    }
}
