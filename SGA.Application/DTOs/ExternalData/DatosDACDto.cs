using System;
using System.Collections.Generic;

namespace SGA.Application.DTOs.ExternalData;

public class DatosDACDto
{
    public decimal PromedioEvaluaciones { get; set; }
    public int PeriodosEvaluados { get; set; }
    public DateTime FechaUltimaEvaluacion { get; set; }
    public List<EvaluacionPeriodoDto> Evaluaciones { get; set; } = new();
}

public class EvaluacionPeriodoDto
{
    public string Periodo { get; set; } = string.Empty;
    public decimal Calificacion { get; set; }
    public DateTime Fecha { get; set; }
}
