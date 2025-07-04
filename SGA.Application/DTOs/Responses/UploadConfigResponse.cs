namespace SGA.Application.DTOs.Responses
{
    public class UploadConfigResponse
    {
        public int MaxSizeBytes { get; set; }
        public int MaxSizeMB { get; set; }
        public string[] AllowedExtensions { get; set; } = Array.Empty<string>();
        public string[] AllowedMimeTypes { get; set; } = Array.Empty<string>();
        public string AcceptAttribute { get; set; } = string.Empty;
    }
}
