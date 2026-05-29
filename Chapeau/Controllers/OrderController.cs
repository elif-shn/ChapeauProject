using Chapeau.Enums;
using Chapeau.Extensions;
using Chapeau.Models;
using Chapeau.Services;
using Chapeau.Services.Interfaces;
using Chapeau.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

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
        public IActionResult TakeOrder(Card? selectedCard, Category? selectedCategory, int selectedTableId)
        {
            try
            {
                List<Menu> allMenus = _menuService.GetMenus(selectedCard, null, true);
                var categories = _menuService.GetCategoriesByCard(allMenus, selectedCard);

                if (selectedCategory != null && !categories.Contains(selectedCategory.Value))
                {
                    selectedCategory = null;
                }

                List<Menu> filteredMenus = _menuService.GetMenus(selectedCard, selectedCategory, true);
                return View(new TakeOrderViewModel
                {
                    Menu = filteredMenus,
                    Categories = categories,
                    SelectedCard = selectedCard,
                    SelectedCategory = selectedCategory,
                    CurrentOrders = HttpContext.Session.GetObject<List<OrderItem>>("CurrentOrder") ?? new List<OrderItem>()
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
        public IActionResult AddNewItemToCurrentOrder(TakeOrderViewModel model)
        {
            try
            {
                List<OrderItem> currentItems = HttpContext.Session.GetObject<List<OrderItem>>("CurrentOrder") ?? new List<OrderItem>();

                MenuItem dbMenuItem = _menuService.GetMenuItemById(model.NewOrderItem.MenuItem.MenuItemId);

                OrderItem newItem = new OrderItem
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

            return RedirectToAction("TakeOrder", new
            {
                selectedCard = model.SelectedCard,
                selectedCategory = model.SelectedCategory,
                selectedTableId = model.SelectedTableId
            });
        }
        [HttpPost]
        public IActionResult SendOrder()
        {
            try
            {
                List<OrderItem> currentItems = HttpContext.Session.GetObject<List<OrderItem>>("CurrentOrder");

                if (currentItems != null && currentItems.Count > 0)
                {
                    foreach (var item in currentItems)
                    {

                        int currentItemsqQantity = item.OrderItemQuantity;

                        for (int i = 0; i < currentItemsqQantity; i++)
                        {
                            _orderServices.AddOrderItemToOrder(item);
                        }
                        _menuService.DecreaseStock(item.MenuItem.MenuItemId, currentItemsqQantity);
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
        public IActionResult DecreaseItemQuantityInCurrentOrder(TakeOrderViewModel model)
        {
            try
            {
                List<OrderItem> currentItems = HttpContext.Session.GetObject<List<OrderItem>>("CurrentOrder") ?? new List<OrderItem>();

                MenuItem dbMenuItem = _menuService.GetMenuItemById(model.NewOrderItem.MenuItem.MenuItemId);

                OrderItem newItem = new OrderItem
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

            return RedirectToAction("TakeOrder", new
            {
                selectedCard = model.SelectedCard,
                selectedCategory = model.SelectedCategory,
                selectedTableId = model.SelectedTableId
            });
        }
        [HttpPost]
        public IActionResult RemoveItem(int menuItemId, int selectedTableId)
        {
            var items = HttpContext.Session.GetObject<List<OrderItem>>("CurrentOrder");

            if (items != null)
            {
                items = _orderServices.RemoveItem(items, menuItemId);
                HttpContext.Session.SetObject("CurrentOrder", items);
            }

            return RedirectToAction("Index", new { selectedTableId });
        }/*
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
        }*/

    }
}
