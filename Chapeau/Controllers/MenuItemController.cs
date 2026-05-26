using Chapeau.Enums;
using Chapeau.Models;
using Chapeau.Services.Interfaces;
using Chapeau.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Chapeau.Controllers
{
    public class MenuItemController : Controller
    {
        private readonly IMenuService _menuService;


        public MenuItemController(IMenuService menuService)
        {
            _menuService = menuService;
        }

        public IActionResult Index(Card? selectedCard, Category? selectedCategory)
        {
            try
            {
                return View(new MenuDisplayViewModel
                {
                    Menu = _menuService.GetMenuDisplay(selectedCard, selectedCategory).menus,
                    Categories = _menuService.GetMenuDisplay(selectedCard, selectedCategory).categories,
                    SelectedCard = selectedCard,
                    SelectedCategory = selectedCategory,
                });
            }
            catch (Exception ex)
            {
                return View(new MenuDisplayViewModel
                {
                    Menu = new List<Menu>(),
                    Categories = new List<Category>()
                });
            }

        }

    }
}
