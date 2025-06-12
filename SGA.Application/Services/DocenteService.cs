using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SGA.Domain.Constants;
using SGA.Domain.Entities;
using SGA.Infrastructure.Repositories;

namespace SGA.Application.Services
{
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

        public async Task<Docente> GetDocenteByUsernameAsync(string username)
        {
            return await _docenteRepository.GetByUsernameAsync(username);
        }

        public async Task<Docente> GetDocenteByEmailAsync(string email)
        {
            return await _docenteRepository.GetByEmailAsync(email);
        }        public async Task<bool> ValidarRequisitosAscensoAsync(int docenteId)
        {
            var docente = await _docenteRepository.GetByIdAsync(docenteId);
            if (docente == null)
                return false;

            if (docente.NivelActual >= 5)
                return false;

            var requisito = ObtenerRequisitoParaNivel(docente.NivelActual);
            if (requisito == null)
                return false;

            // Los indicadores ahora están en la entidad IndicadorDocente relacionada
            var indicador = docente.Indicadores;
            if (indicador == null)
                return false;

            return indicador.TiempoEnRolActual >= requisito.TiempoMinimo &&
                   indicador.NumeroObras >= requisito.ObrasMinimas &&
                   indicador.PuntajeEvaluacion >= requisito.PuntajeEvaluacionMinimo &&
                   indicador.HorasCapacitacion >= requisito.HorasCapacitacionMinimas &&
                   indicador.TiempoInvestigacion >= requisito.TiempoInvestigacionMinimo;
        }        public async Task<Dictionary<string, bool>> ObtenerEstadoRequisitosAsync(int docenteId)
        {
            var docente = await _docenteRepository.GetByIdAsync(docenteId);
            if (docente == null)
                return new Dictionary<string, bool>();

            var requisito = ObtenerRequisitoParaNivel(docente.NivelActual);
            if (requisito == null)
                return new Dictionary<string, bool>();

            // Los indicadores ahora están en la entidad IndicadorDocente relacionada
            var indicador = docente.Indicadores;
            if (indicador == null)
                return new Dictionary<string, bool>();

            return new Dictionary<string, bool>
            {
                { "TiempoEnRol", indicador.TiempoEnRolActual >= requisito.TiempoMinimo },
                { "NumeroObras", indicador.NumeroObras >= requisito.ObrasMinimas },
                { "PuntajeEvaluacion", indicador.PuntajeEvaluacion >= requisito.PuntajeEvaluacionMinimo },
                { "HorasCapacitacion", indicador.HorasCapacitacion >= requisito.HorasCapacitacionMinimas },
                { "TiempoInvestigacion", indicador.TiempoInvestigacion >= requisito.TiempoInvestigacionMinimo }
            };
        }        public async Task ActualizarIndicadoresAsync(int docenteId, int tiempoRol, int numeroObras, decimal puntajeEvaluacion, int horasCapacitacion, int tiempoInvestigacion)
        {
            var docente = await _docenteRepository.GetByIdAsync(docenteId);
            if (docente == null)
                throw new Exception("Docente no encontrado");

            // Actualizar o crear los indicadores del docente
            if (docente.Indicadores == null)
            {
                docente.Indicadores = new IndicadorDocente
                {
                    DocenteId = docenteId,
                    Docente = docente,
                    TiempoEnRolActual = tiempoRol,
                    NumeroObras = numeroObras,
                    PuntajeEvaluacion = puntajeEvaluacion,
                    HorasCapacitacion = horasCapacitacion,
                    TiempoInvestigacion = tiempoInvestigacion,
                    FechaActualizacion = DateTime.UtcNow
                };
            }
            else
            {
                docente.Indicadores.TiempoEnRolActual = tiempoRol;
                docente.Indicadores.NumeroObras = numeroObras;
                docente.Indicadores.PuntajeEvaluacion = puntajeEvaluacion;
                docente.Indicadores.HorasCapacitacion = horasCapacitacion;
                docente.Indicadores.TiempoInvestigacion = tiempoInvestigacion;
                docente.Indicadores.FechaActualizacion = DateTime.UtcNow;
            }

            await _docenteRepository.UpdateAsync(docente);
        }        public async Task UpdateDocenteAsync(Docente docente)
        {
            var existingDocente = await _docenteRepository.GetByIdAsync(docente.Id);
            if (existingDocente == null)
                throw new Exception("Docente no encontrado");

            existingDocente.Cedula = docente.Cedula;
            existingDocente.Nombres = docente.Nombres;
            existingDocente.Apellidos = docente.Apellidos;
            existingDocente.Email = docente.Email;
            existingDocente.TelefonoContacto = docente.TelefonoContacto;
            existingDocente.FacultadId = docente.FacultadId;
            existingDocente.NivelActual = docente.NivelActual;
            existingDocente.FechaIngresoNivelActual = docente.FechaIngresoNivelActual;
            existingDocente.NombreUsuario = docente.NombreUsuario;
            existingDocente.PasswordHash = docente.PasswordHash;
            existingDocente.IntentosFallidos = docente.IntentosFallidos;
            existingDocente.Bloqueado = docente.Bloqueado;
            existingDocente.FechaBloqueo = docente.FechaBloqueo;
            existingDocente.EsAdministrador = docente.EsAdministrador;
            existingDocente.Activo = docente.Activo;
            existingDocente.FechaBaja = docente.FechaBaja;
            existingDocente.MotivoBaja = docente.MotivoBaja;

            await _docenteRepository.UpdateAsync(existingDocente);
        }

        public async Task CreateDocenteAsync(Docente docente)
        {
            var existingByUsername = await _docenteRepository.GetByUsernameAsync(docente.NombreUsuario);
            if (existingByUsername != null)
            {
                throw new Exception("Ya existe un docente con este nombre de usuario");
            }

            var existingByCedula = await _docenteRepository.GetByCedulaAsync(docente.Cedula);
            if (existingByCedula != null)
            {
                throw new Exception("Ya existe un docente con esta cédula");
            }

            await _docenteRepository.AddAsync(docente);
        }

        private RequisitoAscenso? ObtenerRequisitoParaNivel(int nivelActual)
        {
            return Array.Find(ReglasAscenso.Requisitos, r => r.NivelActual == nivelActual);
        }
    }
}