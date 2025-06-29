using System;

namespace SGA.Api.Configuration
{
    public static class ConfigurationHelper
    {
        /// <summary>
        /// Obtiene una cadena de conexión desde variables de entorno o configuración
        /// </summary>
        /// <param name="configuration">Configuración de la aplicación</param>
        /// <param name="connectionName">Nombre de la conexión</param>
        /// <param name="envVariableName">Nombre de la variable de entorno</param>
        /// <returns>Cadena de conexión</returns>
        public static string GetConnectionString(IConfiguration configuration, string connectionName, string envVariableName)
        {
            var connectionString = Environment.GetEnvironmentVariable(envVariableName)
                                 ?? configuration.GetConnectionString(connectionName);
            
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException($"Connection string '{connectionName}' not found. Set {envVariableName} environment variable or configure in appsettings.json");
            }
            
            return connectionString;
        }

        /// <summary>
        /// Obtiene una configuración desde variables de entorno o configuración
        /// </summary>
        /// <param name="configuration">Configuración de la aplicación</param>
        /// <param name="configKey">Clave de configuración</param>
        /// <param name="envVariableName">Nombre de la variable de entorno</param>
        /// <param name="defaultValue">Valor por defecto si no se encuentra</param>
        /// <returns>Valor de configuración</returns>
        public static string GetConfigurationValue(IConfiguration configuration, string configKey, string envVariableName, string? defaultValue = null)
        {
            var value = Environment.GetEnvironmentVariable(envVariableName)
                       ?? configuration[configKey]
                       ?? defaultValue;
            
            if (string.IsNullOrEmpty(value))
            {
                throw new InvalidOperationException($"Configuration '{configKey}' not found. Set {envVariableName} environment variable or configure in appsettings.json");
            }
            
            return value;
        }

        /// <summary>
        /// Valida que todas las configuraciones requeridas estén presentes
        /// </summary>
        /// <param name="configuration">Configuración de la aplicación</param>
        public static void ValidateRequiredConfiguration(IConfiguration configuration)
        {
            var requiredConfigs = new[]
            {
                ("DefaultConnection", "SGA_DB_CONNECTION"),
                ("JWT:SecretKey", "SGA_JWT_SECRET_KEY")
            };

            foreach (var (configKey, envVar) in requiredConfigs)
            {
                var value = Environment.GetEnvironmentVariable(envVar) ?? configuration[configKey];
                if (string.IsNullOrEmpty(value))
                {
                    throw new InvalidOperationException($"Required configuration missing: {configKey} (environment variable: {envVar})");
                }
            }
        }
    }
}
