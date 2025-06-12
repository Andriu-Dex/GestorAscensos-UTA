using System;

namespace SGA.Domain.Entities
{
    public class DatosTTHH
    {
        public int Id { get; set; }
        public required string Cedula { get; set; }
        public required string Nombres { get; set; }
        public required string Apellidos { get; set; }
        public int FacultadId { get; set; }
        public string? Celular { get; set; }
        public string? TelefonoConvencional { get; set; }
        public string? EmailPersonal { get; set; }
        public string? Direccion { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public string? EstadoCivil { get; set; }        public DateTime FechaIngreso { get; set; }
        public DateTime FechaRegistro { get; set; }
        public DateTime FechaActualizacion { get; set; }
        public bool Activo { get; set; } = true;
        
        // Navegación
        public required Facultad Facultad { get; set; }
    }
}
