using Ech.Executive.Authentication.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Ech.Executive.Database;
using Ech.Executive.Settings;
using Ech.Common.Crypto;
using Microsoft.AspNetCore.Identity;

namespace Ech.Executive.Authentication.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppSettings _appSettings;
        private readonly MySQLDbContext _db;

        public AuthService(IOptions<AppSettings> appSettings, MySQLDbContext db)
        {
            _appSettings = appSettings.Value;
            _db = db;
        }

        public async Task<AuthenticateResponse?> Authenticate(AuthenticateRequest model)
        {
            var user = await _db.Users.SingleOrDefaultAsync(x => x.email == model.Email);

            // return null if user not found
            if (user == null) return null;
            if (user.roleId == User.Role.User) return null;

            // verify password hash
            var res = Hash.Verify(model.Password, user.hashedPassword);
            if (res == false) return null;

            // authentication successful so generate jwt token
            var token = await generateJwtToken(user);

            return new AuthenticateResponse(user, token);
        }

        public async Task<IEnumerable<User>> GetAll(bool? isActive = true)
        {
            return await _db.Users.Where(x => x.isActive == isActive).ToListAsync();
        }

        public async Task<User?> GetById(int id)
        {
            return await _db.Users.FirstOrDefaultAsync(x => x.id == id);
        }

        // helper methods
        private async Task<string> generateJwtToken(User user)
        {
            //Generate token that is valid for 7 days
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = await Task.Run(() =>
            {

                var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[] { new Claim("id", user.id.ToString()) }),
                    Expires = DateTime.UtcNow.AddDays(7),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                return tokenHandler.CreateToken(tokenDescriptor);
            });

            return tokenHandler.WriteToken(token);
        }
    }
}
