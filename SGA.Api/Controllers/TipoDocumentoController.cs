using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGA.Domain.Enums;

namespace SGA.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TipoDocumentoController : ControllerBase
{
    [HttpGet]
    public ActionResult<List<TipoDocumentoDto>> GetTiposDocumento()
    {
        try
        {
            var tipos = Enum.GetValues<TipoDocumento>()
                .Select(tipo => new TipoDocumentoDto
                {
                    Id = (int)tipo,
                    Codigo = tipo.ToString(),
                    Nombre = GetNombreTipoDocumento(tipo),
                    Descripcion = GetDescripcionTipoDocumento(tipo),
                    RequiereValidacion = true,
                    FormatoEsperado = "PDF",
                    TamanoMaximoMB = 10,
                    EsActivo = true
                })
                .ToList();

            return Ok(tipos);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public ActionResult<TipoDocumentoDto> GetTipoDocumento(int id)
    {
        try
        {
            if (!Enum.IsDefined(typeof(TipoDocumento), id))
                return NotFound("Tipo de documento no encontrado");

            var tipo = (TipoDocumento)id;
            var tipoDto = new TipoDocumentoDto
            {
                Id = (int)tipo,
                Codigo = tipo.ToString(),
                Nombre = GetNombreTipoDocumento(tipo),
                Descripcion = GetDescripcionTipoDocumento(tipo),
                RequiereValidacion = true,
                FormatoEsperado = "PDF",
                TamanoMaximoMB = 10,
                EsActivo = true
            };

            return Ok(tipoDto);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    private static string GetNombreTipoDocumento(TipoDocumento tipo)
    {
        return tipo switch
        {
            TipoDocumento.CertificadoTrabajo => "Certificado de Trabajo",
            TipoDocumento.EvaluacionesDocentes => "Evaluación Docente",
            TipoDocumento.CertificadosCapacitacion => "Certificado de Capacitación",
            TipoDocumento.ObrasAcademicas => "Obra Académica",
            TipoDocumento.CertificadoInvestigacion => "Certificado de Investigación",
            TipoDocumento.Otro => "Otros Documentos",
            _ => tipo.ToString()
        };
    }

    private static string GetDescripcionTipoDocumento(TipoDocumento tipo)
    {
        return tipo switch
        {
            TipoDocumento.CertificadoTrabajo => "Certificados laborales y de experiencia",
            TipoDocumento.EvaluacionesDocentes => "Resultados de evaluaciones docentes",
            TipoDocumento.CertificadosCapacitacion => "Certificados de cursos y capacitaciones",
            TipoDocumento.ObrasAcademicas => "Publicaciones, artículos, libros y capítulos",
            TipoDocumento.CertificadoInvestigacion => "Documentos relacionados con investigación",
            TipoDocumento.Otro => "Otros documentos de respaldo",
            _ => $"Documento de tipo {tipo}"
        };
    }
}

public class TipoDocumentoDto
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public bool RequiereValidacion { get; set; }
    public string FormatoEsperado { get; set; } = string.Empty;
    public int TamanoMaximoMB { get; set; }
    public bool EsActivo { get; set; }
}
