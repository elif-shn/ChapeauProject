using Chapeau.Models;

namespace Chapeau.Services
{
    public interface IUserService
    {
        User? GetByUsernameAndPassword(string username, string password);
    }
}
