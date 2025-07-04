namespace SGA.Application.Constants
{
    public static class FileLimits
    {
        // Límites para fotos de perfil
        public static class ProfileImages
        {
            public const int MaxSizeBytes = 5 * 1024 * 1024; // 5MB
            public const int MaxSizeMB = 5;
            public const int MaxWidth = 800;
            public const int MaxHeight = 800;
            public const int JpegQuality = 85;
            
            public static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
            public static readonly string[] AllowedMimeTypes = { "image/jpeg", "image/jpg", "image/png", "image/webp" };
        }
    }
}
