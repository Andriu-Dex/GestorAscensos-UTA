namespace SGA.Application.DTOs.Docentes;

public class DocenteDto
{
    public Guid Id { get; set; }
    public string Cedula { get; set; } = string.Empty;
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string NivelActual { get; set; } = string.Empty;
    public DateTime FechaInicioNivelActual { get; set; }
    public DateTime? FechaUltimoAscenso { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    
    // Datos importados
    public DateTime? FechaNombramiento { get; set; }
    public decimal? PromedioEvaluaciones { get; set; }
    public int? HorasCapacitacion { get; set; }
    public int? NumeroObrasAcademicas { get; set; }
    public int? MesesInvestigacion { get; set; }
    public DateTime? FechaUltimaImportacion { get; set; }
    
    // Propiedades calculadas para ascenso
    public bool PuedeAscender { get; set; }
    public string SiguienteNivel { get; set; } = string.Empty;
    public ValidacionRequisitosDto? Requisitos { get; set; }
}

public class ImportarDatosRequest
{
    public string Cedula { get; set; } = string.Empty;
    public string Sistema { get; set; } = string.Empty; // TTHH, DAC, DITIC, DIRINV
}

public class ImportarDatosResponse
{
    public bool Exitoso { get; set; }
    public string Mensaje { get; set; } = string.Empty;
    public Dictionary<string, object?> DatosImportados { get; set; } = new();
}

public class ValidacionRequisitosDto
{
    public string NivelActual { get; set; } = string.Empty;
    public string NivelSiguiente { get; set; } = string.Empty;
    public bool CumpleTodos { get; set; }
    public List<RequisitoDto> Requisitos { get; set; } = new();
}

public class RequisitoDto
{
    public string Nombre { get; set; } = string.Empty;
    public string ValorRequerido { get; set; } = string.Empty;
    public string ValorActual { get; set; } = string.Empty;
    public bool Cumple { get; set; }
    public string Descripcion { get; set; } = string.Empty;
}
