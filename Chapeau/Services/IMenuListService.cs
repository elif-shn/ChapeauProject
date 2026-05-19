using Chapeau.Enums;
using Chapeau.Models;

namespace Chapeau.Services
{
    public interface IMenuListService
    {
        List<Menu> GetAllMenus();
        public List<Category> GetAllCategories();
        List<Category> GetCategoriesByMenu(int? menuId);


    }
}
