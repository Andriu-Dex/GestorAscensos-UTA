using System.Collections.Generic;
using System.Threading.Tasks;
using SGA.Application.DTOs;

namespace SGA.Application.Services
{
    public interface IValidacionAscensoService
    {
        /// <summary>
        /// Valida si un docente cumple con los requisitos para ascenso según su nivel actual
        /// </summary>
        Task<bool> ValidarRequisitosAscensoAsync(int docenteId);
        
        /// <summary>
        /// Valida los requisitos para un ascenso específico
        /// </summary>
        Task<RequisitosAscensoResult> ValidarRequisitosAscensoAsync(int docenteId, int nivelSolicitado);
        
        /// <summary>
        /// Obtiene un diccionario con el estado de cada requisito para el ascenso
        /// </summary>
        Task<Dictionary<string, bool>> ObtenerEstadoRequisitosAsync(int docenteId, int? nivelActual = null);
        
        /// <summary>
        /// Valida si los indicadores cumplen con los requisitos para un ascenso específico
        /// </summary>
        bool ValidarIndicadoresParaAscenso(int nivelActual, int tiempoRol, int numeroObras, 
            decimal puntajeEvaluacion, int horasCapacitacion, int tiempoInvestigacion);
    }
}
