namespace SGA.Web.Models
{
    public enum TipoDocumentoEnum
    {
        ObrasAcademicas,
        CertificadosCapacitacion,
        CertificadoInvestigacion,
        CertificadoTrabajo,
        EvaluacionesDocentes,
        Otro
    }

    public static class TipoDocumentoHelper
    {
        public static string GetDisplayName(TipoDocumentoEnum tipo)
        {
            return tipo switch
            {
                TipoDocumentoEnum.ObrasAcademicas => "Obras Académicas",
                TipoDocumentoEnum.CertificadosCapacitacion => "Certificados de Capacitación",
                TipoDocumentoEnum.CertificadoInvestigacion => "Evidencias de Investigación",
                TipoDocumentoEnum.CertificadoTrabajo => "Certificados de Trabajo",
                TipoDocumentoEnum.EvaluacionesDocentes => "Evaluaciones Docentes",
                TipoDocumentoEnum.Otro => "Otros",
                _ => tipo.ToString()
            };
        }
    }
}
