using System.Collections.Generic;
using System.Threading.Tasks;
using SGA.Domain.Entities;

namespace SGA.Application.Services
{    public interface IDocenteService
    {
        Task<Docente> GetDocenteByIdAsync(int id);
        Task<Docente> GetDocenteByCedulaAsync(string cedula);
        Task<Docente> GetDocenteByUsernameAsync(string username);
        Task<Docente> GetDocenteByEmailAsync(string email);
        Task<bool> ValidarRequisitosAscensoAsync(int docenteId);
        Task<Dictionary<string, bool>> ObtenerEstadoRequisitosAsync(int docenteId);
        Task ActualizarIndicadoresAsync(int docenteId, int tiempoRol, int numeroObras, decimal puntajeEvaluacion, int horasCapacitacion, int tiempoInvestigacion);
        Task UpdateDocenteAsync(Docente docente);
        Task CreateDocenteAsync(Docente docente);
    }
}
