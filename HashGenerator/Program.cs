using System;
using BCrypt.Net;

namespace HashGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== VERIFICADOR DE HASHES BCRYPT - SISTEMA SGA ===");
            Console.WriteLine();

            // Generar hash para la contraseña "123456"
            string password = "123456";
            string newHash = BCrypt.Net.BCrypt.HashPassword(password);

            Console.WriteLine($"Contraseña: {password}");
            Console.WriteLine($"Nuevo hash generado: {newHash}");
            Console.WriteLine();

            // Verificar que el nuevo hash funciona
            bool isNewHashValid = BCrypt.Net.BCrypt.Verify(password, newHash);
            Console.WriteLine($"Verificación del nuevo hash: {isNewHashValid}");
            Console.WriteLine();

            // Verificar con el hash actual de la base de datos
            string currentHash = "$2a$11$Ky2XR0gSQAYPag27Gfdg4O3h5aNnrBwU4JfVgLudVR/9U88NyEf4mu";
            Console.WriteLine($"Hash actual en BD: {currentHash}");
            
            bool isCurrentValid = BCrypt.Net.BCrypt.Verify(password, currentHash);
            Console.WriteLine($"Verificación con hash actual: {isCurrentValid}");
            Console.WriteLine();

            // Probar con diferentes contraseñas posibles
            string[] possiblePasswords = { "123456", "password", "sparedes", "steven123", "Sparedes123", "sparedes123" };
            
            Console.WriteLine("Probando diferentes contraseñas con el hash actual:");
            foreach (string pwd in possiblePasswords)
            {
                bool isValid = BCrypt.Net.BCrypt.Verify(pwd, currentHash);
                Console.WriteLine($"  {pwd}: {(isValid ? "✓ VÁLIDA" : "✗ Inválida")}");
            }

            Console.WriteLine();
            Console.WriteLine("=== SCRIPT SQL PARA ACTUALIZAR LA CONTRASEÑA ===");
            Console.WriteLine();
            Console.WriteLine($"UPDATE Usuarios SET PasswordHash = '{newHash}' WHERE Email = 'sparedes@uta.edu.ec';");
            Console.WriteLine();
            
            Console.WriteLine("Presiona cualquier tecla para continuar...");
            Console.ReadKey();
        }
    }
}
