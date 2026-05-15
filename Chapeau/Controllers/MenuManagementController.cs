using Chapeau.Models;
using Chapeau.Services;
using Chapeau.ViewModels;
using Microsoft.AspNetCore.Mvc;
namespace Chapeau.Controllers
{
    public class MenuManagementController : Controller
    {
        private IMenuService _menuService;

        public MenuManagementController(IMenuService menuService)
        {
            _menuService = menuService;
        }

        public IActionResult Index(int menuId = 0, int category = 0)
        {
            MenuManagementViewModel vm = new MenuManagementViewModel();
            vm.MenuItems = _menuService.GetFilteredMenuItems(menuId, category);
            vm.SelectedMenuId = menuId;
            vm.SelectedCategory = category;
            return View(vm);
        }

        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Add(MenuItem newItem)
        {
            _menuService.AddMenuItem(newItem);
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            MenuManagementViewModel vm = new MenuManagementViewModel();
            vm.ItemToEdit = _menuService.GetMenuItemById(id);
            return View(vm);
        }

        [HttpPost]
        public IActionResult Edit(MenuItem updatedItem)
        {
            _menuService.UpdateMenuItem(updatedItem);
            return RedirectToAction("Index");
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