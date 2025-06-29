using System.ComponentModel;

namespace SGA.Domain.Enums;

/// <summary>
/// Roles de usuario en el sistema
/// </summary>
public enum RolUsuario
{
    [Description("Docente")]
    Docente = 1,
    
    [Description("Administrador")]
    Administrador = 2
}

/// <summary>
/// Niveles de Titular Auxiliar para docentes universitarios
/// Solo se permiten valores de 1 a 5 según la estructura académica
/// </summary>
public enum NivelTitular
{
    [Description("Titular Auxiliar 1")]
    Titular1 = 1,
    
    [Description("Titular Auxiliar 2")]
    Titular2 = 2,
    
    [Description("Titular Auxiliar 3")]
    Titular3 = 3,
    
    [Description("Titular Auxiliar 4")]
    Titular4 = 4,
    
    [Description("Titular Auxiliar 5")]
    Titular5 = 5
}

/// <summary>
/// Estados posibles para las solicitudes de ascenso
/// </summary>
public enum EstadoSolicitud
{
    [Description("Pendiente")]
    Pendiente = 1,
    
    [Description("En Proceso")]
    EnProceso = 2,
    
    [Description("Aprobada")]
    Aprobada = 3,
    
    [Description("Rechazada")]
    Rechazada = 4
}

/// <summary>
/// Tipos de documentos que se pueden adjuntar a las solicitudes
/// </summary>
public enum TipoDocumento
{
    [Description("Certificado de Trabajo")]
    CertificadoTrabajo = 1,
    
    [Description("Evaluaciones Docentes")]
    EvaluacionesDocentes = 2,
    
    [Description("Certificados de Capacitación")]
    CertificadosCapacitacion = 3,
    
    [Description("Obras Académicas")]
    ObrasAcademicas = 4,
    
    [Description("Certificado de Investigación")]
    CertificadoInvestigacion = 5,
    
    [Description("Otro")]
    Otro = 6
}
