namespace NewHorizon.Utils
{
    using System;
    using System.Security.Cryptography;

    public static class PasswordHashingUtil
    {
        private const int SaltSize = 16; // 128 bit salt
        private const int KeySize = 32; // 256 bit key

        public static (string, string) HashPassword(string password, string salt = null)
        {
            if (salt == null)
            {
                using (var randomNumberGenerator = RandomNumberGenerator.Create())
                {
                    var saltBytes = new byte[SaltSize];
                    randomNumberGenerator.GetBytes(saltBytes);
                    salt = Convert.ToBase64String(saltBytes);
                }
            }

            using (var keyDerivationFunction = new Rfc2898DeriveBytes(password, Convert.FromBase64String(salt), 10000))
            {
                var keyBytes = keyDerivationFunction.GetBytes(KeySize);
                var hashedPassword = Convert.ToBase64String(keyBytes);
                return (hashedPassword, salt);
            }
        }

        public static bool VerifyPassword(string password, string hashedPassword, string salt)
        {
            var (expectedHashedPassword, _) = HashPassword(password, salt);
            return hashedPassword == expectedHashedPassword;
        }
    }
}
