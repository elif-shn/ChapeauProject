using Chapeau.Enums;
using Chapeau.Extensions;
using Chapeau.Models;
using Chapeau.Services.Interfaces;
using Chapeau.ViewModels;
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

        public IActionResult ViewMenuForTakeOrder(Card? selectedCard, Category? selectedCategory, int selectedTableId)
        {
            try
            {
                MenuFilterData data = _menuService.GetMenuData(selectedCard,selectedCategory,true);

                return View("TakeOrder",
                    new TakeOrderViewModel
                    {
                        MenuFilterData = data,
                        SelectedTableId = selectedTableId,
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
                        CurrentOrders = GetCurrentOrder()
                    });
            }
        }

        [HttpPost]
        public IActionResult AddNewItemToCurrentOrder(TakeOrderViewModel model)
        {
            try
            {
                var orders = GetCurrentOrder();

                var item = new OrderItem
                {
                    MenuItem = new MenuItem
                    {
                        MenuItemId = model.NewOrderItem.MenuItem.MenuItemId
                    },
                    Comment = model.NewOrderItem?.Comment ?? ""
                };

                orders = _orderServices .ModifyCurrentOrderItem(orders, item, 1);

                SaveCurrentOrder(orders);

                return RedirectToAction(
                    nameof(ViewMenuForTakeOrder),
                    new
                    {
                        model.MenuFilterData,
                        model.SelectedTableId
                    });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;

                return RedirectToAction(
                    nameof(ViewMenuForTakeOrder),
                    new
                    {
                        model.MenuFilterData,
                        model.SelectedTableId
                    });
            }
        }

        [HttpPost]
        public IActionResult SendOrder(TakeOrderViewModel model)
        {
            try
            {
                var orders = GetCurrentOrder();

                if (!orders.Any())
                {
                    return RedirectToAction(
                        nameof(ViewMenuForTakeOrder),
                        new
                        {
                            model.SelectedTableId
                        });
                }

                var order = new Order
                {
                    TableId = model.SelectedTableId.Value,
                    OrderTime = DateTime.Now,
                    OrderStatus = OrderStatus.Ordered,
                    OrderItems = orders
                };

                _orderServices.SendOrder(order);

                HttpContext.Session.Remove(CurrentOrderSessionKey);

                TempData["SuccessMessage"] ="Order sent successfully.";

                return RedirectToAction(
                    nameof(ViewMenuForTakeOrder),
                    new
                    {
                        model.SelectedTableId
                    });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;

                return RedirectToAction(
                    nameof(ViewMenuForTakeOrder),
                    new
                    {
                        model.SelectedTableId
                    });
            }
        }

        [HttpPost]
        public IActionResult DecreaseItemQuantityInCurrentOrder(TakeOrderViewModel model)
        {
            try
            {
                var orders = GetCurrentOrder();

                var item = new OrderItem
                {
                    MenuItem = new MenuItem
                    {
                        MenuItemId = model.NewOrderItem.MenuItem.MenuItemId
                    },
                    Comment = model.NewOrderItem?.Comment ?? ""
                };

                orders = _orderServices.ModifyCurrentOrderItem(orders, item, -1);

                SaveCurrentOrder(orders);

                return RedirectToAction(
                    nameof(ViewMenuForTakeOrder),
                    new
                    {
                        model.MenuFilterData,
                        model.SelectedTableId
                    });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;

                return RedirectToAction(
                    nameof(ViewMenuForTakeOrder),
                    new
                    {
                        model.MenuFilterData,
                        model.SelectedTableId
                    });
            }
        }

        [HttpPost]
        public IActionResult RemoveItem(int menuItemId, TakeOrderViewModel model)
        {
            try
            {
                var orders = GetCurrentOrder();

                orders =_orderServices.RemoveItem(orders,menuItemId);

                SaveCurrentOrder(orders);

                return RedirectToAction(
                    nameof(ViewMenuForTakeOrder),
                    new
                    {
                        model.MenuFilterData,
                        model.SelectedTableId
                    });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;

                return RedirectToAction(
                    nameof(ViewMenuForTakeOrder),
                    new
                    {
                        model.MenuFilterData,
                        model.SelectedTableId
                    });
            }
        }

        [HttpPost]
        public IActionResult AddNote(int menuItemId, string comment, TakeOrderViewModel model)
        {
            try
            {
                var orders = GetCurrentOrder();

                var item = orders.FirstOrDefault(x => x.MenuItem.MenuItemId == menuItemId);

                if (item != null)
                {
                    item.Comment = comment;
                }

                SaveCurrentOrder(orders);

                return RedirectToAction(
                    nameof(ViewMenuForTakeOrder),
                    new
                    {
                        model.MenuFilterData,
                        model.SelectedTableId
                    });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;

                return RedirectToAction(
                    nameof(ViewMenuForTakeOrder),
                    new
                    {
                        model.MenuFilterData,
                        model.SelectedTableId
                    });
            }
        }

        [HttpPost]
        public IActionResult CancelOrder(TakeOrderViewModel model)
        {
            try
            {
                HttpContext.Session.Remove(CurrentOrderSessionKey);

                TempData["SuccessMessage"] = "Order cancelled successfully.";

                return RedirectToAction(
                    nameof(ViewMenuForTakeOrder),
                    new
                    {
                        model.SelectedTableId
                    });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;

                return RedirectToAction(
                    nameof(ViewMenuForTakeOrder),
                    new
                    {
                        model.SelectedTableId
                    });
            }
        }
    }
}
