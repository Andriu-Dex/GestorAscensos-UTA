using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using SGA.Application.Interfaces;
using System;
using System.Threading.Tasks;

namespace SGA.Application.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<bool> EnviarCorreoAsync(string destinatario, string asunto, string cuerpoHtml, string? cuerpoTexto = null)
        {
            try
            {
                var mensaje = new MimeMessage();
                
                // Configurar remitente
                var fromName = _configuration["EmailSettings:FromName"] ?? "Sistema de Gestión de Ascensos";
                var fromEmail = _configuration["EmailSettings:FromEmail"] ?? "noreply@sistema.edu.ec";
                mensaje.From.Add(new MailboxAddress(fromName, fromEmail));
                
                // Configurar destinatario
                mensaje.To.Add(new MailboxAddress("", destinatario));
                mensaje.Subject = asunto;

                // Crear el cuerpo del mensaje
                var constructor = new BodyBuilder();
                constructor.HtmlBody = cuerpoHtml;
                if (!string.IsNullOrEmpty(cuerpoTexto))
                {
                    constructor.TextBody = cuerpoTexto;
                }
                mensaje.Body = constructor.ToMessageBody();

                // Configurar y enviar
                using var cliente = new SmtpClient();
                
                var host = _configuration["EmailSettings:SmtpHost"];
                var port = int.Parse(_configuration["EmailSettings:SmtpPort"] ?? "587");
                var enableSsl = bool.Parse(_configuration["EmailSettings:EnableSsl"] ?? "true");
                
                await cliente.ConnectAsync(host, port, enableSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None);
                
                var usuario = _configuration["EmailSettings:SmtpUser"];
                var password = _configuration["EmailSettings:SmtpPass"];
                
                if (!string.IsNullOrEmpty(usuario) && !string.IsNullOrEmpty(password))
                {
                    await cliente.AuthenticateAsync(usuario, password);
                }
                
                await cliente.SendAsync(mensaje);
                await cliente.DisconnectAsync(true);
                
                _logger.LogInformation("📧 Correo enviado exitosamente a {Destinatario} con asunto: {Asunto}", destinatario, asunto);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al enviar correo a {Destinatario}: {Error}", destinatario, ex.Message);
                return false;
            }
        }

        public async Task<bool> EnviarCorreoFelicitacionAscensoAsync(string emailDocente, string nombreDocente, string nivelAnterior, string nivelNuevo)
        {
            var asunto = "🎉 ¡Felicitaciones! Su solicitud de ascenso ha sido aprobada";
            
            var cuerpoHtml = GenerarPlantillaFelicitacion(nombreDocente, nivelAnterior, nivelNuevo);
            var cuerpoTexto = $@"
¡Felicitaciones {nombreDocente}!

Su solicitud de ascenso ha sido APROBADA.

Detalles del ascenso:
• Nivel anterior: {nivelAnterior}
• Nivel nuevo: {nivelNuevo}
• Fecha de aprobación: {DateTime.Now:dd/MM/yyyy}

Este ascenso reconoce su dedicación, esfuerzo y excelencia académica. 
Sus contadores de obras, horas de capacitación e investigación se han reiniciado para el próximo nivel.

Atentamente,
Sistema de Gestión de Ascensos Docentes
Universidad Técnica de Ambato";

            return await EnviarCorreoAsync(emailDocente, asunto, cuerpoHtml, cuerpoTexto);
        }

        public async Task<bool> EnviarCorreoRechazoAscensoAsync(string emailDocente, string nombreDocente, string nivelSolicitado, string motivo)
        {
            var asunto = "📋 Información sobre su solicitud de ascenso";
            
            var cuerpoHtml = GenerarPlantillaRechazo(nombreDocente, nivelSolicitado, motivo);
            var cuerpoTexto = $@"
Estimado/a {nombreDocente},

Le informamos que su solicitud de ascenso a {nivelSolicitado} ha sido revisada.

Motivo de la decisión:
{motivo}

Le recomendamos revisar los requisitos y continuar trabajando en los aspectos mencionados para futuras solicitudes.

Atentamente,
Sistema de Gestión de Ascensos Docentes
Universidad Técnica de Ambato";

            return await EnviarCorreoAsync(emailDocente, asunto, cuerpoHtml, cuerpoTexto);
        }

        private string GenerarPlantillaFelicitacion(string nombreDocente, string nivelAnterior, string nivelNuevo)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #8a1538; color: white; padding: 20px; text-align: center; border-radius: 8px 8px 0 0; }}
        .content {{ background-color: #f9f9f9; padding: 30px; border-radius: 0 0 8px 8px; }}
        .celebration {{ font-size: 48px; text-align: center; margin: 20px 0; }}
        .highlight {{ background-color: #d4edda; padding: 15px; border-left: 4px solid #28a745; margin: 20px 0; }}
        .footer {{ text-align: center; margin-top: 30px; color: #666; font-size: 12px; }}
        .nivel {{ display: inline-block; background-color: #8a1538; color: white; padding: 5px 10px; border-radius: 4px; margin: 0 5px; }}
        .arrow {{ font-size: 24px; color: #28a745; margin: 0 10px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>🎓 Universidad Técnica de Ambato</h1>
            <h2>Sistema de Gestión de Ascensos Docentes</h2>
        </div>
        <div class='content'>
            <div class='celebration'>🎉 ¡FELICITACIONES! 🎉</div>
            
            <h2>Estimado/a {nombreDocente},</h2>
            
            <p>Nos complace informarle que su <strong>solicitud de ascenso ha sido APROBADA</strong>.</p>
            
            <div class='highlight'>
                <h3>🚀 Detalles de su ascenso:</h3>
                <p style='text-align: center; font-size: 18px;'>
                    <span class='nivel'>{nivelAnterior}</span>
                    <span class='arrow'>➡️</span>
                    <span class='nivel'>{nivelNuevo}</span>
                </p>
                <p><strong>📅 Fecha de aprobación:</strong> {DateTime.Now:dd/MM/yyyy}</p>
            </div>
            
            <p>Este ascenso es el reconocimiento a su <strong>dedicación, esfuerzo y excelencia académica</strong>. Su compromiso con la educación y la investigación ha sido fundamental para lograr este importante avance en su carrera profesional.</p>
            
            <h3>📋 Información importante:</h3>
            <ul>
                <li>Sus contadores de obras académicas, horas de capacitación y meses de investigación se han <strong>reiniciado automáticamente</strong></li>
                <li>Los nuevos cálculos para el siguiente ascenso comenzarán desde la fecha de esta promoción</li>
                <li>Su nuevo nivel académico ya está actualizado en el sistema</li>
            </ul>
            
            <p>¡Continúe con su excelente trabajo y muchas felicitaciones por este logro!</p>
            
            <div class='footer'>
                <p>Este es un correo automático del Sistema de Gestión de Ascensos Docentes<br>
                Universidad Técnica de Ambato - {DateTime.Now.Year}</p>
            </div>
        </div>
    </div>
</body>
</html>";
        }

        private string GenerarPlantillaRechazo(string nombreDocente, string nivelSolicitado, string motivo)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #8a1538; color: white; padding: 20px; text-align: center; border-radius: 8px 8px 0 0; }}
        .content {{ background-color: #f9f9f9; padding: 30px; border-radius: 0 0 8px 8px; }}
        .info-box {{ background-color: #fff3cd; padding: 15px; border-left: 4px solid #ffc107; margin: 20px 0; }}
        .footer {{ text-align: center; margin-top: 30px; color: #666; font-size: 12px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>🎓 Universidad Técnica de Ambato</h1>
            <h2>Sistema de Gestión de Ascensos Docentes</h2>
        </div>
        <div class='content'>
            <h2>Estimado/a {nombreDocente},</h2>
            
            <p>Le escribimos para informarle sobre el estado de su solicitud de ascenso a <strong>{nivelSolicitado}</strong>.</p>
            
            <div class='info-box'>
                <h3>📋 Observaciones de la revisión:</h3>
                <p>{motivo}</p>
            </div>
            
            <p>Le recomendamos revisar los requisitos establecidos y continuar trabajando en los aspectos mencionados. Esto le permitirá fortalecer su perfil académico para futuras solicitudes.</p>
            
            <p>Recuerde que puede consultar el estado de sus indicadores en cualquier momento a través del sistema y solicitar un nuevo ascenso cuando cumpla con todos los requisitos.</p>
            
            <p>Agradecemos su dedicación y compromiso con la excelencia académica.</p>
            
            <div class='footer'>
                <p>Este es un correo automático del Sistema de Gestión de Ascensos Docentes<br>
                Universidad Técnica de Ambato - {DateTime.Now.Year}</p>
            </div>
        </div>
    </div>
</body>
</html>";
        }
    }
}
