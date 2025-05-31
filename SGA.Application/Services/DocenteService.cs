using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SGA.Domain.Constants;
using SGA.Domain.Entities;
using SGA.Infrastructure.Repositories;

namespace SGA.Application.Services
{
    public interface IDocenteService
    {
        Task<Docente> GetDocenteByIdAsync(int id);
        Task<Docente> GetDocenteByCedulaAsync(string cedula);
        Task<bool> ValidarRequisitosAscensoAsync(int docenteId);
        Task<Dictionary<string, bool>> ObtenerEstadoRequisitosAsync(int docenteId);
        Task ActualizarIndicadoresAsync(int docenteId, int tiempoRol, int numeroObras, decimal puntajeEvaluacion, int horasCapacitacion, int tiempoInvestigacion);
    }

    public class DocenteService : IDocenteService
    {
        private readonly IDocenteRepository _docenteRepository;

        public DocenteService(IDocenteRepository docenteRepository)
        {
            _docenteRepository = docenteRepository;
        }

        public async Task<Docente> GetDocenteByIdAsync(int id)
        {
            return await _docenteRepository.GetByIdAsync(id);
        }

        public async Task<Docente> GetDocenteByCedulaAsync(string cedula)
        {
            return await _docenteRepository.GetByCedulaAsync(cedula);
        }

        public async Task<bool> ValidarRequisitosAscensoAsync(int docenteId)
        {
            var docente = await _docenteRepository.GetByIdAsync(docenteId);
            if (docente == null)
                return false;

            if (docente.NivelActual >= 5) // Ya está en nivel máximo
                return false;

            var requisito = ObtenerRequisitoParaNivel(docente.NivelActual);
            if (requisito == null)
                return false;

            return docente.TiempoEnRolActual >= requisito.TiempoMinimo &&
                   docente.NumeroObras >= requisito.ObrasMinimas &&
                   docente.PuntajeEvaluacion >= requisito.PuntajeEvaluacionMinimo &&
                   docente.HorasCapacitacion >= requisito.HorasCapacitacionMinimas &&
                   docente.TiempoInvestigacion >= requisito.TiempoInvestigacionMinimo;
        }

        public async Task<Dictionary<string, bool>> ObtenerEstadoRequisitosAsync(int docenteId)
        {
            var docente = await _docenteRepository.GetByIdAsync(docenteId);
            if (docente == null)
                return new Dictionary<string, bool>();

            var requisito = ObtenerRequisitoParaNivel(docente.NivelActual);
            if (requisito == null)
                return new Dictionary<string, bool>();

            return new Dictionary<string, bool>
            {
                { "TiempoEnRol", docente.TiempoEnRolActual >= requisito.TiempoMinimo },
                { "NumeroObras", docente.NumeroObras >= requisito.ObrasMinimas },
                { "PuntajeEvaluacion", docente.PuntajeEvaluacion >= requisito.PuntajeEvaluacionMinimo },
                { "HorasCapacitacion", docente.HorasCapacitacion >= requisito.HorasCapacitacionMinimas },
                { "TiempoInvestigacion", docente.TiempoInvestigacion >= requisito.TiempoInvestigacionMinimo }
            };
        }

        public async Task ActualizarIndicadoresAsync(int docenteId, int tiempoRol, int numeroObras, decimal puntajeEvaluacion, int horasCapacitacion, int tiempoInvestigacion)
        {
            var docente = await _docenteRepository.GetByIdAsync(docenteId);
            if (docente == null)
                throw new Exception("Docente no encontrado");

            docente.TiempoEnRolActual = tiempoRol;
            docente.NumeroObras = numeroObras;
            docente.PuntajeEvaluacion = puntajeEvaluacion;
            docente.HorasCapacitacion = horasCapacitacion;
            docente.TiempoInvestigacion = tiempoInvestigacion;

            await _docenteRepository.UpdateAsync(docente);
        }

        private RequisitoAscenso ObtenerRequisitoParaNivel(int nivelActual)
        {
            return Array.Find(ReglasAscenso.Requisitos, r => r.NivelActual == nivelActual);
        }
    }
}
