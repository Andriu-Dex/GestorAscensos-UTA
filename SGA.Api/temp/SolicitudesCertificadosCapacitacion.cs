using System;
using System.Collections.Generic;

namespace SGA.Api.temp;

public partial class SolicitudesCertificadosCapacitacion
{
    public Guid Id { get; set; }

    public Guid DocenteId { get; set; }

    public string DocenteCedula { get; set; } = null!;

    public string NombreCurso { get; set; } = null!;

    public string InstitucionOfertante { get; set; } = null!;

    public string TipoCapacitacion { get; set; } = null!;

    public DateTime FechaInicio { get; set; }

    public DateTime FechaFin { get; set; }

    public int HorasDuracion { get; set; }

    public string Modalidad { get; set; } = null!;

    public string? NumeroRegistro { get; set; }

    public string? AreaTematica { get; set; }

    public string? Descripcion { get; set; }

    public string? ArchivoNombre { get; set; }

    public string? ArchivoTipo { get; set; }

    public long? ArchivoTamano { get; set; }

    public string Estado { get; set; } = null!;

    public string? ComentariosRevision { get; set; }

    public Guid? RevisadoPorId { get; set; }

    public DateTime? FechaRevision { get; set; }

    public string? MotivoRechazo { get; set; }

    public string? ComentariosSolicitud { get; set; }

    public Guid SolicitudGrupoId { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public long? ArchivoTamanoOriginal { get; set; }

    public byte[]? ContenidoArchivo { get; set; }

    public byte[]? ArchivoContenido { get; set; }

    public bool? ArchivoEstaComprimido { get; set; }

    public long? ArchivoTamanoComprimido { get; set; }

    public virtual Docente Docente { get; set; } = null!;

    public virtual Usuario? RevisadoPor { get; set; }
}
