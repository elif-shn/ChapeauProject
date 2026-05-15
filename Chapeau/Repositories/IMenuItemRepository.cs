using Chapeau.Enums;
using Chapeau.Models;
using Chapeau.ViewModels;
namespace Chapeau.Repositories
{
    public interface IMenuItemRepository
    {
        List<MenuItem> GetAllByFilter(MenuFilterViewModel menuFilterViewModel);
    }
}
