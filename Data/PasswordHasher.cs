using System;
using System.Security.Cryptography;

namespace GyanPath.Data
{
    public static class PasswordHasher
    {
        private const int SaltSize = 16; // 128 bits
        private const int KeySize = 32;  // 256 bits
        private const int Iterations = 10000;

        public static void HashPassword(string password, out string hash, out string salt)
        {
            byte[] saltBytes = new byte[SaltSize];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }

            using (var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, Iterations, HashAlgorithmName.SHA256))
            {
                byte[] hashBytes = pbkdf2.GetBytes(KeySize);
                hash = Convert.ToBase64String(hashBytes);
                salt = Convert.ToBase64String(saltBytes);
            }
        }

        public static bool VerifyPassword(string password, string storedHash, string storedSalt)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(storedHash) || string.IsNullOrEmpty(storedSalt))
                return false;

            try
            {
                byte[] saltBytes = Convert.FromBase64String(storedSalt);
                byte[] expectedHashBytes = Convert.FromBase64String(storedHash);

                using (var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, Iterations, HashAlgorithmName.SHA256))
                {
                    byte[] actualHashBytes = pbkdf2.GetBytes(KeySize);

                    if (actualHashBytes.Length != expectedHashBytes.Length)
                        return false;

                    int diff = 0;
                    for (int i = 0; i < actualHashBytes.Length; i++)
                    {
                        diff |= actualHashBytes[i] ^ expectedHashBytes[i];
                    }

                    return diff == 0;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
