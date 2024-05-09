using Ech.Executive.Authentication.Model;

namespace Ech.Executive.Authentication.Services
{
    public interface IAuthService
    {
        Task<AuthenticateResponse?> Authenticate(AuthenticateRequest model);
        Task<IEnumerable<User>> GetAll(bool? isActive = true);
        Task<User?> GetById(int id);
    }
}
