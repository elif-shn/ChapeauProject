using Chapeau.Enums;
using Chapeau.Models;
using Chapeau.Repositories;
namespace Chapeau.Services
{
    public class MenuService : IMenuService
    {
        private IMenuRepository _menuRepository;

        public MenuService(IMenuRepository menuRepository)
        {
            _menuRepository = menuRepository;
        }
        public List<Menu> GetAllByFilter(Card? card, Category? category)
        {
            try
            {
                return _menuRepository.GetAllByFilter(card, category);
            }
            catch
            {
                throw;
            }
            
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

        public List<Menu> GetActiveItems(Card? card, Category? category)
        {
            try
            {

                List<Menu> menus = _menuRepository.GetAllByFilter(card, category);

                List<Menu> filteredMenus = new List<Menu>();

                foreach (var menu in menus)
                {
                    Menu newMenu = new Menu
                    {
                        MenuId = menu.MenuId,
                        Card = menu.Card,
                        Category = menu.Category,
                        MenuItems = new List<MenuItem>()
                    };

                    foreach (var item in menu.MenuItems)
                    {
                        if (item.IsActive)
                        {
                            newMenu.MenuItems.Add(item);
                        }
                    }

                    filteredMenus.Add(newMenu);
                }

                return filteredMenus;

            }
            catch
            {
                throw;
            }
        }
        public List<Category> GetCategoriesByCard(List<Menu> menus, Card? selectedCard)
        {
            try
            {
                List<Category> categories = new List<Category>();

                foreach (var menu in menus)
                {
                    if (selectedCard == null || menu.Card == selectedCard)
                    {
                        if (!categories.Contains(menu.Category))
                        {
                            categories.Add(menu.Category);
                        }
                    }
                }

                return categories;

            }
            catch
            {
                throw;
            }
            
        }
    }
}
