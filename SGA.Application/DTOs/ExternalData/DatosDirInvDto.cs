using System;
using System.Collections.Generic;

namespace SGA.Application.DTOs.ExternalData;

public class DatosDirInvDto
{
    public int NumeroObrasAcademicas { get; set; }
    public int NumeroObras { get; set; } // Mantenemos para compatibilidad
    public int MesesInvestigacion { get; set; }
    public int ProyectosActivos { get; set; }
    public DateTime? FechaUltimaPublicacion { get; set; }
    public List<ObraAcademicaDto> Obras { get; set; } = new();
    public List<ProyectoInvestigacionDto> Proyectos { get; set; } = new();
}

public class ObraAcademicaDto
{
    public string Titulo { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public DateTime FechaPublicacion { get; set; }
    public string Revista { get; set; } = string.Empty;
    public string Autores { get; set; } = string.Empty;
}

// DTO extendido para incluir el PDF
public class ObraAcademicaConPdfDto : ObraAcademicaDto
{
    public byte[]? PdfComprimido { get; set; }
    public string? NombreArchivo { get; set; }
    public int? TamañoOriginal { get; set; }
}

public class ProyectoInvestigacionDto
{
    public string Titulo { get; set; } = string.Empty;
    public DateTime FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
    public string Estado { get; set; } = string.Empty;
    public string Rol { get; set; } = string.Empty;
}
