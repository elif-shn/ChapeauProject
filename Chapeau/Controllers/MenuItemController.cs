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
        private readonly IMenuRepository _menuRepository;

        public MenuItemController(
            IMenuItemRepository menuItemRepository,
            IMenuRepository menuRepository)
        {
            _menuItemRepository = menuItemRepository;
            _menuRepository = menuRepository;
        }
        public ActionResult Index(MenuFilterViewModel menuFilterViewModel)
        {
            List<MenuItemViewModel> menuViewModel = new List<MenuItemViewModel>();

            menuFilterViewModel.Categories = Enum.GetValues(typeof(Category)).Cast<Category>().ToList();

            menuFilterViewModel.Menus = _menuRepository.GetAllMenus();

            List<MenuItem> menuItems = _menuItemRepository.GetAllByFilter(menuFilterViewModel);

            foreach (var item in menuItems)
            {
                string status;

                if (item.Stock == 0)
                    status = "OUT OF STOCK";
                else if (item.Stock <= 10)
                    status = "ALMOST OUT OF STOCK";
                else
                    status = "IN STOCK";

                menuViewModel.Add(new MenuItemViewModel
                {
                    MenuItemName = item.MenuItemName,
                    MenuItemPrice = item.MenuItemPrice,
                    MenuId = item.MenuId,
                    Category = item.Category,
                    StockStatus = status
                });
            }

            menuFilterViewModel.MenuItems = menuViewModel;

            return View(menuFilterViewModel);
        }
    }
}
