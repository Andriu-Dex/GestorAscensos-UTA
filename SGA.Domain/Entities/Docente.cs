using System;
using System.Collections.Generic;

namespace SGA.Domain.Entities
{    public class Docente
    {
        public Docente()
        {
            Documentos = new List<Documento>();
            Solicitudes = new List<SolicitudAscenso>();
            LogsAuditoria = new List<LogAuditoria>();
        }

        public int Id { get; set; }
        public required string Cedula { get; set; }
        public required string Nombres { get; set; }
        public required string Apellidos { get; set; }
        public required string Email { get; set; }
        public required string TelefonoContacto { get; set; }
        
        // Relación con Facultad (normalizada)
        public int FacultadId { get; set; }

        // Nivel actual del docente (Titular 1, 2, 3, 4, 5)
        public int NivelActual { get; set; }

        // Fechas importantes
        public DateTime FechaIngresoNivelActual { get; set; }
        
        // Autenticación
        public required string NombreUsuario { get; set; }
        public required string PasswordHash { get; set; }
        public int IntentosFallidos { get; set; }
        public bool Bloqueado { get; set; }
        public DateTime? FechaBloqueo { get; set; }
        public bool EsAdministrador { get; set; }
        public DateTime FechaRegistro { get; set; }
        
        // Estado del docente
        public bool Activo { get; set; } = true;
        public DateTime? FechaBaja { get; set; }
        public string? MotivoBaja { get; set; }
        
        // Navegación
        public required Facultad Facultad { get; set; }
        public IndicadorDocente? Indicadores { get; set; }
        public ICollection<Documento> Documentos { get; set; }
        public ICollection<SolicitudAscenso> Solicitudes { get; set; }
        public ICollection<LogAuditoria> LogsAuditoria { get; set; }
    }
}
