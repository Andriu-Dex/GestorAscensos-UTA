using System.ComponentModel;
using System.Reflection;
using SGA.Web.Models.Enums;

namespace SGA.Web.Extensions;

/// <summary>
/// Extensiones para trabajar con enums
/// </summary>
public static class EnumExtensions
{
    /// <summary>
    /// Obtiene la descripción de un enum usando el atributo Description
    /// </summary>
    /// <param name="value">Valor del enum</param>
    /// <returns>Descripción del enum</returns>
    public static string GetDescription(this Enum value)
    {
        FieldInfo? field = value.GetType().GetField(value.ToString());
        if (field != null)
        {
            DescriptionAttribute? attribute = field.GetCustomAttribute<DescriptionAttribute>();
            if (attribute != null)
            {
                return attribute.Description;
            }
        }
        return value.ToString();
    }

    /// <summary>
    /// Valida si un nivel de ascenso es válido (secuencial)
    /// </summary>
    /// <param name="nivelActual">Nivel actual del docente</param>
    /// <param name="nivelSolicitado">Nivel solicitado para ascenso</param>
    /// <returns>True si el ascenso es válido</returns>
    public static bool EsAscensoValido(this NivelTitular nivelActual, NivelTitular nivelSolicitado)
    {
        return (int)nivelSolicitado == (int)nivelActual + 1;
    }

    /// <summary>
    /// Obtiene el siguiente nivel de titular
    /// </summary>
    /// <param name="nivelActual">Nivel actual</param>
    /// <returns>Siguiente nivel si existe, null si ya está en el máximo</returns>
    public static NivelTitular? GetSiguienteNivel(this NivelTitular nivelActual)
    {
        if (nivelActual == NivelTitular.Titular5)
            return null;

        return (NivelTitular)((int)nivelActual + 1);
    }

    /// <summary>
    /// Verifica si es el nivel máximo
    /// </summary>
    /// <param name="nivel">Nivel a verificar</param>
    /// <returns>True si es el nivel máximo</returns>
    public static bool EsNivelMaximo(this NivelTitular nivel)
    {
        return nivel == NivelTitular.Titular5;
    }
}
