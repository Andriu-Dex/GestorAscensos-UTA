using System.Threading.Tasks;

namespace SGA.Application.Interfaces
{
    public interface IEmailService
    {
        Task<bool> EnviarCorreoAsync(string destinatario, string asunto, string cuerpoHtml, string? cuerpoTexto = null);
        Task<bool> EnviarCorreoFelicitacionAscensoAsync(string emailDocente, string nombreDocente, string nivelAnterior, string nivelNuevo);
        Task<bool> EnviarCorreoRechazoAscensoAsync(string emailDocente, string nombreDocente, string nivelSolicitado, string motivo);
    }
}
