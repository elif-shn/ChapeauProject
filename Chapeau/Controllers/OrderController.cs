using Chapeau.Enums;
using Chapeau.Extensions;
using Chapeau.Models;
using Chapeau.Services;
using Chapeau.Services.Interfaces;
using Chapeau.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Chapeau.Controllers
{
    public class OrderController : Controller
    {
        private readonly IMenuService _menuService;
        private readonly ITablesService _tableService;
        private readonly IOrderService _orderServices;



        public OrderController(IMenuService menuService, ITablesService tableService, IOrderService orderServices)
        {
            _menuService = menuService;
            _tableService = tableService;
            _orderServices = orderServices;
        }

        public IActionResult Index()
        {
            List<Order> orders = _orderServices.GetAllOrders();
            return View(orders);
        }

        public IActionResult OrderItems(int orderId)
        {
            try
            {
                List<OrderItem> orderItems = _orderServices.GetOrderItemsByOrderId(orderId);
                return View(orderItems);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public IActionResult UpdateStatus(int orderId, OrderStatus status)
        {
            try
            {
                _orderServices.UpdateOrderStatus(orderId, status);
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
        public IActionResult UpdateItemStatus(int orderItemId, string status)
        {
            try
            {
                OrderStatus newStatus = Enum.Parse<OrderStatus>(status);
                _orderServices.UpdateOrderItemStatus(orderItemId, newStatus);
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
                TempData["SuccessMessage"] = "Finished orders retrieved successfully.";
                return View(finishedOrders);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Error");
            }
        }
        public IActionResult TakeOrder(TakeOrderViewModel takeOrderViewModel)
        {
            try
            {
                return View(new TakeOrderViewModel
                {
                    Menu = _menuService.GetMenuDisplay(takeOrderViewModel.selectedCard, takeOrderViewModel.selectedCategory).menus,
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
            try
            {
                var menuItem = _menuService.GetMenuItemById(model.MenuItemId);

                List<CurrentOrderModel> items = HttpContext.Session.GetObject<List<CurrentOrderModel>>("CurrentOrder") ?? new List<CurrentOrderModel>();

                items = _orderServices.AddOrUpdateOrderItem(items, model, menuItem);

                HttpContext.Session.SetObject("CurrentOrder", items);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction("Index", new
            {
                selectedCard = selectedCard,
                selectedCategory = selectedCategory,
                selectedTableId = selectedTableId
            });
        }
        [HttpPost]
        public IActionResult SendOrder(int selectedTableId)
        {
            if (selectedTableId <= 0)
            {
                TempData["ErrorMessage"] = "Please select a table before sending the order.";
                return RedirectToAction("Index");
            }
            try
            {
                List<CurrentOrderModel> items = HttpContext.Session.GetObject<List<CurrentOrderModel>>("CurrentOrder");

                if (items != null && items.Count > 0)
                {
                    foreach (var item in items)
                    {

                        int quantity = item.Quantity;

                        for (int i = 0; i < quantity; i++)
                        {
                            _orderServices.AddOrderItemToOrder(selectedTableId, item.MenuItemId, item.Comment);
                            _menuService.DecreaseStock(item.MenuItemId, quantity);
                        }
                    }
                    HttpContext.Session.Remove("CurrentOrder");
                }
                TempData["SuccessMessage"] = "Order successfully sent to the kitchen!";

                return RedirectToAction("Index", "TakeOrder");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while sending the order: " + ex.Message;
                return RedirectToAction("Index");
            }
        }
        [HttpPost]
        public IActionResult IncreaseQuantity(int menuItemId, int selectedTableId)
        {
            var menuItem = _menuService.GetMenuItemById(menuItemId);
            var items = HttpContext.Session.GetObject<List<CurrentOrderModel>>("CurrentOrder");

            if (items == null) items = new List<CurrentOrderModel>();

            try
            {
                items = _orderServices.UpdateItemQuantity(items, menuItemId, 1, menuItem);
                HttpContext.Session.SetObject("CurrentOrder", items);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction("Index", new { selectedTableId });
        }
        [HttpPost]
        public IActionResult DecreaseQuantity(int menuItemId, int selectedTableId)
        {
            var items = HttpContext.Session.GetObject<List<CurrentOrderModel>>("CurrentOrder");

            if (items != null)
            {
                items = _orderServices.UpdateItemQuantity(items, menuItemId, -1, null);
                HttpContext.Session.SetObject("CurrentOrder", items);
            }

            return RedirectToAction("Index", new { selectedTableId });
        }

        [HttpPost]
        public IActionResult RemoveItem(int menuItemId, int selectedTableId)
        {
            var items = HttpContext.Session.GetObject<List<CurrentOrderModel>>("CurrentOrder");

            if (items != null)
            {
                items = _orderServices.RemoveItem(items, menuItemId);
                HttpContext.Session.SetObject("CurrentOrder", items);
            }

            return RedirectToAction("Index", new { selectedTableId });
        }
        [HttpPost]
        public IActionResult AddNote(int menuItemId, int selectedTableId, string comment)
        {
            var items = HttpContext.Session.GetObject<List<CurrentOrderModel>>("CurrentOrder") ?? new List<CurrentOrderModel>();

            CurrentOrderModel itemToUpdate = null;

            foreach (var item in items)
            {
                if (item.MenuItemId == menuItemId)
                {
                    itemToUpdate = item;
                }
            }

            if (itemToUpdate != null)
            {
                itemToUpdate.Comment = comment;

                HttpContext.Session.SetObject("CurrentOrder", items);
            }
            return RedirectToAction("Index", new { selectedTableId = selectedTableId });
        }
        [HttpPost]
        public IActionResult CancelOrder(int selectedTableId)
        {
            try
            {
                HttpContext.Session.Remove("CurrentOrder");

                TempData["SuccessMessage"] = "Order cancelled successfully.";

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;

                return RedirectToAction("Index", new { selectedTableId });
            }
        }

    }
}
