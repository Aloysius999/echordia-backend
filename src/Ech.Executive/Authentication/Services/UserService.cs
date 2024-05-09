using Ech.Executive.Database;
using Ech.Executive.Authentication.Model;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Configuration;

namespace Ech.Executive.Authentication.Services
{
    public class UserService : IUserService
    {
        private readonly IAuthService _authService;

        public UserService(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<IEnumerable<UserModel>> GetAllUsers(bool? isActive)
        {
            var users = await _authService.GetAll();
            List<UserModel> lst = new List<UserModel>();
            foreach(User user in users)
            {
                lst.Add(Map(user));
            }
            return lst;
        }

        public async Task<UserModel?> GetUserById(int id)
        {
            var user = await _authService.GetById(id);
            UserModel res = Map(user);
            return res;
        }

        private UserModel Map(User user)
        {
            string json = JsonConvert.SerializeObject(user);
            UserModel res = JsonConvert.DeserializeObject<UserModel>(json);
            return res;
        }
    }
}
