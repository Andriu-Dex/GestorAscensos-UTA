namespace SGA.Application.DTOs.Admin;

/// <summary>
/// DTO para visualizar título académico
/// </summary>
public class TituloAcademicoDto
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public int OrdenJerarquico { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public bool EstaActivo { get; set; }
    public string? ModificadoPor { get; set; }
    public bool EsTituloSistema { get; set; }
    public int? NivelEquivalente { get; set; }
    public string? ColorHex { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaModificacion { get; set; }
    
    // Propiedades calculadas
    public string RepresentacionCompleta { get; set; } = string.Empty;
    public string EstadoTexto => EstaActivo ? "Activo" : "Inactivo";
    public string TipoTexto => EsTituloSistema ? "Sistema" : "Personalizado";
}

/// <summary>
/// DTO para crear o actualizar título académico
/// </summary>
public class CrearActualizarTituloAcademicoDto
{
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public int OrdenJerarquico { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public bool EstaActivo { get; set; } = true;
    public int? NivelEquivalente { get; set; }
    public string? ColorHex { get; set; }
}

/// <summary>
/// DTO para listado resumido de títulos académicos
/// </summary>
public class TituloAcademicoResumenDto
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Codigo { get; set; } = string.Empty;
    public int OrdenJerarquico { get; set; }
    public bool EstaActivo { get; set; }
    public bool EsTituloSistema { get; set; }
    public string? ColorHex { get; set; }
    public DateTime FechaModificacion { get; set; }
    public string? ModificadoPor { get; set; }
}

/// <summary>
/// DTO para opciones de selección en formularios
/// </summary>
public class TituloAcademicoOpcionDto
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Codigo { get; set; } = string.Empty;
    public int OrdenJerarquico { get; set; }
    public bool EsTituloSistema { get; set; }
    public bool EstaActivo { get; set; }
    public string? ColorHex { get; set; }
    public string? Descripcion { get; set; }
    
    public string TextoOpcion => $"{Nombre} (Orden: {OrdenJerarquico})";
}

/// <summary>
/// DTO híbrido para configuraciones que combina enum y títulos dinámicos
/// </summary>
public class NivelAcademicoHibridoDto
{
    public string Id { get; set; } = string.Empty; // "enum_1" o "titulo_guid"
    public string Nombre { get; set; } = string.Empty;
    public string Codigo { get; set; } = string.Empty;
    public int OrdenJerarquico { get; set; }
    public bool EsEnum { get; set; }
    public bool EsTituloSistema { get; set; }
    public string? ColorHex { get; set; }
    public string Tipo => EsEnum ? "Predefinido" : "Personalizado";
}
