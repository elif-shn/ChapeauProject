using Chapeau.Enums;
using Chapeau.Models;
using Chapeau.Repositories;
using Chapeau.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Chapeau.Controllers
{
    public class MenuItemController : Controller
    {
        private readonly IMenuItemRepository _menuItemRepository;

        public MenuItemController(IMenuItemRepository menuItemRepository)
        {
            _menuItemRepository = menuItemRepository;
        }
        public ActionResult Index(int? selectedMenuId, Category? selectedCategory)
        {
            List<MenuItemViewModel> menuViewModel = new List<MenuItemViewModel>();

            MenuFilterViewModel menuFilterViewModel = new MenuFilterViewModel
            {
                SelectedMenuId = selectedMenuId,
                SelectedCategory = selectedCategory,
                Categories = Enum.GetValues(typeof(Category)).Cast<Category>().ToList()
            };

            List<MenuItem> menuItems = _menuItemRepository.GetAllByFilter(menuFilterViewModel);

            foreach (var item in menuItems)
            {
                menuViewModel.Add(new MenuItemViewModel
                {
                    MenuItemName = item.MenuItemName,
                    MenuItemPrice = item.MenuItemPrice
                });
            }

            menuFilterViewModel.MenuItems = menuViewModel;

            return View(menuFilterViewModel);
        }
    }
}
