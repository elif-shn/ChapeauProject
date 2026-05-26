using Chapeau.Models;

namespace Chapeau.Repositories.Interfaces
{
    public interface IUserRepository
    {
        User? GetByUsernameAndPassword(string username, string password);
        
    }

}
