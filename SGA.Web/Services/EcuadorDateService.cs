using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace SGA.Web.Services
{
    /// <summary>
    /// Servicio para manejo de fechas en zona horaria de Ecuador
    /// Utiliza JavaScript para garantizar formateo correcto
    /// </summary>
    public class EcuadorDateService
    {
        private readonly IJSRuntime _jsRuntime;

        public EcuadorDateService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        /// <summary>
        /// Formatea una fecha UTC para mostrarla en hora de Ecuador
        /// </summary>
        /// <param name="utcDate">Fecha en UTC</param>
        /// <param name="format">Formato: 'short', 'long', 'time', 'datetime'</param>
        /// <returns>Fecha formateada en hora de Ecuador</returns>
        public async Task<string> FormatEcuadorDateAsync(DateTime utcDate, string format = "datetime")
        {
            try
            {
                return await _jsRuntime.InvokeAsync<string>("EcuadorDateHelper.formatEcuadorDate", utcDate, format);
            }
            catch (Exception)
            {
                // Fallback en caso de error
                return FormatEcuadorDateFallback(utcDate, format);
            }
        }

        /// <summary>
        /// Obtiene el tiempo transcurrido desde una fecha UTC hasta ahora (en Ecuador)
        /// </summary>
        /// <param name="utcDate">Fecha UTC de referencia</param>
        /// <returns>Tiempo transcurrido formateado (ej: "2h", "5m", "Ahora")</returns>
        public async Task<string> GetTimeAgoAsync(DateTime utcDate)
        {
            try
            {
                return await _jsRuntime.InvokeAsync<string>("EcuadorDateHelper.getTimeAgo", utcDate);
            }
            catch (Exception)
            {
                // Fallback en caso de error
                return GetTimeAgoFallback(utcDate);
            }
        }

        /// <summary>
        /// Formatea una fecha de revisión usando JavaScript global
        /// </summary>
        /// <param name="fechaUtc">Fecha UTC</param>
        /// <param name="incluirHora">Si incluir la hora</param>
        /// <returns>Fecha formateada</returns>
        public async Task<string> FormatearFechaRevisionAsync(DateTime? fechaUtc, bool incluirHora = true)
        {
            if (!fechaUtc.HasValue) return "";

            try
            {
                return await _jsRuntime.InvokeAsync<string>("formatearFechaRevision", fechaUtc.Value, incluirHora);
            }
            catch (Exception)
            {
                // Fallback en caso de error
                return FormatEcuadorDateFallback(fechaUtc.Value, incluirHora ? "datetime" : "short");
            }
        }

        /// <summary>
        /// Método de respaldo para formatear fechas sin JavaScript
        /// </summary>
        private string FormatEcuadorDateFallback(DateTime utcDate, string format)
        {
            // Zona horaria de Ecuador (UTC-5)
            var ecuadorTimeZone = TimeZoneInfo.CreateCustomTimeZone(
                "Ecuador Standard Time",
                TimeSpan.FromHours(-5),
                "Ecuador Standard Time",
                "ECT"
            );

            var ecuadorDate = utcDate.Kind == DateTimeKind.Utc 
                ? TimeZoneInfo.ConvertTimeFromUtc(utcDate, ecuadorTimeZone)
                : utcDate;

            return format switch
            {
                "short" => ecuadorDate.ToString("dd/MM/yyyy"),
                "long" => ecuadorDate.ToString("dd 'de' MMMM 'de' yyyy", new System.Globalization.CultureInfo("es-ES")),
                "time" => ecuadorDate.ToString("HH:mm"),
                "datetime" => ecuadorDate.ToString("dd/MM/yyyy HH:mm"),
                _ => ecuadorDate.ToString("dd/MM/yyyy HH:mm")
            };
        }

        /// <summary>
        /// Método de respaldo para calcular tiempo transcurrido sin JavaScript
        /// </summary>
        private string GetTimeAgoFallback(DateTime utcDate)
        {
            // Zona horaria de Ecuador (UTC-5)
            var ecuadorTimeZone = TimeZoneInfo.CreateCustomTimeZone(
                "Ecuador Standard Time",
                TimeSpan.FromHours(-5),
                "Ecuador Standard Time",
                "ECT"
            );

            var ecuadorDate = utcDate.Kind == DateTimeKind.Utc 
                ? TimeZoneInfo.ConvertTimeFromUtc(utcDate, ecuadorTimeZone)
                : utcDate;
            
            var now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, ecuadorTimeZone);
            var diff = now - ecuadorDate;

            if (diff.TotalMinutes < 1) return "Ahora";
            if (diff.TotalMinutes < 60) return $"{(int)diff.TotalMinutes}m";
            if (diff.TotalHours < 24) return $"{(int)diff.TotalHours}h";
            if (diff.TotalDays < 7) return $"{(int)diff.TotalDays}d";
            
            return ecuadorDate.ToString("dd/MM");
        }
    }
}
