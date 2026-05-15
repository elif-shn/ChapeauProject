using Chapeau.Models;

namespace Chapeau.Repositories
{
    public interface IMenuListRepository
    {
        List<Menu> GetAllMenus();
    }
}
