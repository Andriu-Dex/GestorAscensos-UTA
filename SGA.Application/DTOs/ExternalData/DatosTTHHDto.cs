using System;
using System.Collections.Generic;

namespace SGA.Application.DTOs.ExternalData;

public class DatosTTHHDto
{
    // Datos personales
    public string Cedula { get; set; } = string.Empty;
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool Activo { get; set; }
    
    // Datos académicos
    public string NivelAcademico { get; set; } = string.Empty;
    public string NivelActual { get; set; } = string.Empty;
    public int DiasEnNivelActual { get; set; }
    
    // Datos laborales
    public DateTime? FechaNombramiento { get; set; }
    public string CargoActual { get; set; } = string.Empty;
    public DateTime? FechaInicioCargoActual { get; set; }
    public DateTime? FechaIngresoNivelActual { get; set; }
    public string Facultad { get; set; } = string.Empty;
    public string Departamento { get; set; } = string.Empty;
    
    // Historial de promociones
    public List<PromocionDto> HistorialPromociones { get; set; } = new();
}
