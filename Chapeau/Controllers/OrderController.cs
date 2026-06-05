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
            List<Order> runningOrders = _orderServices.GetKitchenOrders();
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

                var allMenus = _menuService.GetMenus(selectedCard, null, true).ToList();
                var categories = allMenus.Select(m => m.Category).Distinct().ToList();

                // Seçili kategori mevcut kategoriler arasında yoksa sıfırlıyoruz
                if (selectedCategory != null && !categories.Contains(selectedCategory.Value))
                    selectedCategory = null;

                var filteredMenus = allMenus
                    .Where(m => selectedCategory == null || m.Category == selectedCategory)
                    .ToList();

                return View("TakeOrder", new TakeOrderViewModel
                {
                    Menu = filteredMenus,
                    Categories = categories,
                    SelectedCard = selectedCard,
                    SelectedCategory = selectedCategory,
                    SelectedTableId = selectedTableId,
                    CurrentOrders = HttpContext.Session.GetObject<List<OrderItem>>("CurrentOrder") ?? new()
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
                var currentItems = HttpContext.Session.GetObject<List<OrderItem>>("CurrentOrder") ?? new List<OrderItem>();
                var dbMenuItem = _menuService.GetMenuItemById(model.NewOrderItem.MenuItem.MenuItemId);

                var newItem = new OrderItem
                {
                    MenuItem = dbMenuItem,
                    Comment = model.NewOrderItem.Comment
                };

                currentItems = _orderServices.ModifyCurrentOrderItem(currentItems, newItem, 1);
                HttpContext.Session.SetObject("CurrentOrder", currentItems);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction("ViewMenuForTakeOrder", new
            {
                selectedCard = model.SelectedCard,
                selectedCategory = model.SelectedCategory,
                selectedTableId = HttpContext.Session.GetInt32("SelectedTableId")
            });
        }

        [HttpPost]
        public IActionResult SendOrder()
        {
            try
            {
                var tableId = HttpContext.Session.GetInt32("SelectedTableId");
                var currentItems = HttpContext.Session.GetObject<List<OrderItem>>("CurrentOrder");

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
                HttpContext.Session.Remove("CurrentOrder");
                TempData["SuccessMessage"] = "Order sent successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction("ViewMenuForTakeOrder", new
            {
                selectedTableId = HttpContext.Session.GetInt32("SelectedTableId")
            });
        }

        [HttpPost]
        public IActionResult DecreaseItemQuantityInCurrentOrder(TakeOrderViewModel model)
        {
            try
            {
                var currentItems = HttpContext.Session.GetObject<List<OrderItem>>("CurrentOrder") ?? new List<OrderItem>();
                var dbMenuItem = _menuService.GetMenuItemById(model.NewOrderItem.MenuItem.MenuItemId);

                var newItem = new OrderItem
                {
                    MenuItem = dbMenuItem,
                    Comment = model.NewOrderItem.Comment
                };

                currentItems = _orderServices.ModifyCurrentOrderItem(currentItems, newItem, -1);
                HttpContext.Session.SetObject("CurrentOrder", currentItems);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction("ViewMenuForTakeOrder", new
            {
                selectedCard = model.SelectedCard,
                selectedCategory = model.SelectedCategory,
                selectedTableId = HttpContext.Session.GetInt32("SelectedTableId")
            });
        }

        [HttpPost]
        public IActionResult RemoveItem(int menuItemId)
        {
            var items = HttpContext.Session.GetObject<List<OrderItem>>("CurrentOrder");

            if (items != null)
            {
                items = _orderServices.RemoveItem(items, menuItemId);
                HttpContext.Session.SetObject("CurrentOrder", items);
            }

            return RedirectToAction("ViewMenuForTakeOrder", new
            {
                selectedTableId = HttpContext.Session.GetInt32("SelectedTableId")
            });
        }

        [HttpPost] // Güvenlik için HttpPost niteliğini eklemek iyi bir pratiktir
        public IActionResult AddNote(int menuItemId, string comment)
        {
            var items = HttpContext.Session.GetObject<List<OrderItem>>("CurrentOrder") ?? new List<OrderItem>();

            // LINQ: Eski foreach döngüsünü tamamen kaldırıp nokta atışı elemanı buluyoruz
            var itemToUpdate = items.FirstOrDefault(item => item.MenuItem.MenuItemId == menuItemId);

            if (itemToUpdate != null)
            {
                itemToUpdate.Comment = comment;
                HttpContext.Session.SetObject("CurrentOrder", items);
            }

            return RedirectToAction("ViewMenuForTakeOrder", new
            {
                selectedTableId = HttpContext.Session.GetInt32("SelectedTableId")
            });
        }

        [HttpPost]
        public IActionResult CancelOrder(int selectedTableId)
        {
            try
            {
                HttpContext.Session.Remove("CurrentOrder");
                TempData["SuccessMessage"] = "Order cancelled successfully.";
                return RedirectToAction("ViewMenuForTakeOrder", new { selectedTableId });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("ViewMenuForTakeOrder", new { selectedTableId });
            }
        }
    }
}
