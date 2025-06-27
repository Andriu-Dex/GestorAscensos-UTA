using System;
using System.Collections.Generic;

namespace SGA.Application.DTOs.ExternalData;

public class DatosDACDto
{
    public decimal PromedioEvaluaciones { get; set; }
    public int PeriodosEvaluados { get; set; }
    public DateTime FechaUltimaEvaluacion { get; set; }
    public bool CumpleRequisito { get; set; }
    public decimal RequisitoMinimo { get; set; }
    public string PeriodoEvaluado { get; set; } = string.Empty;
    public string Mensaje { get; set; } = string.Empty;
    public List<EvaluacionPeriodoDto> Evaluaciones { get; set; } = new();
}

public class EvaluacionPeriodoDto
{
    public string Periodo { get; set; } = string.Empty;
    public decimal Calificacion { get; set; }
    public decimal Porcentaje { get; set; }
    public DateTime Fecha { get; set; }
    public int EstudiantesEvaluaron { get; set; }
}
