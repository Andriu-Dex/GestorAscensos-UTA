using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace SGA.Web.Shared
{
    /// <summary>
    /// Componente para mostrar fechas formateadas en zona horaria de Ecuador
    /// </summary>
    public partial class EcuadorDateDisplay : ComponentBase
    {
        [Inject] private IJSRuntime JSRuntime { get; set; } = default!;

        [Parameter] public DateTime? Date { get; set; }
        [Parameter] public string Format { get; set; } = "datetime"; // 'short', 'long', 'datetime', 'time'
        [Parameter] public string CssClass { get; set; } = "";
        [Parameter] public string DefaultText { get; set; } = "";

        private string _formattedDate = "";

        protected override async Task OnParametersSetAsync()
        {
            if (Date.HasValue)
            {
                try
                {
                    _formattedDate = await JSRuntime.InvokeAsync<string>(
                        "EcuadorDateHelper.formatEcuadorDate", 
                        Date.Value.ToString("O"), // ISO 8601 format
                        Format
                    );
                }
                catch
                {
                    // Fallback si JavaScript no está disponible
                    var ecuadorTimeZone = TimeZoneInfo.CreateCustomTimeZone(
                        "Ecuador Standard Time",
                        TimeSpan.FromHours(-5),
                        "Ecuador Standard Time",
                        "ECT"
                    );

                    var ecuadorDate = Date.Value.Kind == DateTimeKind.Utc 
                        ? TimeZoneInfo.ConvertTimeFromUtc(Date.Value, ecuadorTimeZone)
                        : Date.Value;

                    _formattedDate = Format switch
                    {
                        "short" => ecuadorDate.ToString("dd/MM/yyyy"),
                        "long" => ecuadorDate.ToString("dd 'de' MMMM 'de' yyyy", new System.Globalization.CultureInfo("es-ES")),
                        "time" => ecuadorDate.ToString("HH:mm"),
                        _ => ecuadorDate.ToString("dd/MM/yyyy HH:mm")
                    };
                }
            }
            else
            {
                _formattedDate = DefaultText;
            }
        }
    }
}
