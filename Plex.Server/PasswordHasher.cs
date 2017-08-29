using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Security;

namespace Plex.Server
{
    public static class PasswordHasher
    {
        private const int SaltLength = 128;
        public static byte[] GenerateRandomSalt()
        {
            RandomNumberGenerator _generator = RandomNumberGenerator.Create();
            byte[] data = new byte[SaltLength];
            _generator.GetNonZeroBytes(data);
            return data;
        }

        public static string Hash(string password, byte[] salt)
        {
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(20);

            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);
            
            string savedPasswordHash = Convert.ToBase64String(hashBytes);
            return savedPasswordHash;
        }
    }
}
