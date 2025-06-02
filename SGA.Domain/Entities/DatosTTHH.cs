using System;

namespace SGA.Domain.Entities
{    public class DatosTTHH
    {
        public int Id { get; set; }
        public required string Cedula { get; set; }
        public required string Nombres { get; set; }
        public required string Apellidos { get; set; }
        public required string Facultad { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string? Celular { get; set; }
    }
}
