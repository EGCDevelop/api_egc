using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

namespace api_egc.Utils
{
    public class PasswordHasher
    {
        private const int SaltSize = 16;

        private const int HashSize = 32;

        private const int Iterations = 10000;


        public static string HashPassword(string password)
        {
            // Generar un salt aleatorio
            byte[] salt = new byte[SaltSize];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            // Generar el hash
            byte[] hash = KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA512,
                iterationCount: Iterations,
                numBytesRequested: HashSize);

            // Combinar salt y hash
            byte[] hashBytes = new byte[SaltSize + HashSize];
            Array.Copy(salt, 0, hashBytes, 0, SaltSize);
            Array.Copy(hash, 0, hashBytes, SaltSize, HashSize);

            // Convertir a base64 para almacenamiento
            return Convert.ToBase64String(hashBytes);
        }

        public static bool VerifyPassword(string password, string hashedPassword)
        {
            // Convertir de base64 a bytes
            byte[] hashBytes = Convert.FromBase64String(hashedPassword);

            // Extraer el salt
            byte[] salt = new byte[SaltSize];
            Array.Copy(hashBytes, 0, salt, 0, SaltSize);

            // Extraer el hash almacenado
            byte[] storedHash = new byte[HashSize];
            Array.Copy(hashBytes, SaltSize, storedHash, 0, HashSize);

            // Generar hash de la contraseña proporcionada
            byte[] computedHash = KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA512,
                iterationCount: Iterations,
                numBytesRequested: HashSize);

            // Comparar los hashes
            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != storedHash[i]) return false;
            }

            return true;
        }
    }
}
