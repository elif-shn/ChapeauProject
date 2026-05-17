using Chapeau.Enums;
using Chapeau.Models;
using Chapeau.Repositories;
using Chapeau.Services;
using Chapeau.ViewModels;
using Microsoft.AspNetCore.Mvc;
namespace Chapeau.Controllers
{
    public class MenuManagementController : Controller
    {
        private readonly IMenuService _menuService;
        private readonly IMenuListService _menuListService;


        public MenuManagementController(IMenuService menuService, IMenuListService menuListService)
        {
            _menuService = menuService;
            _menuListService = menuListService;
        }

        /*public IActionResult Index(int menuId = 0, int category = 0)
        {
           MenuManagementViewModel vm = new MenuManagementViewModel();
            vm.MenuItems = _menuService.GetFilteredMenuItems(menuId, category);
            vm.SelectedMenuId = menuId;
            vm.SelectedCategory = category;
            return View(vm);
        }*/
        public ActionResult Index(MenuViewModel menuViewModel)
        {
            menuViewModel.Categories = Enum.GetValues(typeof(Category)).Cast<Category>().ToList();

            menuViewModel.Menus = _menuListService.GetAllMenus();


            List<MenuItem> menuItems = _menuService.GetAllByFilter(menuViewModel);


            menuViewModel.MenuItems = menuItems;

            return View(menuViewModel);
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
            MenuViewModel vm = new MenuViewModel();
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