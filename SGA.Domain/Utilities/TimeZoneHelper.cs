using System;

namespace SGA.Domain.Utilities
{
    /// <summary>
    /// Utilidades para manejo de zonas horarias del sistema
    /// Configurado para la zona horaria de Ecuador (UTC-5)
    /// </summary>
    public static class TimeZoneHelper
    {
        /// <summary>
        /// Zona horaria de Ecuador (UTC-5)
        /// </summary>
        public static readonly TimeZoneInfo EcuadorTimeZone = TimeZoneInfo.CreateCustomTimeZone(
            "Ecuador Standard Time",
            TimeSpan.FromHours(-5),
            "Ecuador Standard Time",
            "ECT"
        );

        /// <summary>
        /// Obtiene la fecha y hora actual en la zona horaria de Ecuador
        /// </summary>
        /// <returns>DateTime actual en zona horaria de Ecuador</returns>
        public static DateTime GetEcuadorDateTime()
        {
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, EcuadorTimeZone);
        }

        /// <summary>
        /// Convierte una fecha UTC a la zona horaria de Ecuador
        /// </summary>
        /// <param name="utcDateTime">Fecha en UTC</param>
        /// <returns>Fecha convertida a zona horaria de Ecuador</returns>
        public static DateTime ConvertToEcuadorTime(DateTime utcDateTime)
        {
            if (utcDateTime.Kind == DateTimeKind.Utc)
            {
                return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, EcuadorTimeZone);
            }
            
            // Si no es UTC, asumimos que ya está en hora local
            return utcDateTime;
        }

        /// <summary>
        /// Convierte una fecha de Ecuador a UTC para almacenamiento
        /// </summary>
        /// <param name="ecuadorDateTime">Fecha en zona horaria de Ecuador</param>
        /// <returns>Fecha convertida a UTC</returns>
        public static DateTime ConvertToUtc(DateTime ecuadorDateTime)
        {
            return TimeZoneInfo.ConvertTimeToUtc(ecuadorDateTime, EcuadorTimeZone);
        }

        /// <summary>
        /// Formatea una fecha UTC para mostrar en zona horaria de Ecuador
        /// </summary>
        /// <param name="utcDateTime">Fecha en UTC</param>
        /// <param name="format">Formato de fecha (opcional)</param>
        /// <returns>Fecha formateada en zona horaria de Ecuador</returns>
        public static string FormatEcuadorTime(DateTime utcDateTime, string format = "dd/MM/yyyy HH:mm")
        {
            var ecuadorTime = ConvertToEcuadorTime(utcDateTime);
            return ecuadorTime.ToString(format);
        }

        /// <summary>
        /// Formatea una fecha UTC para mostrar en zona horaria de Ecuador con cultura específica
        /// </summary>
        /// <param name="utcDateTime">Fecha en UTC</param>
        /// <param name="format">Formato de fecha</param>
        /// <param name="culture">Cultura para el formato</param>
        /// <returns>Fecha formateada en zona horaria de Ecuador</returns>
        public static string FormatEcuadorTime(DateTime utcDateTime, string format, System.Globalization.CultureInfo culture)
        {
            var ecuadorTime = ConvertToEcuadorTime(utcDateTime);
            return ecuadorTime.ToString(format, culture);
        }

        /// <summary>
        /// Calcula la diferencia de tiempo entre ahora (Ecuador) y una fecha UTC
        /// </summary>
        /// <param name="utcDateTime">Fecha en UTC</param>
        /// <returns>TimeSpan con la diferencia</returns>
        public static TimeSpan GetTimeDifferenceFromNow(DateTime utcDateTime)
        {
            var ecuadorNow = GetEcuadorDateTime();
            var ecuadorDateTime = ConvertToEcuadorTime(utcDateTime);
            return ecuadorNow - ecuadorDateTime;
        }
    }
}
