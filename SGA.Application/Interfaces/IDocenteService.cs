using SGA.Application.DTOs.Docentes;

namespace SGA.Application.Interfaces;

public interface IDocenteService
{
    Task<DocenteDto?> GetDocenteByIdAsync(Guid id);
    Task<DocenteDto?> GetDocenteByCedulaAsync(string cedula);
    Task<DocenteDto?> GetDocenteByEmailAsync(string email);
    Task<ImportarDatosResponse> ImportarDatosTTHHAsync(string cedula);
    Task<ImportarDatosResponse> ImportarTiempoRolTTHHAsync(string cedula);
    Task<ImportarDatosResponse> ImportarDatosDACAsync(string cedula);
    Task<ImportarDatosResponse> ImportarDatosDITICAsync(string cedula);
    Task<ImportarDatosResponse> ImportarDatosDIRINVAsync(string cedula);
    Task<ImportarDatosResponse> ImportarObrasAcademicasAsync(string cedula);
    Task<ValidacionRequisitosDto> ValidarRequisitosAscensoAsync(string cedula, string nivelObjetivo);
    Task<bool> ActualizarNivelDocenteAsync(Guid docenteId, string nuevoNivel);
    Task<IndicadoresDocenteDto> GetIndicadoresAsync(string cedula);
    Task<RequisitosAscensoDto> GetRequisitosAscensoAsync(string cedula, string nivelObjetivo);
}
