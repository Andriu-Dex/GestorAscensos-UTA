using System;

class Program
{
    static void Main()
    {
        string hash = "$2a$11$Hc2UOGzXMPl9FBNmjPHCX.5pbqPGYdzeZZ2IpXiAGqmYwu0LSUC5G";
        
        // Posibles contraseñas a probar
        string[] passwords = {
            "@Andriu3Dex@",
            "123456",
            "password",
            "adex123",
            "Andriu3Dex",
            "@andriu3dex@"
        };
        
        foreach (string password in passwords)
        {
            bool isValid = BCrypt.Net.BCrypt.Verify(password, hash);
            Console.WriteLine($"Contraseña '{password}': {(isValid ? "CORRECTA" : "INCORRECTA")}");
        }
    }
}
