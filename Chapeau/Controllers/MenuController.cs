using Chapeau.Enums;
using Chapeau.Models;
using Microsoft.AspNetCore.Authorization;
using Chapeau.ViewModels;
using Microsoft.AspNetCore.Mvc;
namespace Chapeau.Controllers
{
    public class MenuController : Controller
    {
        private readonly IMenuService _menuService;

        /*for login to the management part user name : Mehedi and Password :12345*/
        /*for login for the waiter user name : Elif and Password :1234*/
        public MenuController(IMenuService menuService)
        {
            _menuService = menuService;
        }

        public IActionResult Index(Card? selectedCard, Category? selectedCategory)
        {
            try
            {
                MenuFilterData data =
                    _menuService.GetMenuData(selectedCard, selectedCategory, true);

                return View(new MenuListViewModel
                {
                    MenuFilterData = data,
                });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;

                return View(new MenuListViewModel
                {
                    MenuFilterData = new MenuFilterData(),
                });
            }
        }
        [Authorize(Roles = "Manager")]
        public IActionResult Management(Card? selectedCard, Category? selectedCategory) 
        {
            try
            {
                MenuFilterData data = _menuService.GetMenuData(selectedCard, selectedCategory, false);
                return View(new MenuManagementViewModel
                {
                    Menu = data.Menus,
                    Categories = data.Categories,
                    SelectedCard = data.SelectedCard,
                    SelectedCategory = data.SelectedCategory
                });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View(new MenuManagementViewModel());
            }
        }
        [Authorize(Roles = "Manager")]
        public IActionResult Add()
        {
            return View(new MenuManagementViewModel { ItemToEdit = new MenuItem() });
        }

        [Authorize(Roles = "Manager")]
        [HttpPost]
        public IActionResult Add(MenuManagementViewModel model)
        {
            if (model.ItemToEdit != null && model.SelectedCard.HasValue && model.SelectedCategory.HasValue)
            {
                int cardId = (int)model.SelectedCard.Value;
                int categoryId = (int)model.SelectedCategory.Value;

                _menuService.AddMenuItem(model.ItemToEdit, cardId, categoryId);
                TempData["SuccessMessage"] = "Menu item added successfully!";
                return RedirectToAction("Management");
            }
            return View(model);
        }
        [HttpGet]
        [Authorize(Roles = "Manager")]
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
                SelectedCard = item.Menu?.Card,
                SelectedCategory = item.Menu?.Category,
            };
            return View(viewModel);
        }

        [Authorize(Roles = "Manager")]
        [HttpPost]
        public IActionResult Edit(MenuManagementViewModel model)
        {

            if (model.ItemToEdit != null && model.SelectedCard.HasValue && model.SelectedCategory.HasValue)
            {
                int cardId = (int)model.SelectedCard.Value;
                int categoryId = (int)model.SelectedCategory.Value;

                _menuService.UpdateMenuItem(model.ItemToEdit, cardId, categoryId);
                TempData["SuccessMessage"] = "Menu item updated successfully!";
                return RedirectToAction("Management");
            }

            return View(model);
        }

        [Authorize(Roles = "Manager")]
        [HttpPost]
        public IActionResult Deactivate(int id)
        {
            _menuService.DeactivateMenuItem(id);
            TempData["SuccessMessage"] = "Menu item deactivated successfully!";
            return RedirectToAction("Management");
        }

        [Authorize(Roles = "Manager")]
        [HttpPost]
        public IActionResult Activate(int id)
        {
            _menuService.ActivateMenuItem(id);
            TempData["SuccessMessage"] = "Menu item activated successfully!";
            return RedirectToAction("Management");
        }
    }
}