using Chapeau.Enums;
using Chapeau.Models;
using Chapeau.Repositories;
using Chapeau.Services.Interfaces;
using Chapeau.ViewModels;
using Microsoft.AspNetCore.Mvc;
namespace Chapeau.Controllers
{
    public class MenuManagementController : Controller
    {
        private readonly IMenuService _menuService;


        public MenuManagementController(IMenuService menuService)
        {
            _menuService = menuService;
        }

        public IActionResult Index(Card? selectedCard, Category? selectedCategory)
        {
            try
            {
                List<Menu> menu = _menuService.GetAllByFilter(selectedCard, selectedCategory);
                return View(new MenuManagementViewModel
                {
                    Menu = menu,
                    Categories = _menuService.GetCategoriesByCard(menu, selectedCard),
                    SelectedCard = selectedCard,
                    SelectedCategory = selectedCategory,
                });
            }

            catch (Exception ex)
            {
                return View(new MenuManagementViewModel
                {
                    Menu = new List<Menu>(),
                    Categories = new List<Category>()
                });
            }

        }

        public IActionResult Add()
        {
            return View(new MenuManagementViewModel { ItemToEdit = new MenuItem() });
        }

        [HttpPost]
        public IActionResult Add(MenuManagementViewModel model)
        {
            if (model.ItemToEdit != null && model.SelectedCard.HasValue && model.SelectedCategory.HasValue)
            {
                int cardId = (int)model.SelectedCard.Value;
                int categoryId = (int)model.SelectedCategory.Value;

                _menuService.AddMenuItem(model.ItemToEdit, cardId, categoryId);

                return RedirectToAction("Index");
            }
            return View(model);
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            MenuItem item = _menuService.GetMenuItemById(id);

            if (item == null)
            {
                return NotFound();
            }


            MenuManagementViewModel viewModel = new MenuManagementViewModel
            {
                ItemToEdit = item,

            };

            return View(viewModel);
        }
        [HttpPost]
        public IActionResult Edit(MenuManagementViewModel model)
        {

            if (model.ItemToEdit != null && model.SelectedCard.HasValue && model.SelectedCategory.HasValue)
            {
                int cardId = (int)model.SelectedCard.Value;
                int categoryId = (int)model.SelectedCategory.Value;

                _menuService.UpdateMenuItem(model.ItemToEdit, cardId, categoryId);

                return RedirectToAction("Index");
            }

            return View(model);
        }

        public IActionResult Deactivate(int id)
        {
            _menuService.DeactivateMenuItem(id);
            return RedirectToAction("Index");
        }

        public IActionResult Activate(int id)
        {
            _menuService.ActivateMenuItem(id);
            return RedirectToAction("Index");
        }
    }
}