namespace SGA.Web.Models;

public class ImportarDatosResponse
{
    public bool Exitoso { get; set; }
    public string Mensaje { get; set; } = string.Empty;
    public Dictionary<string, object> DatosImportados { get; set; } = new();
}
