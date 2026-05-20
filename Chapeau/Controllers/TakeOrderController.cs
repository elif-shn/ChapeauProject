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
        private readonly IMenuListService _menuListService;
        private readonly ITablesService _tableService;
        private readonly ITakeOrderService _takeOrderService;


        public TakeOrderController(IMenuService menuService, IMenuListService menuListService, ITablesService tableService, ITakeOrderService takeOrderService)
        {
            _menuService = menuService;
            _menuListService = menuListService;
            _tableService = tableService;
            _takeOrderService = takeOrderService;
        }
        public ActionResult Index(TakeOrderViewModel model)
        {
            model.Menus = _menuListService.GetAllMenus();
            model.Categories = _menuListService.GetCategoriesByMenu(model.SelectedMenuId);

            bool categoryExists = false;

            foreach (var category in model.Categories)
            {
                if (category == model.SelectedCategory)
                {
                    categoryExists = true;
                    break;
                }
            }

            if (!categoryExists)
            {
                model.SelectedCategory = null;
            }

            model.MenuItems = _menuService.GetActiveItems(model.SelectedMenuId, model.SelectedCategory);
            model.CurrentOrderItems = HttpContext.Session.GetObject<List<OrderItem>>("CurrentOrder") ?? new List<OrderItem>();
            model.Tables = _tableService.GetOccupiedTables();

            model.IsTakeOrder = true;
            model.ActiveOrderId = model.OrderId;

            return View(model);
        }

        [HttpPost]
        [HttpPost]
        public IActionResult AddToCurrentOrder(TakeOrderActionViewModel model)
        {
            string comment = model.Comment ?? "";

            List<OrderItem> items =
                HttpContext.Session.GetObject<List<OrderItem>>("CurrentOrder")
                ?? new List<OrderItem>();

            OrderItem existing = null;

            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].MenuItem.MenuItemId == model.MenuItemId &&
                    items[i].Comment == comment)
                {
                    existing = items[i];
                    break;
                }
            }

            if (existing != null)
            {
                existing.OrderItemQuantity++;
            }
            else
            {
                MenuItem menuItem = _menuService.GetMenuItemById(model.MenuItemId);

                items.Add(new OrderItem
                {
                    MenuItem = menuItem,
                    OrderItemQuantity = 1,
                    Comment = comment
                });
            }

            HttpContext.Session.SetObject("CurrentOrder", items);

            return RedirectToAction("Index", new { SelectedTableId = model.SelectedTableId });
        }

        [HttpPost]
        public IActionResult RemoveFromCurrentOrder(TakeOrderActionViewModel model)
        {
            if (string.IsNullOrEmpty(model.Comment))
            {
                model.Comment = "";
            }

            List<OrderItem> items =
                HttpContext.Session.GetObject<List<OrderItem>>("CurrentOrder")
                ?? new List<OrderItem>();

            OrderItem existing = null;

            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].MenuItem.MenuItemId == model.MenuItemId &&
                    items[i].Comment == model.Comment)
                {
                    existing = items[i];
                    break;
                }
            }

            if (existing != null)
            {
                existing.OrderItemQuantity--;

                if (existing.OrderItemQuantity <= 0)
                {
                    items.Remove(existing);
                }
            }

            HttpContext.Session.SetObject("CurrentOrder", items);

            return RedirectToAction("Index", new { SelectedTableId = model.SelectedTableId });
        }
        [HttpPost]
        [HttpPost]
        public IActionResult SendOrder(TakeOrderActionViewModel model)
        {
            try
            {
                List<OrderItem> currentItems =
                    HttpContext.Session.GetObject<List<OrderItem>>("CurrentOrder")
                    ?? new List<OrderItem>();

                if (currentItems.Count == 0 || model.SelectedTableId == 0)
                {
                    TempData["ErrorMessage"] = "Please select a table.";

                    return RedirectToAction("Index",new { SelectedTableId = model.SelectedTableId });
                }

                foreach (var item in currentItems)
                {
                    for (int i = 0; i < item.OrderItemQuantity; i++)
                    {
                        _takeOrderService.AddItemToTable(
                            model.SelectedTableId,
                            item.MenuItem.MenuItemId,
                            item.Comment
                        );
                    }
                }

                HttpContext.Session.Remove("CurrentOrder");

                TempData["SuccessMessage"] = "Order sent successfully!";

                return RedirectToAction("Index",
                    new { SelectedTableId = model.SelectedTableId });
            }
            catch
            {
                TempData["ErrorMessage"] = "Something went wrong while sending the order.";

                return RedirectToAction("Index",
                    new { SelectedTableId = model.SelectedTableId });
            }
        }
        public IActionResult CancelOrder()
        {
            List<OrderItem> items = HttpContext.Session.GetObject<List<OrderItem>>("CurrentOrder") ?? new List<OrderItem>();


            HttpContext.Session.Remove("CurrentOrder");

            return RedirectToAction("Index");
        }
    }

}
