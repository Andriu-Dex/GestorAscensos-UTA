using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace HashGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== GENERADOR DE HASHES - SISTEMA SGA ===");
            Console.WriteLine();

            while (true)
            {
                Console.WriteLine("Seleccione el tipo de hash a generar:");
                Console.WriteLine("1. Hash de contraseña (SHA256 + Base64)");
                Console.WriteLine("2. Hash de archivo (SHA256 + Hexadecimal)");
                Console.WriteLine("3. Hash de texto personalizado");
                Console.WriteLine("4. Salir");
                Console.Write("Opción: ");

                var option = Console.ReadLine();

                switch (option)
                {
                    case "1":
                        GeneratePasswordHash();
                        break;
                    case "2":
                        GenerateFileHash();
                        break;
                    case "3":
                        GenerateCustomTextHash();
                        break;
                    case "4":
                        return;
                    default:
                        Console.WriteLine("Opción inválida. Intente nuevamente.");
                        break;
                }

                Console.WriteLine();
                Console.WriteLine("Presione cualquier tecla para continuar...");
                Console.ReadKey();
                Console.Clear();
            }
        }

        // Método idéntico al usado en AuthController.cs
        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        // Método idéntico al usado en CryptoService.cs para archivos
        private static string GenerateFileHashFromBytes(byte[] fileBytes)
        {
            using var sha256 = SHA256.Create();
            var hashBytes = sha256.ComputeHash(fileBytes);
            
            var sb = new StringBuilder();
            foreach (var b in hashBytes)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }

        private static void GeneratePasswordHash()
        {
            Console.WriteLine("\n=== HASH DE CONTRASEÑA ===");
            Console.Write("Ingrese la contraseña: ");
            string password = Console.ReadLine() ?? "";

            if (string.IsNullOrEmpty(password))
            {
                Console.WriteLine("Contraseña no puede estar vacía.");
                return;
            }

            string hash = HashPassword(password);
            
            Console.WriteLine();
            Console.WriteLine($"Contraseña: {password}");
            Console.WriteLine($"Hash (SHA256 + Base64): {hash}");
            Console.WriteLine();
            Console.WriteLine("Este hash se puede usar directamente en la tabla Docentes:");
            Console.WriteLine($"PasswordHash = '{hash}'");
        }

        private static void GenerateFileHash()
        {
            Console.WriteLine("\n=== HASH DE ARCHIVO ===");
            Console.Write("Ingrese la ruta completa del archivo: ");
            string filePath = Console.ReadLine() ?? "";

            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                Console.WriteLine("Archivo no existe o ruta inválida.");
                return;
            }

            try
            {
                byte[] fileBytes = File.ReadAllBytes(filePath);
                string hash = GenerateFileHashFromBytes(fileBytes);
                
                Console.WriteLine();
                Console.WriteLine($"Archivo: {Path.GetFileName(filePath)}");
                Console.WriteLine($"Tamaño: {fileBytes.Length:N0} bytes");
                Console.WriteLine($"Hash (SHA256 + Hex): {hash}");
                Console.WriteLine();
                Console.WriteLine("Este hash se puede usar en la tabla Documentos:");
                Console.WriteLine($"HashSHA256 = '{hash}'");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al procesar el archivo: {ex.Message}");
            }
        }

        private static void GenerateCustomTextHash()
        {
            Console.WriteLine("\n=== HASH DE TEXTO PERSONALIZADO ===");
            Console.Write("Ingrese el texto a hashear: ");
            string text = Console.ReadLine() ?? "";

            if (string.IsNullOrEmpty(text))
            {
                Console.WriteLine("Texto no puede estar vacío.");
                return;
            }

            // Generar ambos tipos de hash
            string passwordStyleHash = HashPassword(text);
            byte[] textBytes = Encoding.UTF8.GetBytes(text);
            string fileStyleHash = GenerateFileHashFromBytes(textBytes);

            Console.WriteLine();
            Console.WriteLine($"Texto: {text}");
            Console.WriteLine($"Hash estilo contraseña (Base64): {passwordStyleHash}");
            Console.WriteLine($"Hash estilo archivo (Hexadecimal): {fileStyleHash}");
        }

        // Método adicional para verificar contraseñas
        public static bool VerifyPasswordHash(string password, string storedHash)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            var computedHash = Convert.ToBase64String(hash);
            return computedHash == storedHash;
        }
    }
}
