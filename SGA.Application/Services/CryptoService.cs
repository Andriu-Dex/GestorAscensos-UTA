using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace SGA.Application.Services
{
    public interface ICryptoService
    {
        byte[] EncryptData(byte[] data, string purpose);
        byte[] DecryptData(byte[] encryptedData, string purpose);
        string GenerateHash(Stream stream);
    }

    public class CryptoService : ICryptoService
    {
        private readonly byte[] _entropy;

        public CryptoService()
        {
            // Entropy para DPAPI - en producción este valor debería estar en configuración segura
            _entropy = Encoding.UTF8.GetBytes("SGA_UTA_Sistema_Ascensos_Secure_Encryption");
        }

        public byte[] EncryptData(byte[] data, string purpose)
        {
            try
            {
                // Usar DPAPI para cifrar los datos
                return ProtectedData.Protect(data, _entropy, DataProtectionScope.CurrentUser);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al cifrar datos ({purpose}): {ex.Message}", ex);
            }
        }

        public byte[] DecryptData(byte[] encryptedData, string purpose)
        {
            try
            {
                // Usar DPAPI para descifrar los datos
                return ProtectedData.Unprotect(encryptedData, _entropy, DataProtectionScope.CurrentUser);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al descifrar datos ({purpose}): {ex.Message}", ex);
            }
        }

        public string GenerateHash(Stream stream)
        {
            using (var sha256 = SHA256.Create())
            {
                // Resetear la posición del stream para leerlo desde el principio
                if (stream.CanSeek)
                {
                    stream.Position = 0;
                }

                // Calcular el hash SHA-256
                var hashBytes = sha256.ComputeHash(stream);
                
                // Convertir a string hexadecimal
                var sb = new StringBuilder();
                foreach (var b in hashBytes)
                {
                    sb.Append(b.ToString("x2"));
                }

                // Resetear nuevamente la posición del stream para futuros usos
                if (stream.CanSeek)
                {
                    stream.Position = 0;
                }
                
                return sb.ToString();
            }
        }
    }
}
