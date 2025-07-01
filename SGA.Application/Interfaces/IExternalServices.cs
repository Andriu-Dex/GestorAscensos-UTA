using SGA.Application.DTOs;
using SGA.Application.DTOs.Docentes;
using SGA.Application.DTOs.ExternalData;

namespace SGA.Application.Interfaces;

public interface IExternalDataService
{
    Task<DTOs.ExternalData.DatosTTHHDto?> ImportarDatosTTHHAsync(string cedula);
    Task<DTOs.ExternalData.DatosDACDto?> ImportarDatosDACAsync(string cedula);
    Task<DTOs.ExternalData.DatosDITICDto?> ImportarDatosDITICAsync(string cedula);
    Task<DTOs.ExternalData.DatosDirInvDto?> ImportarDatosDirInvAsync(string cedula);
    Task<List<DTOs.ExternalData.ObraAcademicaConPdfDto>> ObtenerObrasAcademicasConPdfAsync(string cedula);
}

public interface IValidacionAscensoService
{
    DTOs.Docentes.RequisitoAscensoDto ValidarRequisitos(DTOs.Docentes.DocenteDto docente, Domain.Enums.NivelTitular? nivelDestino = null);
    Task<DTOs.Docentes.RequisitoAscensoDto> ValidarRequisitosAscensoAsync(Domain.Entities.Docente docente, Domain.Enums.NivelTitular? nivelDestino = null);
    Task<DTOs.Docentes.RequisitoAscensoDto> ValidarRequisitosConDatosExternosAsync(string cedula, Domain.Enums.NivelTitular? nivelDestino = null);
    bool PuedeAscender(DTOs.Docentes.DocenteDto docente);
    Domain.Enums.NivelTitular? ObtenerSiguienteNivel(Domain.Enums.NivelTitular nivelActual);
}
