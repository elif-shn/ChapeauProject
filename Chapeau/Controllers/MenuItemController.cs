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
        public ActionResult Index(string search)
        {
            List<MenuItemViewModel> menuViewModel = new List<MenuItemViewModel>();

            try
            {
                List<MenuItem> menuItems = _menuItemRepository.GetAll();

                foreach (var item in menuItems)
                {
                    menuViewModel.Add(new MenuItemViewModel
                    {
                        MenuItemName = item.MenuItemName,
                        MenuItemPrice = item.MenuItemPrice
                    });
                }

                return View("Index", menuViewModel);
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Message;
                return View("Index", new List<MenuItemViewModel>());
            }
        }
    }
}
