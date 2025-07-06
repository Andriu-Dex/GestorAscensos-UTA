using SGA.Domain.Common;

namespace SGA.Domain.Entities;

/// <summary>
/// Entidad para títulos académicos dinámicos que complementa el sistema de niveles predefinidos.
/// Permite crear títulos personalizados manteniendo compatibilidad con el enum NivelTitular.
/// </summary>
public class TituloAcademico : BaseEntity
{
    /// <summary>
    /// Nombre del título académico (ej: "Profesor Asociado", "Investigador Senior")
    /// </summary>
    public string Nombre { get; set; } = string.Empty;
    
    /// <summary>
    /// Descripción detallada del título
    /// </summary>
    public string? Descripcion { get; set; }
    
    /// <summary>
    /// Orden jerárquico del título (para determinar ascensos válidos)
    /// </summary>
    public int OrdenJerarquico { get; set; }
    
    /// <summary>
    /// Código único del título (ej: "PROF_ASOC", "INV_SR")
    /// </summary>
    public string Codigo { get; set; } = string.Empty;
    
    /// <summary>
    /// Indica si el título está activo y disponible para uso
    /// </summary>
    public bool EstaActivo { get; set; } = true;
    
    /// <summary>
    /// Usuario que creó o modificó por última vez este título
    /// </summary>
    public string? ModificadoPor { get; set; }
    
    /// <summary>
    /// Indica si es un título del sistema (predefinido) o personalizado
    /// Los títulos del sistema no se pueden eliminar
    /// </summary>
    public bool EsTituloSistema { get; set; } = false;
    
    /// <summary>
    /// Referencia al nivel equivalente del enum (opcional, para compatibilidad)
    /// </summary>
    public int? NivelEquivalente { get; set; }
    
    /// <summary>
    /// Color hex para representación visual en la interfaz
    /// </summary>
    public string? ColorHex { get; set; }
    
    /// <summary>
    /// Configuraciones de requisitos donde este título es el nivel actual
    /// </summary>
    public virtual ICollection<ConfiguracionRequisito> ConfiguracionesComoActual { get; set; } = new List<ConfiguracionRequisito>();
    
    /// <summary>
    /// Configuraciones de requisitos donde este título es el nivel solicitado
    /// </summary>
    public virtual ICollection<ConfiguracionRequisito> ConfiguracionesComoSolicitado { get; set; } = new List<ConfiguracionRequisito>();
    
    /// <summary>
    /// Valida que el título académico tenga datos consistentes
    /// </summary>
    public bool EsTituloValido()
    {
        if (string.IsNullOrWhiteSpace(Nombre) || Nombre.Length < 3 || Nombre.Length > 100)
            return false;
            
        if (string.IsNullOrWhiteSpace(Codigo) || Codigo.Length < 2 || Codigo.Length > 20)
            return false;
            
        if (OrdenJerarquico < 1 || OrdenJerarquico > 100)
            return false;
            
        // Validar formato del color si se proporciona
        if (!string.IsNullOrEmpty(ColorHex) && !ColorHex.StartsWith("#"))
            return false;
            
        return true;
    }
    
    /// <summary>
    /// Obtiene una representación completa del título
    /// </summary>
    public string ObtenerRepresentacionCompleta()
    {
        var partes = new List<string> { Nombre };
        
        if (!string.IsNullOrEmpty(Descripcion))
            partes.Add($"({Descripcion})");
            
        partes.Add($"[Orden: {OrdenJerarquico}]");
        
        if (EsTituloSistema)
            partes.Add("[Sistema]");
            
        return string.Join(" ", partes);
    }
}
