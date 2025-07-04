using MudBlazor;

namespace SGA.Web.Services
{
    public static class UTAThemeProvider
    {
        public static MudTheme Theme => new()
        {
            Palette = new PaletteLight()
            {
                Primary = "#8a1538",
                Secondary = "#741230",
                Tertiary = "#d4af37",
                Success = "#28a745",
                Info = "#17a2b8",
                Warning = "#ffc107",
                Error = "#dc3545",
                Dark = "#212529",
                TextPrimary = "#333333",
                TextSecondary = "#6c757d",
                Background = "#ffffff",
                Surface = "#ffffff",
                AppbarBackground = "#8a1538",
                AppbarText = "#ffffff",
                DrawerBackground = "#ffffff",
                DrawerText = "#333333"
            },
            PaletteDark = new PaletteDark()
            {
                Primary = "#8a1538",
                Secondary = "#741230",
                Tertiary = "#d4af37",
                Success = "#4caf50",
                Info = "#2196f3",
                Warning = "#ff9800",
                Error = "#f44336",
                Dark = "#1e1e1e",
                TextPrimary = "#ffffff",
                TextSecondary = "#aaaaaa",
                Background = "#1e1e1e",
                Surface = "#2d2d30",
                AppbarBackground = "#8a1538",
                AppbarText = "#ffffff",
                DrawerBackground = "#2d2d30",
                DrawerText = "#ffffff"
            },
            Typography = new Typography()
            {
                Default = new Default()
                {
                    FontFamily = new[] { "Poppins", "Helvetica Neue", "Helvetica", "Arial", "sans-serif" },
                    FontSize = "0.875rem",
                    FontWeight = 400,
                    LineHeight = 1.43,
                    LetterSpacing = "0.01071em"
                },
                H1 = new H1()
                {
                    FontFamily = new[] { "Poppins", "Helvetica Neue", "Helvetica", "Arial", "sans-serif" },
                    FontSize = "6rem",
                    FontWeight = 300,
                    LineHeight = 1.167,
                    LetterSpacing = "-0.01562em"
                },
                H2 = new H2()
                {
                    FontFamily = new[] { "Poppins", "Helvetica Neue", "Helvetica", "Arial", "sans-serif" },
                    FontSize = "3.75rem",
                    FontWeight = 300,
                    LineHeight = 1.2,
                    LetterSpacing = "-0.00833em"
                },
                H3 = new H3()
                {
                    FontFamily = new[] { "Poppins", "Helvetica Neue", "Helvetica", "Arial", "sans-serif" },
                    FontSize = "3rem",
                    FontWeight = 400,
                    LineHeight = 1.167,
                    LetterSpacing = "0em"
                },
                H4 = new H4()
                {
                    FontFamily = new[] { "Poppins", "Helvetica Neue", "Helvetica", "Arial", "sans-serif" },
                    FontSize = "2.125rem",
                    FontWeight = 400,
                    LineHeight = 1.235,
                    LetterSpacing = "0.00735em"
                },
                H5 = new H5()
                {
                    FontFamily = new[] { "Poppins", "Helvetica Neue", "Helvetica", "Arial", "sans-serif" },
                    FontSize = "1.5rem",
                    FontWeight = 400,
                    LineHeight = 1.334,
                    LetterSpacing = "0em"
                },
                H6 = new H6()
                {
                    FontFamily = new[] { "Poppins", "Helvetica Neue", "Helvetica", "Arial", "sans-serif" },
                    FontSize = "1.25rem",
                    FontWeight = 500,
                    LineHeight = 1.6,
                    LetterSpacing = "0.0075em"
                }
            },
            Shadows = new Shadow(),
            LayoutProperties = new LayoutProperties()
            {
                DrawerWidthLeft = "260px",
                DrawerWidthRight = "300px",
                AppbarHeight = "64px"
            },
            ZIndex = new ZIndex()
        };
    }
}
