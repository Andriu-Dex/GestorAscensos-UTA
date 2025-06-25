namespace SGA.Domain.Enums;

public enum RolUsuario
{
    Docente = 1,
    Administrador = 2
}

public enum NivelTitular
{
    Titular1 = 1,
    Titular2 = 2,
    Titular3 = 3,
    Titular4 = 4,
    Titular5 = 5
}

public enum EstadoSolicitud
{
    Pendiente = 1,
    EnProceso = 2,
    Aprobada = 3,
    Rechazada = 4
}

public enum TipoDocumento
{
    CertificadoTrabajo = 1,
    EvaluacionesDocentes = 2,
    CertificadosCapacitacion = 3,
    ObrasAcademicas = 4,
    CertificadoInvestigacion = 5,
    Otro = 6
}
