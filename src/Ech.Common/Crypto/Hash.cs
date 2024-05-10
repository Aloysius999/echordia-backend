namespace Ech.Common.Crypto
{
    public class Hash
    {
        public static string HashPassword(string password)
        {
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(password, 10);
            return passwordHash;
        }

        public static bool Verify(string submittedPasswordHash, string passwordHash)
        {
            bool validPassword = BCrypt.Net.BCrypt.Verify(submittedPasswordHash, passwordHash);
            return validPassword;
        }
    }
}
