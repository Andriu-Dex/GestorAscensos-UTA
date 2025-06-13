using System;
using System.Globalization;
using System.Linq;
using System.Text;

namespace SGA.Domain.Utilities
{
    public static class EmailNormalizer
    {
        /// <summary>
        /// Genera un correo electrónico institucional en el formato: inicial_del_nombre + apellido + @uta.edu.ec
        /// Ejemplo: "Daniel Jerez" -> "djerez@uta.edu.ec"
        /// </summary>
        /// <param name="nombres">Los nombres completos del docente</param>
        /// <param name="apellidos">Los apellidos completos del docente</param>
        /// <returns>El correo electrónico institucional normalizado</returns>
        public static string GenerarCorreoInstitucional(string nombres, string apellidos)
        {
            if (string.IsNullOrWhiteSpace(nombres) || string.IsNullOrWhiteSpace(apellidos))
            {
                throw new ArgumentException("Los nombres y apellidos son requeridos para generar el correo institucional.");
            }

            // Normalizar texto: quitar acentos, espacios y convertir a minúsculas
            var nombresNormalizados = NormalizarTexto(nombres.Trim());
            var apellidosNormalizados = NormalizarTexto(apellidos.Trim());

            // Obtener la primera inicial del primer nombre
            var primerNombre = nombresNormalizados.Split(' ')[0];
            var inicialNombre = primerNombre.Substring(0, 1).ToLower();

            // Obtener el primer apellido
            var primerApellido = apellidosNormalizados.Split(' ')[0].ToLower();

            // Generar el correo en el formato: inicial + apellido + @uta.edu.ec
            return $"{inicialNombre}{primerApellido}@uta.edu.ec";
        }

        /// <summary>
        /// Normaliza un texto removiendo acentos, caracteres especiales y convirtiendo a ASCII
        /// </summary>
        /// <param name="texto">El texto a normalizar</param>
        /// <returns>El texto normalizado</returns>
        private static string NormalizarTexto(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return string.Empty;

            // Remover acentos y caracteres especiales
            var textoNormalizado = texto.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (char c in textoNormalizado)
            {
                var categoriaUnicode = CharUnicodeInfo.GetUnicodeCategory(c);
                if (categoriaUnicode != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        /// <summary>
        /// Valida si un correo electrónico sigue el formato institucional correcto
        /// </summary>
        /// <param name="email">El correo a validar</param>
        /// <returns>True si el correo sigue el formato institucional</returns>
        public static bool EsCorreoInstitucionalValido(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            // Verificar que termine con @uta.edu.ec
            if (!email.EndsWith("@uta.edu.ec", StringComparison.OrdinalIgnoreCase))
                return false;

            // Verificar que la parte local tenga el formato correcto (inicial + apellido)
            var parteLocal = email.Substring(0, email.IndexOf('@'));
            
            // Debe tener al menos 2 caracteres (1 inicial + 1 char del apellido mínimo)
            return parteLocal.Length >= 2 && parteLocal.All(char.IsLetter);
        }
    }
}
