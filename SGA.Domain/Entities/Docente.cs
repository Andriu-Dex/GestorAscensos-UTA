using SGA.Domain.Common;
using SGA.Domain.Enums;
using SGA.Domain.Extensions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGA.Domain.Entities;

public class Docente : BaseEntity
{
    public string Cedula { get; set; } = string.Empty;
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Celular { get; set; }
    public NivelTitular NivelActual { get; set; } = NivelTitular.Titular1;
    public DateTime FechaInicioNivelActual { get; set; }
    public DateTime? FechaUltimoAscenso { get; set; }
    public bool EstaActivo { get; set; } = true;
    
    // Datos importados de sistemas externos
    public DateTime? FechaNombramiento { get; set; }
    public decimal? PromedioEvaluaciones { get; set; }
    public int? HorasCapacitacion { get; set; }
    public int? NumeroObrasAcademicas { get; set; }
    public int? MesesInvestigacion { get; set; }
    public DateTime? FechaUltimaImportacion { get; set; }
    
    // Foto de perfil
    [Column(TypeName = "varbinary(max)")]
    public byte[]? FotoPerfil { get; set; }

    // Propiedad calculada para mostrar en frontend
    [NotMapped]
    public string? FotoPerfilBase64 => FotoPerfil != null 
        ? Convert.ToBase64String(FotoPerfil)
        : null;
    
    // Relaciones
    public Guid UsuarioId { get; set; }
    public virtual Usuario Usuario { get; set; } = null!;
    
    // Relación con Departamento
    public Guid? DepartamentoId { get; set; }
    public virtual Departamento? Departamento { get; set; }
    
    public virtual ICollection<SolicitudAscenso> SolicitudesAscenso { get; set; } = new List<SolicitudAscenso>();
    public virtual ICollection<ObraAcademica> ObrasAcademicas { get; set; } = new List<ObraAcademica>();
    
    // Propiedades calculadas
    public string NombreCompleto => $"{Nombres} {Apellidos}";
    public string NivelDescripcion => NivelActual.GetDescription();
    public bool PuedeAscender => !NivelActual.EsNivelMaximo();
    public NivelTitular? SiguienteNivel => NivelActual.GetSiguienteNivel();
    
    // Métodos de validación usando las extensiones de enum
    public bool CumpleRequisitosParaNivel(NivelTitular nivelObjetivo)
    {
        // Validar que el ascenso sea secuencial
        if (!NivelActual.EsAscensoValido(nivelObjetivo))
            return false;
            
        var tiempoEnNivel = DateTime.UtcNow - FechaInicioNivelActual;
        var requisitos = nivelObjetivo.GetRequisitos();
        
        return tiempoEnNivel.TotalDays >= (requisitos.AnosEnNivel * 365) &&
               (NumeroObrasAcademicas ?? 0) >= requisitos.ObrasMinimas &&
               (PromedioEvaluaciones ?? 0) >= requisitos.PromedioEvaluacionMinimo &&
               (HorasCapacitacion ?? 0) >= requisitos.HorasCapacitacionMinimas &&
               (MesesInvestigacion ?? 0) >= requisitos.MesesInvestigacionMinimos;
    }
    
    /// <summary>
    /// Valida si el docente puede crear una nueva solicitud de ascenso
    /// </summary>
    /// <returns>True si puede crear una solicitud</returns>
    public bool PuedeCrearSolicitudAscenso()
    {
        // No puede ascender si ya está en el nivel máximo
        if (NivelActual.EsNivelMaximo())
            return false;
            
        // No puede crear solicitud si ya tiene una activa
        if (TieneSolicitudActiva())
            return false;
            
        // Debe cumplir los requisitos para el siguiente nivel
        var siguienteNivel = NivelActual.GetSiguienteNivel();
        return siguienteNivel.HasValue && CumpleRequisitosParaNivel(siguienteNivel.Value);
    }
    
    /// <summary>
    /// Verifica si el docente tiene una solicitud activa
    /// </summary>
    /// <returns>True si tiene una solicitud activa</returns>
    public bool TieneSolicitudActiva()
    {
        return SolicitudesAscenso.Any(s => s.Estado.EsEstadoActivo());
    }
    
    /// <summary>
    /// Obtiene los requisitos que cumple actualmente para el siguiente nivel
    /// </summary>
    /// <returns>Objeto con el estado de cumplimiento de requisitos</returns>
    public EstadoRequisitos GetEstadoRequisitos()
    {
        var siguienteNivel = NivelActual.GetSiguienteNivel();
        if (!siguienteNivel.HasValue)
        {
            return new EstadoRequisitos { NivelMaximoAlcanzado = true };
        }
        
        var requisitos = siguienteNivel.Value.GetRequisitos();
        var tiempoEnNivel = DateTime.UtcNow - FechaInicioNivelActual;
        
        return new EstadoRequisitos
        {
            NivelObjetivo = siguienteNivel.Value,
            CumpleTiempo = tiempoEnNivel.TotalDays >= (requisitos.AnosEnNivel * 365),
            CumpleObras = (NumeroObrasAcademicas ?? 0) >= requisitos.ObrasMinimas,
            CumpleEvaluaciones = (PromedioEvaluaciones ?? 0) >= requisitos.PromedioEvaluacionMinimo,
            CumpleCapacitacion = (HorasCapacitacion ?? 0) >= requisitos.HorasCapacitacionMinimas,
            CumpleInvestigacion = (MesesInvestigacion ?? 0) >= requisitos.MesesInvestigacionMinimos,
            DiasEnNivel = (int)tiempoEnNivel.TotalDays,
            ObrasActuales = NumeroObrasAcademicas ?? 0,
            PromedioActual = PromedioEvaluaciones ?? 0,
            HorasActuales = HorasCapacitacion ?? 0,
            MesesInvestigacionActuales = MesesInvestigacion ?? 0
        };
    }
}

/// <summary>
/// Clase que representa el estado de cumplimiento de requisitos
/// </summary>
public class EstadoRequisitos
{
    public bool NivelMaximoAlcanzado { get; set; }
    public NivelTitular? NivelObjetivo { get; set; }
    public bool CumpleTiempo { get; set; }
    public bool CumpleObras { get; set; }
    public bool CumpleEvaluaciones { get; set; }
    public bool CumpleCapacitacion { get; set; }
    public bool CumpleInvestigacion { get; set; }
    public int DiasEnNivel { get; set; }
    public int ObrasActuales { get; set; }
    public decimal PromedioActual { get; set; }
    public int HorasActuales { get; set; }
    public int MesesInvestigacionActuales { get; set; }
    
    public bool CumpleTodosLosRequisitos => 
        CumpleTiempo && CumpleObras && CumpleEvaluaciones && 
        CumpleCapacitacion && CumpleInvestigacion;
}
