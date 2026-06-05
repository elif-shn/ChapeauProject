using Chapeau.Enums;
using Chapeau.Extensions;
using Chapeau.Models;
using Chapeau.Services.Interfaces;
using Chapeau.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Chapeau.Controllers
{
    public class OrderController : Controller
    {
        private readonly IMenuService _menuService;
        private readonly IOrderService _orderServices;

        public OrderController(IMenuService menuService, IOrderService orderServices)
        {
            _menuService = menuService;
            _orderServices = orderServices;
        }

        public IActionResult Index()
        {
            List<Order> runningOrders = _orderServices.GetRunningOrders();
            return View(runningOrders);
        }

        [HttpPost]
        public IActionResult UpdateStatus(Order order, OrderStatus status)
        {
            try
            {
                _orderServices.UpdateOrderStatus(order, status);
                TempData["SuccessMessage"] = "Order status updated successfully.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public IActionResult UpdateItemStatus(OrderItem orderItem, OrderItemStatus status)
        {
            try
            {
                _orderServices.UpdateOrderItemStatus(orderItem, status);
                TempData["SuccessMessage"] = "Item status updated.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        public IActionResult FinishedOrders()
        {
            try
            {
                List<Order> finishedOrders = _orderServices.GetFinishedOrders();
                return View(finishedOrders);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        public IActionResult ViewMenuForTakeOrder(Card? selectedCard, Category? selectedCategory, int selectedTableId)
        {
            try
            {
                HttpContext.Session.SetInt32("SelectedTableId", selectedTableId);

                MenuFilterData data = _menuService.GetMenuData(selectedCard, selectedCategory, false);

                return View("TakeOrder", new TakeOrderViewModel
                {
                    Menu = data.Menus,
                    Categories = data.Categories,
                    SelectedCard = data.SelectedCard,
                    SelectedCategory = data.SelectedCategory,
                    SelectedTableId = selectedTableId,
                    CurrentOrders = new List<OrderItem>()
                });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View(new TakeOrderViewModel
                {
                    Menu = new List<Menu>(),
                    SelectedCard = selectedCard,
                    SelectedTableId = selectedTableId
                });
            }
        }

        [HttpPost]
        public IActionResult AddNewItemToCurrentOrder(TakeOrderViewModel model)
        {
            try
            {
                var currentItems = model.CurrentOrders ?? new List<OrderItem>();
                var dbMenuItem = _menuService.GetMenuItemById(model.NewOrderItem.MenuItem.MenuItemId);

                var newItem = new OrderItem
                {
                    MenuItem = dbMenuItem,
                    Comment = model.NewOrderItem?.Comment ?? ""
                };

                model.CurrentOrders = _orderServices.ModifyCurrentOrderItem(currentItems, newItem, 1);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            var allMenus = _menuService.GetMenus(model.SelectedCard, null, true).ToList();
            model.Categories = allMenus.Select(m => m.Category).Distinct().ToList();
            model.Menu = allMenus.Where(m => model.SelectedCategory == null || m.Category == model.SelectedCategory).ToList();

            ViewData["CurrentOrders"] = model.CurrentOrders;
            return View("TakeOrder", model);
        }

        [HttpPost]
        public IActionResult SendOrder(TakeOrderViewModel model)
        {
            try
            {
                var tableId = model.SelectedTableId;
                var currentItems = model.CurrentOrders;

                if (currentItems == null || !currentItems.Any())
                    return RedirectToAction("ViewMenuForTakeOrder", new { selectedTableId = tableId });

                var newOrder = new Order
                {
                    TableId = tableId.Value,
                    OrderTime = DateTime.Now,
                    OrderStatus = OrderStatus.Ordered,
                    OrderItems = currentItems
                };

                _orderServices.SendOrder(newOrder);
                TempData["SuccessMessage"] = "Order sent successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction("ViewMenuForTakeOrder",
                     new
                     {
                         selectedTableId = model.SelectedTableId
                     });
                        }

        [HttpPost]
        public IActionResult DecreaseItemQuantityInCurrentOrder(TakeOrderViewModel model)
        {
            try
            {
                var currentItems = model.CurrentOrders ?? new List<OrderItem>();
                var dbMenuItem = _menuService.GetMenuItemById(model.NewOrderItem.MenuItem.MenuItemId);

                var newItem = new OrderItem
                {
                    MenuItem = dbMenuItem,
                    Comment = model.NewOrderItem?.Comment ?? ""
                };

                model.CurrentOrders = _orderServices.ModifyCurrentOrderItem(currentItems, newItem, -1);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            var allMenus = _menuService.GetMenus(model.SelectedCard, null, true).ToList();
            model.Categories = allMenus.Select(m => m.Category).Distinct().ToList();
            model.Menu = allMenus.Where(m => model.SelectedCategory == null || m.Category == model.SelectedCategory).ToList();

            ViewData["CurrentOrders"] = model.CurrentOrders;
            return View("TakeOrder", model);
        }

        [HttpPost]
        public IActionResult RemoveItem(int menuItemId, TakeOrderViewModel model)
        {
            var items = model.CurrentOrders;

            if (items != null)
            {
                items = _orderServices.RemoveItem(items, menuItemId);
                model.CurrentOrders = items;
            }
            var allMenus = _menuService.GetMenus(model.SelectedCard, null, true).ToList();

            model.Categories = allMenus.Select(m => m.Category).Distinct().ToList();

            model.Menu = allMenus
                .Where(m => model.SelectedCategory == null || m.Category == model.SelectedCategory)
                .ToList();

            ViewData["CurrentOrders"] = model.CurrentOrders;

            return View("TakeOrder", model);
        }

        [HttpPost]
        public IActionResult AddNote(int menuItemId, string comment, TakeOrderViewModel model)
        {
            var items = model.CurrentOrders;

            var itemToUpdate = items.FirstOrDefault(item => item.MenuItem.MenuItemId == menuItemId);

            if (itemToUpdate != null)
            {
                itemToUpdate.Comment = comment;
            }

            var allMenus = _menuService.GetMenus(model.SelectedCard, null, true).ToList();

            model.Categories = allMenus.Select(m => m.Category).Distinct().ToList();

            model.Menu = allMenus
                .Where(m => model.SelectedCategory == null || m.Category == model.SelectedCategory)
                .ToList();

            ViewData["CurrentOrders"] = model.CurrentOrders;

            return View("TakeOrder", model);
        }

        [HttpPost]
        public IActionResult CancelOrder(TakeOrderViewModel model)
        {
            try
            {
                model.CurrentOrders.Clear();
                TempData["SuccessMessage"] = "Order cancelled successfully.";
                return RedirectToAction("ViewMenuForTakeOrder",
                    new
                    {
                        selectedTableId = model.SelectedTableId
                    });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("ViewMenuForTakeOrder", new { model.SelectedTableId });
            }
        }
    }
}
