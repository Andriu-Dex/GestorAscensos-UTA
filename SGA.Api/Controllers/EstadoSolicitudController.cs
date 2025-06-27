using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGA.Domain.Enums;

namespace SGA.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EstadoSolicitudController : ControllerBase
{
    [HttpGet]
    public ActionResult<List<EstadoSolicitudDto>> GetEstadosSolicitud()
    {
        try
        {
            var estados = Enum.GetValues<EstadoSolicitud>()
                .Select(estado => new EstadoSolicitudDto
                {
                    Id = (int)estado,
                    Codigo = estado.ToString(),
                    Nombre = GetNombreEstado(estado),
                    Descripcion = GetDescripcionEstado(estado),
                    Color = GetColorEstado(estado),
                    EsEstadoFinal = IsEstadoFinal(estado)
                })
                .ToList();

            return Ok(estados);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public ActionResult<EstadoSolicitudDto> GetEstadoSolicitud(int id)
    {
        try
        {
            if (!Enum.IsDefined(typeof(EstadoSolicitud), id))
                return NotFound("Estado de solicitud no encontrado");

            var estado = (EstadoSolicitud)id;
            var estadoDto = new EstadoSolicitudDto
            {
                Id = (int)estado,
                Codigo = estado.ToString(),
                Nombre = GetNombreEstado(estado),
                Descripcion = GetDescripcionEstado(estado),
                Color = GetColorEstado(estado),
                EsEstadoFinal = IsEstadoFinal(estado)
            };

            return Ok(estadoDto);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    private static string GetNombreEstado(EstadoSolicitud estado)
    {
        return estado switch
        {
            EstadoSolicitud.Pendiente => "Pendiente",
            EstadoSolicitud.EnProceso => "En Proceso",
            EstadoSolicitud.Aprobada => "Aprobada",
            EstadoSolicitud.Rechazada => "Rechazada",
            _ => estado.ToString()
        };
    }

    private static string GetDescripcionEstado(EstadoSolicitud estado)
    {
        return estado switch
        {
            EstadoSolicitud.Pendiente => "Solicitud recibida, pendiente de revisión",
            EstadoSolicitud.EnProceso => "Solicitud en proceso de evaluación",
            EstadoSolicitud.Aprobada => "Solicitud aprobada y ascenso confirmado",
            EstadoSolicitud.Rechazada => "Solicitud rechazada por no cumplir requisitos",
            _ => $"Estado {estado}"
        };
    }

    private static string GetColorEstado(EstadoSolicitud estado)
    {
        return estado switch
        {
            EstadoSolicitud.Pendiente => "#FFA500", // Naranja
            EstadoSolicitud.EnProceso => "#007BFF", // Azul
            EstadoSolicitud.Aprobada => "#28A745",  // Verde
            EstadoSolicitud.Rechazada => "#DC3545", // Rojo
            _ => "#6C757D" // Gris
        };
    }

    private static bool IsEstadoFinal(EstadoSolicitud estado)
    {
        return estado == EstadoSolicitud.Aprobada || estado == EstadoSolicitud.Rechazada;
    }
}

public class EstadoSolicitudDto
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public bool EsEstadoFinal { get; set; }
}
