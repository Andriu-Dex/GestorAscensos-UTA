using System;
using System.Collections.Generic;

namespace SGA.Application.DTOs.ExternalData;

public class DatosDITICDto
{
    public int HorasCapacitacion { get; set; }
    public int CursosCompletados { get; set; }
    public DateTime? FechaUltimoCurso { get; set; }
    public List<CursoDto> Cursos { get; set; } = new();
}

public class CursoDto
{
    public string Nombre { get; set; } = string.Empty;
    public int Horas { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
    public bool Completado { get; set; }
}
