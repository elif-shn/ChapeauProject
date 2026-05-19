using Chapeau.Enums;
using Chapeau.Models;
using Chapeau.Repositories;
using Chapeau.Services;
using Chapeau.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Chapeau.Controllers
{
    public class MenuItemController : Controller
    {
        private readonly IMenuService _menuService;
        private readonly IMenuListService _menuListService;


        public MenuItemController(IMenuService menuService,IMenuListService menuListService)
        {
            _menuService = menuService;
            _menuListService = menuListService;
        }

        public ActionResult Index(MenuViewModel menuViewModel)
        {
            menuViewModel.Categories = _menuListService.GetCategoriesByMenu(menuViewModel.SelectedMenuId);
            bool categoryExists = false;

            foreach (var category in menuViewModel.Categories)
            {
                if (category == menuViewModel.SelectedCategory)
                {
                    categoryExists = true;
                }
            }

            if (!categoryExists)
            {
                menuViewModel.SelectedCategory = null;
            }

            menuViewModel.Menus = _menuListService.GetAllMenus();


            List<MenuItem> menuItems = _menuService.GetActiveItems(menuViewModel.SelectedMenuId, menuViewModel.SelectedCategory);


            menuViewModel.MenuItems = menuItems;

            return View(menuViewModel);
        }

    }
}
