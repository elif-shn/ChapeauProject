using Chapeau.Enums;
using Chapeau.Models;
using Chapeau.Repositories;

namespace Chapeau.Services
{
    public class MenuListService : IMenuListService
    {
        private IMenuListRepository _menuListRepository;

        public MenuListService(IMenuListRepository menuListRepository)
        {
            _menuListRepository = menuListRepository;
        }
        public List<Menu> GetAllMenus()
        {
            return _menuListRepository.GetAllMenus();
        }
        public List<Category> GetAllCategories()
        {
            return Enum.GetValues(typeof(Category)).Cast<Category>().ToList();
        }
        public List<Category> GetCategoriesByMenu(int? menuId)
        {
            List<Category> categories = new List<Category>();

            if (menuId == null)
            {
                return GetAllCategories();
            }

            if (menuId == 1) // Lunch
            {
                categories.Add(Category.Starters);
                categories.Add(Category.Mains);
                categories.Add(Category.Desserts);
            }

            else if (menuId == 2) // Dinner
            {
                categories.Add(Category.Starters);
                categories.Add(Category.Entremets);
                categories.Add(Category.Mains);
                categories.Add(Category.Desserts);
            }

            else if (menuId == 3) // Drinks
            {
                categories.Add(Category.SoftDrinks);
                categories.Add(Category.Beers);
                categories.Add(Category.Wines);
                categories.Add(Category.SpiritDrinks);
                categories.Add(Category.CoffeeAndTea);
            }

            return categories;
        }
    }
}
