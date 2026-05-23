using Chapeau.Enums;
using Chapeau.Extensions;
using Chapeau.Models;
using Chapeau.Services;
using Chapeau.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Chapeau.Controllers
{
    public class TakeOrderController : Controller
    {
        private readonly IMenuService _menuService;
        private readonly ITablesService _tableService;
        private readonly ITakeOrderService _takeOrderService;



        public TakeOrderController(IMenuService menuService, ITablesService tableService, ITakeOrderService takeOrderService)
        {
            _menuService = menuService;
            _tableService = tableService;
            _takeOrderService = takeOrderService;
        }

        public IActionResult Index(Card? selectedCard, Category? selectedCategory, int selectedTableId)
        {
            try
            {
                return View(new TakeOrderViewModel
                {
                    Menu = _menuService.GetMenuDisplay(selectedCard, selectedCategory).menus,
                    Categories = _menuService.GetMenuDisplay(selectedCard, selectedCategory).categories,
                    OccupiedTables = _tableService.GetOccupiedTables(),
                    SelectedTableId = selectedTableId,
                    SelectedCard = selectedCard,
                    SelectedCategory = selectedCategory,
                    CurrentOrder = HttpContext.Session.GetObject<List<CurrentOrderModel>>("CurrentOrder") ?? new List<CurrentOrderModel>()
                });
            }
            catch (Exception ex)
            {
                return View(new TakeOrderViewModel
                {
                    Menu = new List<Menu>(),
                    SelectedCard = selectedCard,

                });
            }

        }
        [HttpPost]
        public IActionResult AddCurrentOrder(CurrentOrderModel model, Card? selectedCard, Category? selectedCategory, int selectedTableId)
        {           
            List<CurrentOrderModel> items = HttpContext.Session.GetObject<List<CurrentOrderModel>>("CurrentOrder") ?? new List<CurrentOrderModel>();

            items = _takeOrderService.AddOrUpdateOrderItem(items, model);

            HttpContext.Session.SetObject("CurrentOrder", items);

            return RedirectToAction("Index", new
            {
                selectedCard = selectedCard,
                selectedCategory = selectedCategory,
                selectedTableId = selectedTableId
            });
        }

    }

}

