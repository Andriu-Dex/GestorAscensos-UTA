namespace SGA.Web.Models
{
    public class UserInfoModel
    {
        public string Id { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Nombres { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Cedula { get; set; } = string.Empty;
        public string Facultad { get; set; } = string.Empty;
        public string Departamento { get; set; } = string.Empty;
        public string TelefonoContacto { get; set; } = string.Empty;
        public int FacultadId { get; set; }
        public FacultadInfo? FacultadInfo { get; set; }
        public int NivelActual { get; set; }
        public DateTime FechaIngresoNivelActual { get; set; }
        public bool EsAdmin { get; set; }
    }

    public class FacultadInfo
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
    }
}
