namespace SGA.Application.DTOs.Responses
{
    public class FileUploadResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? PhotoBase64 { get; set; }
        public FileUploadError? Error { get; set; }

        // Factory methods
        public static FileUploadResponse SuccessResponse(string message, string? photoBase64 = null)
        {
            return new FileUploadResponse
            {
                Success = true,
                Message = message,
                PhotoBase64 = photoBase64
            };
        }

        public static FileUploadResponse ErrorResponse(string message, FileUploadErrorType errorType)
        {
            return new FileUploadResponse
            {
                Success = false,
                Message = message,
                Error = new FileUploadError
                {
                    Type = errorType,
                    Details = message
                }
            };
        }
    }

    public class FileUploadError
    {
        public FileUploadErrorType Type { get; set; }
        public string Details { get; set; } = string.Empty;
    }

    public enum FileUploadErrorType
    {
        InvalidFile,
        FileTooLarge,
        InvalidFormat,
        ProcessingError,
        DatabaseError
    }
}
