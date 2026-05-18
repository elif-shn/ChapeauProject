using Chapeau.Models;
using Chapeau.Repositories;
using Chapeau.ViewModels;
namespace Chapeau.Services
{
    public class MenuService : IMenuService
    {
        private IMenuRepository _menuRepository;

        public MenuService(IMenuRepository menuRepository)
        {
            _menuRepository = menuRepository;
        }
        public List<MenuItem> GetAllByFilter(MenuViewModel menuItems)
        {
            return _menuRepository.GetAllByFilter(menuItems);
        }

        /*public List<MenuItem> GetAllMenuItems()
        {
            return _menuRepository.GetAll();
        }

        public List<MenuItem> GetFilteredMenuItems(int menuId, int category)
        {
            if (menuId == 0 && category == 0)
                return _menuRepository.GetAll();
            return _menuRepository.GetByFilter(menuId, category);
        }*/

        public MenuItem GetMenuItemById(int id)
        {
            return _menuRepository.GetById(id);
        }

        public void AddMenuItem(MenuItem item)
        {
            _menuRepository.Add(item);
        }

        public void UpdateMenuItem(MenuItem item)
        {
            _menuRepository.Update(item);
        }

        public void ActivateMenuItem(int id)
        {
            _menuRepository.SetActive(id, true);
        }

        public void DeactivateMenuItem(int id)
        {
            _menuRepository.SetActive(id, false);
        }
    }
}