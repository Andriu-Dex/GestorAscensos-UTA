using System;

namespace SGA.Application.DTOs.DocumentoImportacion
{
    public class ArchivoImportadoDto
    {
        public Guid Id { get; set; }
        public string NombreArchivo { get; set; } = string.Empty;
        public string TipoDocumento { get; set; } = string.Empty;
        public long TamanoArchivo { get; set; }
        public DateTime FechaImportacion { get; set; }
        public string Estado { get; set; } = "Importado";
        public DateTime? FechaEnvioValidacion { get; set; }
        public Guid? SolicitudId { get; set; }
        public string TamanoFormateado => FormatearTamano(TamanoArchivo);

        private static string FormatearTamano(long bytes)
        {
            string[] sufijos = { "B", "KB", "MB", "GB" };
            int contador = 0;
            decimal tamano = bytes;
            
            while (Math.Round(tamano / 1024) >= 1)
            {
                tamano /= 1024;
                contador++;
            }
            
            return $"{tamano:n1} {sufijos[contador]}";
        }
    }

    public class EnviarValidacionRequestDto
    {
        public string? TipoDocumento { get; set; }
        public Dictionary<string, object> DatosFormulario { get; set; } = new();
    }
}
