using Chapeau.Models;
using Chapeau.Repositories.Interfaces;
using Chapeau.Services.Interfaces;

namespace Chapeau.Services
{
    public class UserServices : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserServices(IUserRepository userRepository)
        {
            this._userRepository = userRepository;
        }

        public User? GetByUsernameAndPassword(string username, string password)
        {
            return _userRepository.GetByUsernameAndPassword(username, password);
        }
    }
}
