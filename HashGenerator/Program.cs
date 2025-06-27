using System;
using BCrypt.Net;

namespace HashGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== VERIFICADOR DE HASH PARA USUARIO ADEX ===");
            Console.WriteLine();

            // Hash existente en la base de datos
            string existingHash = "$2a$11$Hc2UOGzXMPl9FBNmjPHCX.5pbqPGYdzeZZ2IpXiAGqmYwu0LSUC5G";
            
            // Probar diferentes contraseñas
            string[] passwords = { "@Andriu3Dex@", "123456", "admin123", "password", "Andriu3Dex", "@andriu3dex@" };
            
            foreach (string password in passwords)
            {
                bool isValid = BCrypt.Net.BCrypt.Verify(password, existingHash);
                Console.WriteLine($"Contraseña '{password}': {(isValid ? "✓ VÁLIDA" : "✗ INVÁLIDA")}");
            }
            
            Console.WriteLine();
            Console.WriteLine("=== GENERAR NUEVO HASH ===");
            
            // Generar hash para la contraseña "@Andriu3Dex@"
            string newPassword = "@Andriu3Dex@";
            string newHash = BCrypt.Net.BCrypt.HashPassword(newPassword);

            Console.WriteLine($"Nueva contraseña: {newPassword}");
            Console.WriteLine($"Nuevo hash generado: {newHash}");
            Console.WriteLine();

            // Verificar que el nuevo hash funciona
            bool isNewHashValid = BCrypt.Net.BCrypt.Verify(newPassword, newHash);
            Console.WriteLine($"Verificación del nuevo hash: {isNewHashValid}");
            Console.WriteLine();

            Console.WriteLine("=== SCRIPT SQL PARA ACTUALIZAR USUARIO ADEX ===");
            Console.WriteLine();
            Console.WriteLine($@"
-- Actualizar contraseña del usuario adex
UPDATE Usuarios 
SET PasswordHash = '{newHash}'
WHERE Email = 'adex@uta.edu.ec';
");
            
            Console.WriteLine("Presiona cualquier tecla para continuar...");
            Console.ReadKey();
        }
    }
}
