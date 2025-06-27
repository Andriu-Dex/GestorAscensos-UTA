using SGA.Application.DTOs.Docentes;
using SGA.Domain.Enums;

namespace SGA.Application.DTOs;

// Renombrando la clase para evitar conflictos de nombres
public class DocenteDtoV1
{
    public Guid Id { get; set; }
    public string Cedula { get; set; } = string.Empty;
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public NivelTitular NivelActual { get; set; }
    public DateTime FechaInicioNivelActual { get; set; }
    public DateTime? FechaUltimoAscenso { get; set; }
    public bool EstaActivo { get; set; }
    
    // Datos importados
    public DateTime? FechaNombramiento { get; set; }
    public decimal? PromedioEvaluaciones { get; set; }
    public int? HorasCapacitacion { get; set; }
    public int? NumeroObrasAcademicas { get; set; }
    public int? MesesInvestigacion { get; set; }
    public DateTime? FechaUltimaImportacion { get; set; }
    
    // Propiedades calculadas
    public string NombreCompleto => $"{Nombres} {Apellidos}";
    public bool PuedeAscender { get; set; }
    public string SiguienteNivel { get; set; } = string.Empty;
    public Docentes.RequisitoAscensoDto? Requisitos { get; set; }
}

public class CreateDocenteDto
{
    public string Cedula { get; set; } = string.Empty;
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public NivelTitular NivelActual { get; set; } = NivelTitular.Titular1;
    public DateTime FechaInicioNivelActual { get; set; } = DateTime.Now;
}

public class UpdateDocenteDto
{
    public Guid Id { get; set; }
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public NivelTitular NivelActual { get; set; }
    public DateTime FechaInicioNivelActual { get; set; }
    public bool EstaActivo { get; set; }
}
