using Ech.Executive.Authentication.Model;

namespace Ech.Executive.Authentication.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserModel>> GetAllUsers(bool? isActive);
        Task<UserModel?> GetUserById(int id);
    }
}
