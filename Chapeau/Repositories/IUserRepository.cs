using Chapeau.Models;

namespace Chapeau.Repositories
{
    public interface IUserRepository
    {
        User? GetByUsernameAndPassword(string username, string password);
        
    }

}
