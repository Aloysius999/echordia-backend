using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
