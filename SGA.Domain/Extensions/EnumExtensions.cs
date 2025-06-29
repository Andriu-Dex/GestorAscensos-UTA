using System.ComponentModel;
using System.Reflection;
using SGA.Domain.Enums;

namespace SGA.Domain.Extensions;

/// <summary>
/// Extensiones para trabajar con enums del dominio
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

    /// <summary>
    /// Verifica si una solicitud está en estado activo (no terminada)
    /// </summary>
    /// <param name="estado">Estado de la solicitud</param>
    /// <returns>True si está activa</returns>
    public static bool EsEstadoActivo(this EstadoSolicitud estado)
    {
        return estado == EstadoSolicitud.Pendiente || estado == EstadoSolicitud.EnProceso;
    }

    /// <summary>
    /// Verifica si una solicitud está terminada
    /// </summary>
    /// <param name="estado">Estado de la solicitud</param>
    /// <returns>True si está terminada</returns>
    public static bool EsEstadoTerminado(this EstadoSolicitud estado)
    {
        return estado == EstadoSolicitud.Aprobada || estado == EstadoSolicitud.Rechazada;
    }

    /// <summary>
    /// Obtiene todos los valores de un enum
    /// </summary>
    /// <typeparam name="T">Tipo de enum</typeparam>
    /// <returns>Array con todos los valores</returns>
    public static T[] GetAllValues<T>() where T : Enum
    {
        return (T[])Enum.GetValues(typeof(T));
    }

    /// <summary>
    /// Obtiene los requisitos mínimos para un nivel específico
    /// </summary>
    /// <param name="nivel">Nivel objetivo</param>
    /// <returns>Objeto con los requisitos</returns>
    public static RequisitosPorNivel GetRequisitos(this NivelTitular nivel)
    {
        return nivel switch
        {
            NivelTitular.Titular2 => new RequisitosPorNivel
            {
                AnosEnNivel = 4,
                ObrasMinimas = 1,
                PromedioEvaluacionMinimo = 75m,
                HorasCapacitacionMinimas = 96,
                MesesInvestigacionMinimos = 0
            },
            NivelTitular.Titular3 => new RequisitosPorNivel
            {
                AnosEnNivel = 4,
                ObrasMinimas = 2,
                PromedioEvaluacionMinimo = 75m,
                HorasCapacitacionMinimas = 96,
                MesesInvestigacionMinimos = 12
            },
            NivelTitular.Titular4 => new RequisitosPorNivel
            {
                AnosEnNivel = 4,
                ObrasMinimas = 3,
                PromedioEvaluacionMinimo = 75m,
                HorasCapacitacionMinimas = 128,
                MesesInvestigacionMinimos = 24
            },
            NivelTitular.Titular5 => new RequisitosPorNivel
            {
                AnosEnNivel = 4,
                ObrasMinimas = 5,
                PromedioEvaluacionMinimo = 75m,
                HorasCapacitacionMinimas = 160,
                MesesInvestigacionMinimos = 24
            },
            _ => throw new ArgumentException($"No se pueden obtener requisitos para el nivel {nivel}")
        };
    }
}

/// <summary>
/// Clase que representa los requisitos para un nivel específico
/// </summary>
public class RequisitosPorNivel
{
    public int AnosEnNivel { get; set; }
    public int ObrasMinimas { get; set; }
    public decimal PromedioEvaluacionMinimo { get; set; }
    public int HorasCapacitacionMinimas { get; set; }
    public int MesesInvestigacionMinimos { get; set; }
}
