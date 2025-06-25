// Entidades para la base de datos TTHH (Talento Humano)
namespace SGA.Domain.Entities.External;

public class EmpleadoTTHH
{
    public int Id { get; set; }
    public string Cedula { get; set; } = string.Empty;
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime FechaNombramiento { get; set; }
    public string CargoActual { get; set; } = string.Empty;
    public string NivelAcademico { get; set; } = string.Empty;
    public bool EstaActivo { get; set; }
}

public class AccionPersonalTTHH
{
    public int Id { get; set; }
    public string Cedula { get; set; } = string.Empty;
    public string TipoAccion { get; set; } = string.Empty;
    public DateTime FechaAccion { get; set; }
    public string CargoAnterior { get; set; } = string.Empty;
    public string CargoNuevo { get; set; } = string.Empty;
    public string Observaciones { get; set; } = string.Empty;
}

public class CargoTTHH
{
    public int Id { get; set; }
    public string NombreCargo { get; set; } = string.Empty;
    public string NivelTitular { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public bool EstaActivo { get; set; }
}
