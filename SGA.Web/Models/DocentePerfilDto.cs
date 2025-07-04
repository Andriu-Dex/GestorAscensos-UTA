namespace SGA.Web.Models
{
    public class DocentePerfilDto
    {
        public Guid Id { get; set; }
        public string Cedula { get; set; } = string.Empty;
        public string Nombres { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string NivelActual { get; set; } = string.Empty;
        public DateTime FechaInicioNivelActual { get; set; }
        public DateTime? FechaUltimoAscenso { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        
        // Datos importados
        public DateTime? FechaNombramiento { get; set; }
        public decimal? PromedioEvaluaciones { get; set; }
        public int? HorasCapacitacion { get; set; }
        public int? NumeroObrasAcademicas { get; set; }
        public int? MesesInvestigacion { get; set; }
        public DateTime? FechaUltimaImportacion { get; set; }
        
        // Propiedades calculadas para ascenso
        public bool PuedeAscender { get; set; }
        public string SiguienteNivel { get; set; } = string.Empty;
        
        // Información adicional que puede no estar en el DTO original
        public string? Departamento { get; set; }
        public FacultadInfo? Facultad { get; set; }
        
        // Foto de perfil
        public string? FotoPerfilBase64 { get; set; }
    }
}
