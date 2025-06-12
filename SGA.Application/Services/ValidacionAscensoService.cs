using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SGA.Application.DTOs;
using SGA.Domain.Constants;
using SGA.Domain.Entities;
using SGA.Infrastructure.Repositories;

namespace SGA.Application.Services
{
    public class ValidacionAscensoService : IValidacionAscensoService
    {
        private readonly IDocenteRepository _docenteRepository;

        public ValidacionAscensoService(IDocenteRepository docenteRepository)
        {
            _docenteRepository = docenteRepository;
        }

        public async Task<bool> ValidarRequisitosAscensoAsync(int docenteId)
        {
            var docente = await _docenteRepository.GetByIdAsync(docenteId);
            if (docente == null || docente.Indicadores == null)
            {
                return false;
            }

            return ValidarIndicadoresParaAscenso(
                docente.NivelActual,
                docente.Indicadores.TiempoEnRolActual,
                docente.Indicadores.NumeroObras,
                docente.Indicadores.PuntajeEvaluacion,
                docente.Indicadores.HorasCapacitacion,
                docente.Indicadores.TiempoInvestigacion
            );
        }

        public async Task<Dictionary<string, bool>> ObtenerEstadoRequisitosAsync(int docenteId, int? nivelActualParam = null)
        {
            var docente = await _docenteRepository.GetByIdAsync(docenteId);
            if (docente == null || docente.Indicadores == null)
            {
                return new Dictionary<string, bool>();
            }

            int nivelActual = nivelActualParam ?? docente.NivelActual;
            var indicadores = docente.Indicadores;

            var resultado = new Dictionary<string, bool>
            {
                ["TiempoEnRol"] = CumpleTiempoEnRol(nivelActual, indicadores.TiempoEnRolActual),
                ["NumeroObras"] = CumpleNumeroObras(nivelActual, indicadores.NumeroObras),
                ["PuntajeEvaluacion"] = CumplePuntajeEvaluacion(indicadores.PuntajeEvaluacion),
                ["HorasCapacitacion"] = CumpleHorasCapacitacion(nivelActual, indicadores.HorasCapacitacion),
                ["TiempoInvestigacion"] = CumpleTiempoInvestigacion(nivelActual, indicadores.TiempoInvestigacion)
            };

            return resultado;
        }

        public bool ValidarIndicadoresParaAscenso(
            int nivelActual, int tiempoRol, int numeroObras, 
            decimal puntajeEvaluacion, int horasCapacitacion, int tiempoInvestigacion)
        {
            // Si el nivel actual es 5 (máximo), no se puede ascender más
            if (nivelActual >= 5)
            {
                return false;
            }

            // Validación de tiempo en rol (4 años para todos los niveles)
            if (!CumpleTiempoEnRol(nivelActual, tiempoRol))
            {
                return false;
            }

            // Validación de puntaje de evaluación (75% para todos los niveles)
            if (!CumplePuntajeEvaluacion(puntajeEvaluacion))
            {
                return false;
            }

            // Validaciones específicas por nivel
            return nivelActual switch
            {
                1 => ValidarAscensoNivel1a2(numeroObras, horasCapacitacion),
                2 => ValidarAscensoNivel2a3(numeroObras, horasCapacitacion, tiempoInvestigacion),
                3 => ValidarAscensoNivel3a4(numeroObras, horasCapacitacion, tiempoInvestigacion),
                4 => ValidarAscensoNivel4a5(numeroObras, horasCapacitacion, tiempoInvestigacion),
                _ => false
            };
        }

        #region Validaciones específicas por nivel
        
        private bool ValidarAscensoNivel1a2(int numeroObras, int horasCapacitacion)
        {
            return numeroObras >= 1 && horasCapacitacion >= 96;
            // No se requiere tiempo de investigación para este nivel
        }

        private bool ValidarAscensoNivel2a3(int numeroObras, int horasCapacitacion, int tiempoInvestigacion)
        {
            return numeroObras >= 2 && 
                   horasCapacitacion >= 96 && 
                   tiempoInvestigacion >= 12;
        }

        private bool ValidarAscensoNivel3a4(int numeroObras, int horasCapacitacion, int tiempoInvestigacion)
        {
            return numeroObras >= 3 && 
                   horasCapacitacion >= 128 && 
                   tiempoInvestigacion >= 24;
        }

        private bool ValidarAscensoNivel4a5(int numeroObras, int horasCapacitacion, int tiempoInvestigacion)
        {
            return numeroObras >= 5 && 
                   horasCapacitacion >= 160 && 
                   tiempoInvestigacion >= 24;
        }
        
        #endregion

        #region Validaciones de requisitos individuales
        
        private bool CumpleTiempoEnRol(int nivelActual, int tiempoRol)
        {
            // Para todos los niveles se requieren 4 años (48 meses)
            return tiempoRol >= 48;
        }

        private bool CumpleNumeroObras(int nivelActual, int numeroObras)
        {
            return nivelActual switch
            {
                1 => numeroObras >= 1, // De nivel 1 a 2
                2 => numeroObras >= 2, // De nivel 2 a 3
                3 => numeroObras >= 3, // De nivel 3 a 4
                4 => numeroObras >= 5, // De nivel 4 a 5
                _ => false
            };
        }

        private bool CumplePuntajeEvaluacion(decimal puntajeEvaluacion)
        {
            // Para todos los niveles se requiere 75%
            return puntajeEvaluacion >= 75;
        }

        private bool CumpleHorasCapacitacion(int nivelActual, int horasCapacitacion)
        {
            return nivelActual switch
            {
                1 => horasCapacitacion >= 96,  // De nivel 1 a 2
                2 => horasCapacitacion >= 96,  // De nivel 2 a 3
                3 => horasCapacitacion >= 128, // De nivel 3 a 4
                4 => horasCapacitacion >= 160, // De nivel 4 a 5
                _ => false
            };
        }

        private bool CumpleTiempoInvestigacion(int nivelActual, int tiempoInvestigacion)
        {
            return nivelActual switch
            {
                1 => true, // No se requiere para ascenso de nivel 1 a 2
                2 => tiempoInvestigacion >= 12, // De nivel 2 a 3 (12 meses)
                3 => tiempoInvestigacion >= 24, // De nivel 3 a 4 (24 meses)
                4 => tiempoInvestigacion >= 24, // De nivel 4 a 5 (24 meses)
                _ => false
            };
        }
        
        #endregion

        public async Task<RequisitosAscensoResult> ValidarRequisitosAscensoAsync(int docenteId, int nivelSolicitado)
        {
            var docente = await _docenteRepository.GetByIdAsync(docenteId);
            
            if (docente == null)
                throw new InvalidOperationException($"No se encontró el docente con ID {docenteId}");
                
            if (nivelSolicitado != docente.NivelActual + 1)
                throw new InvalidOperationException("Solo se puede solicitar ascenso al nivel inmediato superior");
                
            if (docente.NivelActual >= ReglasAscenso.NivelMaximo)
                throw new InvalidOperationException("Ya ha alcanzado el nivel máximo de la carrera docente");
                
            var indicadores = docente.Indicadores;
            if (indicadores == null)
                throw new InvalidOperationException("El docente no tiene indicadores registrados");
            
            // Obtener requisitos para el nivel actual
            var requisito = ReglasAscenso.ObtenerRequisitoParaNivel(docente.NivelActual);
            if (requisito == null)
                throw new InvalidOperationException($"No se encontraron requisitos para el nivel {docente.NivelActual}");
            
            bool cumpleTiempo = indicadores.TiempoEnRolActual >= requisito.TiempoMinimo;
            bool cumpleObras = indicadores.NumeroObras >= requisito.ObrasMinimas;
            bool cumpleEvaluacion = indicadores.PuntajeEvaluacion >= requisito.PuntajeEvaluacionMinimo;
            bool cumpleCapacitacion = indicadores.HorasCapacitacion >= requisito.HorasCapacitacionMinimas;
            bool cumpleInvestigacion = indicadores.TiempoInvestigacion >= requisito.TiempoInvestigacionMinimo;
            
            var result = new RequisitosAscensoResult
            {
                CumpleTiempo = cumpleTiempo,
                CumpleObras = cumpleObras,
                CumpleEvaluacion = cumpleEvaluacion,
                CumpleCapacitacion = cumpleCapacitacion,
                CumpleInvestigacion = cumpleInvestigacion,
                CumpleTodosRequisitos = cumpleTiempo && cumpleObras && cumpleEvaluacion && 
                                      cumpleCapacitacion && cumpleInvestigacion
            };
            
            return result;
        }
    }
}
