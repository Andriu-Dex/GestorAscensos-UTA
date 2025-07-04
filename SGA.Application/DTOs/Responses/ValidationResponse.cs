namespace SGA.Application.DTOs.Responses
{
    public class ValidationResponse
    {
        public bool IsValid { get; set; }
        public string Message { get; set; } = string.Empty;
        public string[] Errors { get; set; } = Array.Empty<string>();

        public static ValidationResponse Valid(string message = "Validación exitosa")
        {
            return new ValidationResponse { IsValid = true, Message = message };
        }

        public static ValidationResponse Invalid(string message, params string[] errors)
        {
            return new ValidationResponse
            {
                IsValid = false,
                Message = message,
                Errors = errors
            };
        }
    }
}
