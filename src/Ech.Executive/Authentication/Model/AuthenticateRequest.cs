using System.ComponentModel;

namespace Ech.Executive.Authentication.Model
{
    public class AuthenticateRequest
    {
        [DefaultValue("System")]
        public required string Email { get; set; }

        [DefaultValue("System")]
        public required string HashedPassword { get; set; }
    }
}
