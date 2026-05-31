using Chapeau.Models;

namespace Chapeau.Services.Interfaces
{
    public interface IUserService
    {
        User? GetByUsernameAndPassword(string username, string password);
    }
}
