using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SGA.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FacultadController : ControllerBase
{
    [HttpGet]
    public ActionResult<List<FacultadDto>> GetFacultades()
    {
        try
        {
            var facultades = new List<FacultadDto>
            {
                new FacultadDto { Id = 1, Codigo = "FISEI", Nombre = "Facultad de Ingeniería en Sistemas, Electrónica e Industrial", Descripcion = "Facultad de tecnología e ingeniería", Color = "#007BFF", EsActiva = true },
                new FacultadDto { Id = 2, Codigo = "FCHE", Nombre = "Facultad de Ciencias Humanas y de la Educación", Descripcion = "Facultad de educación y humanidades", Color = "#28A745", EsActiva = true },
                new FacultadDto { Id = 3, Codigo = "FCA", Nombre = "Facultad de Contabilidad y Auditoría", Descripcion = "Facultad de ciencias económicas", Color = "#FFC107", EsActiva = true },
                new FacultadDto { Id = 4, Codigo = "FCAG", Nombre = "Facultad de Ciencias Agropecuarias", Descripcion = "Facultad de agricultura y ganadería", Color = "#17A2B8", EsActiva = true },
                new FacultadDto { Id = 5, Codigo = "FCS", Nombre = "Facultad de Ciencias de la Salud", Descripcion = "Facultad de medicina y salud", Color = "#DC3545", EsActiva = true }
            };

            return Ok(facultades);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public ActionResult<FacultadDto> GetFacultad(int id)
    {
        try
        {
            var facultades = new List<FacultadDto>
            {
                new FacultadDto { Id = 1, Codigo = "FISEI", Nombre = "Facultad de Ingeniería en Sistemas, Electrónica e Industrial", Descripcion = "Facultad de tecnología e ingeniería", Color = "#007BFF", EsActiva = true },
                new FacultadDto { Id = 2, Codigo = "FCHE", Nombre = "Facultad de Ciencias Humanas y de la Educación", Descripcion = "Facultad de educación y humanidades", Color = "#28A745", EsActiva = true },
                new FacultadDto { Id = 3, Codigo = "FCA", Nombre = "Facultad de Contabilidad y Auditoría", Descripcion = "Facultad de ciencias económicas", Color = "#FFC107", EsActiva = true },
                new FacultadDto { Id = 4, Codigo = "FCAG", Nombre = "Facultad de Ciencias Agropecuarias", Descripcion = "Facultad de agricultura y ganadería", Color = "#17A2B8", EsActiva = true },
                new FacultadDto { Id = 5, Codigo = "FCS", Nombre = "Facultad de Ciencias de la Salud", Descripcion = "Facultad de medicina y salud", Color = "#DC3545", EsActiva = true }
            };

            var facultad = facultades.FirstOrDefault(f => f.Id == id);
            if (facultad == null)
                return NotFound("Facultad no encontrada");

            return Ok(facultad);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpPost]
    [Authorize(Roles = "Administrador")]
    public ActionResult<FacultadDto> CrearFacultad([FromBody] CrearFacultadDto facultad)
    {
        try
        {
            // En un sistema real, esto se guardaría en la base de datos
            var nuevaFacultad = new FacultadDto
            {
                Id = new Random().Next(1000, 9999), // ID temporal
                Codigo = facultad.Codigo,
                Nombre = facultad.Nombre,
                Descripcion = facultad.Descripcion,
                Color = facultad.Color,
                EsActiva = facultad.EsActiva
            };

            return CreatedAtAction(nameof(GetFacultad), new { id = nuevaFacultad.Id }, nuevaFacultad);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }
}

public class FacultadDto
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public bool EsActiva { get; set; }
}

public class CrearFacultadDto
{
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string Color { get; set; } = "#007BFF";
    public bool EsActiva { get; set; } = true;
}
