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