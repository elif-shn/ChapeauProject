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
    }
}
