using System;
using System.Collections.Generic;

namespace SGA.Domain.Entities
{
    public class Docente
    {
        public int Id { get; set; }
        public string Cedula { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string Email { get; set; }
        public string TelefonoContacto { get; set; }
        public string Facultad { get; set; }
        public string Departamento { get; set; }

        // Nivel actual del docente (Titular 1, 2, 3, 4, 5)
        public int NivelActual { get; set; }

        // Fechas importantes
        public DateTime FechaIngresoNivelActual { get; set; }
        
        // Indicadores para ascenso
        public int TiempoEnRolActual { get; set; } // En años
        public int NumeroObras { get; set; }
        public decimal PuntajeEvaluacion { get; set; } // En porcentaje
        public int HorasCapacitacion { get; set; }
        public int TiempoInvestigacion { get; set; } // En meses
        
        // Navegación
        public ICollection<Documento> Documentos { get; set; }
        public ICollection<SolicitudAscenso> Solicitudes { get; set; }
        
        // Autenticación
        public string NombreUsuario { get; set; }
        public string PasswordHash { get; set; }
        public int IntentosFallidos { get; set; }
        public bool Bloqueado { get; set; }
        public DateTime? FechaBloqueo { get; set; }
    }
}
