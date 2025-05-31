using System;

namespace SGA.Domain.Entities
{
    public class Documento
    {
        public int Id { get; set; }
        public int DocenteId { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public TipoDocumento Tipo { get; set; }
        public byte[] Contenido { get; set; }
        public string ContentType { get; set; }
        public long TamanioBytes { get; set; }
        public DateTime FechaSubida { get; set; }
        
        // Navegación
        public Docente Docente { get; set; }
    }
    
    public enum TipoDocumento
    {
        Obra,
        Capacitacion,
        Investigacion,
        Evaluacion,
        AccionPersonal,
        Otro
    }
}
