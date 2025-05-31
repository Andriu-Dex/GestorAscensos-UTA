using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;
using SGA.Domain.Constants;
using SGA.Domain.Entities;
using SGA.Infrastructure.Repositories;

namespace SGA.Application.Services
{
    public interface ISolicitudAscensoService
    {
        Task<SolicitudAscenso> GetSolicitudByIdAsync(int id);
        Task<IEnumerable<SolicitudAscenso>> GetSolicitudesByDocenteIdAsync(int docenteId);
        Task<IEnumerable<SolicitudAscenso>> GetSolicitudesPendientesAsync();
        Task<SolicitudAscenso> CrearSolicitudAscensoAsync(int docenteId, List<int> documentosIds);
        Task<SolicitudAscenso> ProcesarSolicitudAsync(int solicitudId, EstadoSolicitud nuevoEstado, string motivoRechazo = null, int revisorId = 0);
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
        }

        public async Task<SolicitudAscenso> CrearSolicitudAscensoAsync(int docenteId, List<int> documentosIds)
        {
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            
            var docente = await _docenteRepository.GetByIdAsync(docenteId);
            if (docente == null)
                throw new Exception("Docente no encontrado");

            // Verificar si el docente tiene solicitudes pendientes
            var solicitudesPendientes = await _solicitudRepository.GetByDocenteIdAsync(docenteId);
            if (solicitudesPendientes.Any(s => s.Estado == EstadoSolicitud.Enviada || s.Estado == EstadoSolicitud.EnProceso))
                throw new Exception("Ya tiene una solicitud de ascenso pendiente");

            // Validar requisitos
            var requisito = Array.Find(ReglasAscenso.Requisitos, r => r.NivelActual == docente.NivelActual);
            if (requisito == null)
                throw new Exception("No se encontraron requisitos para el nivel actual");

            if (docente.TiempoEnRolActual < requisito.TiempoMinimo)
                throw new Exception($"No cumple con el tiempo mínimo requerido ({requisito.TiempoMinimo} años)");
                
            if (docente.NumeroObras < requisito.ObrasMinimas)
                throw new Exception($"No cumple con el número mínimo de obras ({requisito.ObrasMinimas})");
                
            if (docente.PuntajeEvaluacion < requisito.PuntajeEvaluacionMinimo)
                throw new Exception($"No cumple con el puntaje mínimo de evaluación ({requisito.PuntajeEvaluacionMinimo}%)");
                
            if (docente.HorasCapacitacion < requisito.HorasCapacitacionMinimas)
                throw new Exception($"No cumple con las horas mínimas de capacitación ({requisito.HorasCapacitacionMinimas} horas)");
                
            if (docente.TiempoInvestigacion < requisito.TiempoInvestigacionMinimo)
                throw new Exception($"No cumple con el tiempo mínimo de investigación ({requisito.TiempoInvestigacionMinimo} meses)");

            // Crear la solicitud
            var solicitud = new SolicitudAscenso
            {
                DocenteId = docenteId,
                FechaSolicitud = DateTime.Now,
                NivelActual = docente.NivelActual,
                NivelSolicitado = docente.NivelActual + 1,
                Estado = EstadoSolicitud.Enviada,
                TiempoEnRol = docente.TiempoEnRolActual,
                NumeroObras = docente.NumeroObras,
                PuntajeEvaluacion = docente.PuntajeEvaluacion,
                HorasCapacitacion = docente.HorasCapacitacion,
                TiempoInvestigacion = docente.TiempoInvestigacion,
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
                    Solicitud = solicitud
                });
            }

            await _solicitudRepository.AddAsync(solicitud);
            scope.Complete();
            
            return solicitud;
        }

        public async Task<SolicitudAscenso> ProcesarSolicitudAsync(int solicitudId, EstadoSolicitud nuevoEstado, string motivoRechazo = null, int revisorId = 0)
        {
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            
            var solicitud = await _solicitudRepository.GetByIdAsync(solicitudId);
            if (solicitud == null)
                throw new Exception("Solicitud no encontrada");

            if (solicitud.Estado == EstadoSolicitud.Aprobada || solicitud.Estado == EstadoSolicitud.Rechazada)
                throw new Exception("Esta solicitud ya fue procesada");

            solicitud.Estado = nuevoEstado;
            solicitud.FechaRevision = DateTime.Now;
            solicitud.RevisorId = revisorId;

            if (nuevoEstado == EstadoSolicitud.Rechazada)
            {
                if (string.IsNullOrWhiteSpace(motivoRechazo))
                    throw new Exception("Debe especificar el motivo del rechazo");
                    
                solicitud.MotivoRechazo = motivoRechazo;
            }
            
            if (nuevoEstado == EstadoSolicitud.Aprobada)
            {
                // Actualizar nivel del docente
                var docente = await _docenteRepository.GetByIdAsync(solicitud.DocenteId);
                if (docente != null)
                {
                    docente.NivelActual = solicitud.NivelSolicitado;
                    docente.FechaIngresoNivelActual = DateTime.Now;
                    
                    // Reiniciar contadores
                    docente.NumeroObras = 0;
                    docente.HorasCapacitacion = 0;
                    docente.TiempoInvestigacion = 0;
                    
                    await _docenteRepository.UpdateAsync(docente);
                }
            }

            await _solicitudRepository.UpdateAsync(solicitud);
            scope.Complete();
            
            return solicitud;
        }
    }
}
