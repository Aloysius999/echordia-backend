using Ech.Schema.Executive;

namespace Ech.Executive.Authentication.Model
{
    public class AuthenticateResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Alias { get; set; }
        public string Token { get; set; }


        public AuthenticateResponse(User user, string token)
        {
            Id = user.id;
            Name = user.name;
            Email = user.email;
            Alias = user.alias;
            Token = token;
        }
    }
}
