using System;
using System.Threading.Tasks;
using SGA.Domain.Entities;
using SGA.Domain.Enums;
using SGA.Infrastructure.Data;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace SGA.Application.Services
{
    public interface ILogService
    {
        Task LogInformacionAsync(string accion, string entidad, int? docenteId = null, string? observaciones = null);
        Task LogErrorAsync(string accion, string entidad, Exception excepcion, int? docenteId = null);
        Task LogAdvertenciaAsync(string accion, string entidad, string mensaje, int? docenteId = null);
        Task LogAuditoriaAsync(string accion, string entidad, int? docenteId, object? valoresAnteriores, object? valoresNuevos, string? observaciones = null);
    }

    public class LogService : ILogService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<LogService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LogService(
            AppDbContext context,
            ILogger<LogService> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task LogInformacionAsync(string accion, string entidad, int? docenteId = null, string? observaciones = null)
        {
            await GuardarLog(accion, entidad, docenteId, observaciones, TipoLog.Informacion);
            _logger.LogInformation("[{Accion}][{Entidad}] {Observaciones}", accion, entidad, observaciones);
        }

        public async Task LogErrorAsync(string accion, string entidad, Exception excepcion, int? docenteId = null)
        {
            string observaciones = $"Error: {excepcion.Message}. {excepcion.StackTrace}";
            await GuardarLog(accion, entidad, docenteId, observaciones, TipoLog.Error);
            _logger.LogError(excepcion, "[{Accion}][{Entidad}] {Mensaje}", accion, entidad, excepcion.Message);
        }

        public async Task LogAdvertenciaAsync(string accion, string entidad, string mensaje, int? docenteId = null)
        {
            await GuardarLog(accion, entidad, docenteId, mensaje, TipoLog.Advertencia);
            _logger.LogWarning("[{Accion}][{Entidad}] {Mensaje}", accion, entidad, mensaje);
        }

        public async Task LogAuditoriaAsync(string accion, string entidad, int? docenteId, object? valoresAnteriores, object? valoresNuevos, string? observaciones = null)
        {
            var valoresAnteriorJson = valoresAnteriores != null 
                ? JsonSerializer.Serialize(valoresAnteriores)
                : null;
                
            var valoresNuevosJson = valoresNuevos != null 
                ? JsonSerializer.Serialize(valoresNuevos)
                : null;

            var log = new LogAuditoria
            {
                Accion = accion,
                Entidad = entidad,
                DocenteId = docenteId,
                Observaciones = observaciones,
                FechaRegistro = DateTime.UtcNow,
                DireccionIP = ObtenerIP(),
                UserAgent = ObtenerUserAgent(),
                ValoresAnteriores = valoresAnteriorJson,
                ValoresNuevos = valoresNuevosJson,
                TipoLog = TipoLog.Auditoria
            };

            await _context.LogsAuditoria.AddAsync(log);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("[AUDITORIA][{Accion}][{Entidad}] Docente: {DocenteId}, {Observaciones}", 
                accion, entidad, docenteId, observaciones);
        }

        private async Task GuardarLog(string accion, string entidad, int? docenteId, string? observaciones, TipoLog tipoLog)
        {
            var log = new LogAuditoria
            {
                Accion = accion,
                Entidad = entidad,
                DocenteId = docenteId,
                Observaciones = observaciones,
                FechaRegistro = DateTime.UtcNow,
                DireccionIP = ObtenerIP(),
                UserAgent = ObtenerUserAgent(),
                TipoLog = tipoLog
            };

            await _context.LogsAuditoria.AddAsync(log);
            await _context.SaveChangesAsync();
        }

        private string ObtenerIP()
        {
            if (_httpContextAccessor.HttpContext == null)
                return "No disponible";

            var ip = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress?.ToString();
            return string.IsNullOrEmpty(ip) ? "No disponible" : ip;
        }

        private string ObtenerUserAgent()
        {
            if (_httpContextAccessor.HttpContext == null)
                return "No disponible";

            var userAgent = _httpContextAccessor.HttpContext.Request.Headers["User-Agent"].ToString();
            return string.IsNullOrEmpty(userAgent) ? "No disponible" : userAgent;
        }
    }
}
