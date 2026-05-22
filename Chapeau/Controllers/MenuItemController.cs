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


        public MenuItemController(IMenuService menuService)
        {
            _menuService = menuService;
        }

        public IActionResult Index(Card? selectedCard, Category? selectedCategory)
        {
            try
            {
                List<Category> categories = _menuService.GetCategoriesByCard(_menuService.GetActiveItems(selectedCard, null), selectedCard);

                ViewBag.Categories = categories;

                if (selectedCategory != null &&
                    !categories.Contains(selectedCategory.Value))
                {
                    selectedCategory = null;
                }
                var menuItems = _menuService.GetActiveItems(selectedCard, selectedCategory);
                var viewModel = new MenuDisplayViewModel
                {
                    Menu = menuItems,
                    SelectedCard = selectedCard,
                    SelectedCategory = selectedCategory
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = " " + ex.Message;
                return View("Index");
            }
           
        }

    }
}
