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
        private readonly IOrderService _orderServices;

        public OrderController(IMenuService menuService, IOrderService orderServices)
        {
            _menuService = menuService;
            _orderServices = orderServices;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetRunningKitchenOrders()
        {
            try
            {
                List<Order> orders = _orderServices.GetRunningOrders(true);
                return View(orders);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View(new List<Order>());
            }
        }

        [HttpGet]
        public IActionResult GetRunningBarOrders()
        {
            try
            {
                List<Order> orders = _orderServices.GetRunningOrders(false);
                return View(orders);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View(new List<Order>());
            }
        }

        [HttpGet]
        public IActionResult GetFinishedKitchenOrders()
        {
            try
            {
                List<Order> orders = _orderServices.GetFinishedOrders(true);
                return View(orders);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View(new List<Order>());
            }
        }

        [HttpGet]
        public IActionResult GetFinishedBarOrders()
        {
            try
            {
                List<Order> orders = _orderServices.GetFinishedOrders(false);
                return View(orders);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View(new List<Order>());
            }
        }

        [HttpPost]
        public IActionResult UpdateOrderStatus(Order order, bool isFood)
        {
            try
            {
                _orderServices.UpdateOrderStatus(order);

                TempData["SuccessMessage"] =
                    "Order status updated successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

           if (isFood)
           {
               return RedirectToAction("GetRunningKitchenOrders");
           }
           else
           {
               return RedirectToAction("GetRunningBarOrders");
           }
        }

        [HttpPost]
        public IActionResult UpdateOrderItemStatus(OrderItem orderItem, bool isFood)
        {
            try
            {
                _orderServices.UpdateOrderItemStatus(orderItem);

                TempData["SuccessMessage"] =
                    "Order item status updated successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            if (isFood)
            {
                return RedirectToAction("GetRunningKitchenOrders");
            }
            else
            {
                return RedirectToAction("GetRunningBarOrders");
            }


        }

        [HttpPost]
        public IActionResult UpdateCourseStatus( Order order, Category category, OrderItemStatus status)
        {
            try
            {
                _orderServices.UpdateCourseStatus(order, category, status);

                TempData["SuccessMessage"]= "Course status updated successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction("GetRunningKitchenOrders");
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
