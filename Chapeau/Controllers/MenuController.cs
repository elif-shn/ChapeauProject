using Chapeau.Enums;
using Chapeau.Models;
using Chapeau.ViewModels;
using Microsoft.AspNetCore.Mvc;
namespace Chapeau.Controllers
{
    public class MenuController : Controller
    {
        private readonly IMenuService _menuService;


        public MenuController(IMenuService menuService)
        {
            _menuService = menuService;
        }
        private MenuViewModel GetViewModel(Card? selectedCard, Category? selectedCategory, bool onlyActive)
        {
            var allMenus = _menuService.GetMenus(selectedCard, null, true).ToList();

            var categories = allMenus.Select(m => m.Category).Distinct().ToList();

            if (selectedCategory != null && !categories.Contains(selectedCategory.Value))
            {
                selectedCategory = null;
            }

            var filteredMenus = allMenus.Where(m => selectedCategory == null || m.Category == selectedCategory).ToList();

            return new MenuViewModel
            {
                Menu = filteredMenus,
                Categories = categories,
                SelectedCard = selectedCard,
                SelectedCategory = selectedCategory,
            };
        }
        public IActionResult Index(Card? selectedCard, Category? selectedCategory)
        {
            return View(GetViewModel(selectedCard, selectedCategory, true));
        }
        public IActionResult Management(Card? selectedCard, Category? selectedCategory)
        {
            return View("Management", GetViewModel(selectedCard, selectedCategory, false));
        }
        public IActionResult Add()
        {
            return View(new MenuViewModel { ItemToEdit = new MenuItem() });
        }

        [HttpPost]
        public IActionResult Add(MenuViewModel model)
        {
            if (model.ItemToEdit != null && model.SelectedCard.HasValue && model.SelectedCategory.HasValue)
            {
                int cardId = (int)model.SelectedCard.Value;
                int categoryId = (int)model.SelectedCategory.Value;

                _menuService.AddMenuItem(model.ItemToEdit, cardId, categoryId);

                return RedirectToAction("Management");
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


            MenuViewModel viewModel = new MenuViewModel
            {
                ItemToEdit = item,

            };

            return View(viewModel);
        }
        [HttpPost]
        public IActionResult Edit(MenuViewModel model)
        {

            if (model.ItemToEdit != null && model.SelectedCard.HasValue && model.SelectedCategory.HasValue)
            {
                int cardId = (int)model.SelectedCard.Value;
                int categoryId = (int)model.SelectedCategory.Value;

                _menuService.UpdateMenuItem(model.ItemToEdit, cardId, categoryId);

                return RedirectToAction("Management");
            }

            return View(model);
        }

        public IActionResult Deactivate(int id)
        {
            _menuService.DeactivateMenuItem(id);
            return RedirectToAction("Management");
        }

        public IActionResult Activate(int id)
        {
            _menuService.ActivateMenuItem(id);
            return RedirectToAction("Management");
        }
    }
}