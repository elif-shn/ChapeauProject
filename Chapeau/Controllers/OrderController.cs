using Chapeau.Enums;
using Chapeau.Extensions;
using Chapeau.Models;
using Chapeau.Services;
using Chapeau.Services.Interfaces;
using Chapeau.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Chapeau.Controllers
{
    public class OrderController : Controller
    {
        private const string CurrentOrderSessionKey = "CurrentOrder";
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

        private List<OrderItem> GetCurrentOrder()
        {
            return HttpContext.Session
                .GetObject<List<OrderItem>>(CurrentOrderSessionKey)
                ?? new List<OrderItem>();
        }

        private void SaveCurrentOrder(List<OrderItem> items)
        {
            HttpContext.Session
                .SetObject(CurrentOrderSessionKey, items);
        }
        [Authorize(Roles = "Waiter")]
        public IActionResult ViewMenuForTakeOrder(Card? selectedCard, Category? selectedCategory, int selectedTableId)
        {
            try
            {
                MenuFilterData data = _menuService.GetMenuData(selectedCard,selectedCategory,true);
                data.SelectedTableId = selectedTableId;
                return View("TakeOrder",
                    new TakeOrderViewModel
                    {
                        MenuFilterData = data,
                        CurrentOrders = GetCurrentOrder()
                    });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;

                return View("TakeOrder",
                    new TakeOrderViewModel
                    {
                        MenuFilterData = new MenuFilterData(),
                        CurrentOrders = GetCurrentOrder(),
                    });
            }
        }
        private IActionResult RedirectToTakeOrder(TakeOrderViewModel model)
        {
            return RedirectToAction(
               "ViewMenuForTakeOrder",
               new
               {
                   selectedCard = model.MenuFilterData?.SelectedCard,
                   selectedCategory = model.MenuFilterData?.SelectedCategory,
                   selectedTableId = model.MenuFilterData?.SelectedTableId
               });
        }



        [HttpPost]
        [Authorize(Roles = "Waiter")]
        public IActionResult AddItemToCurrentOrder(TakeOrderViewModel model)
        {
            try
            {
                List<OrderItem> currentOrder = GetCurrentOrder();

                _orderServices.AddItemToCurrentOrder(currentOrder, model.NewOrderItem.MenuItem.MenuItemId, model.NewOrderItem.Comment);
                if (currentOrder.Any(oi => oi.MenuItem.MenuItemId == model.NewOrderItem.MenuItem.MenuItemId && oi.Comment == model.NewOrderItem.Comment))
                {
                    var orderItem = currentOrder.First(oi => oi.MenuItem.MenuItemId == model.NewOrderItem.MenuItem.MenuItemId && oi.Comment == model.NewOrderItem.Comment);
                    orderItem.Decrease();
                    if (orderItem.OrderItemQuantity <= 0)
                    {
                        currentOrder.Remove(orderItem);
                    }
                }

                SaveCurrentOrder(currentOrder);

                return RedirectToTakeOrder(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToTakeOrder(model);
            }
        }




        [HttpPost]
        [Authorize(Roles = "Waiter")]
        public IActionResult SendOrder(TakeOrderViewModel model)
        {
            try
            {
                List<OrderItem> currentOrder = GetCurrentOrder();

                if (!currentOrder.Any())
                {
                    return RedirectToAction(
                        nameof(ViewMenuForTakeOrder),
                        new
                        {
                            selectedTableId = model.MenuFilterData?.SelectedTableId
                        });
                }
                var user = HttpContext.Session.GetObject<Employee>("LoggedInUser");
                var order = new Order
                {
                    TableId = model.MenuFilterData.SelectedTableId.Value,
                    OrderTime = DateTime.Now,
                    OrderStatus = OrderStatus.Ordered,
                    ServedTime = null,
                    Employee = user,
                    OrderItems = currentOrder,
                };

                _orderServices.CreateOrderWithItems(order);

                HttpContext.Session.Remove(CurrentOrderSessionKey);

                TempData["SuccessMessage"] ="Order sent successfully.";

                return RedirectToAction(
                     "ViewMenuForTakeOrder",
                    new
                    {
                        selectedTableId = model.MenuFilterData?.SelectedTableId
                    });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;

                return RedirectToAction(
                     "ViewMenuForTakeOrder",
                    new
                    {
                        selectedTableId = model.MenuFilterData?.SelectedTableId
                    });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Waiter")]
        public IActionResult RemoveItemInCurrentOrder(TakeOrderViewModel model)
        {
            try
            {
                List<OrderItem> currentOrder = GetCurrentOrder();

             
                var orderItem = currentOrder.FirstOrDefault(oi =>
                    oi.MenuItem.MenuItemId == model.NewOrderItem.MenuItem.MenuItemId &&
                    oi.Comment == model.NewOrderItem.Comment);

                if (orderItem != null)
                {
                    orderItem.Decrease();
                    if (orderItem.OrderItemQuantity <= 0)
                    {
                        currentOrder.Remove(orderItem);
                    }
                }

                SaveCurrentOrder(currentOrder);

                return RedirectToTakeOrder(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;

                return RedirectToTakeOrder(model);
            }
        }





        [HttpPost]
        [Authorize(Roles = "Waiter")]
        public IActionResult DeleteItem(TakeOrderViewModel model)
        {
            try
            {
                List<OrderItem> currentOrder = GetCurrentOrder();

                // Remove the item from the current order manually, since IOrderService does not have DeleteItem
                var orderItem = currentOrder.FirstOrDefault(oi =>
                    oi.MenuItem.MenuItemId == model.NewOrderItem.MenuItem.MenuItemId &&
                    oi.Comment == model.NewOrderItem.Comment);

                if (orderItem != null)
                {
                    currentOrder.Remove(orderItem);
                }

                SaveCurrentOrder(currentOrder);

                return RedirectToTakeOrder(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;

                return RedirectToTakeOrder(model);
            }
        }





        [HttpPost]
        [Authorize(Roles = "Waiter")]
        public IActionResult AddNote(TakeOrderViewModel model)
        {
            try
            {
                List<OrderItem> currentOrder = GetCurrentOrder();

                // Manually add or update the comment for the matching order item
                var orderItem = currentOrder.FirstOrDefault(oi =>
                    oi.MenuItem.MenuItemId == model.NewOrderItem.MenuItem.MenuItemId);

                if (orderItem != null)
                {
                    orderItem.Comment = model.NewOrderItem.Comment;
                }

                SaveCurrentOrder(currentOrder);

                return RedirectToTakeOrder(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;

                return RedirectToTakeOrder(model);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Waiter")]
        public IActionResult CancelOrder(TakeOrderViewModel model)
        {
            HttpContext.Session.Remove(CurrentOrderSessionKey);

            TempData["SuccessMessage"] = "Order cancelled successfully.";

            return RedirectToAction(
                    "ViewMenuForTakeOrder",
                new
                {
                    selectedTableId = model.MenuFilterData?.SelectedTableId
                });
        }
    }
}
