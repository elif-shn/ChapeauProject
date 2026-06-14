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
        public IActionResult AddItemToCurrentOrder(TakeOrderViewModel model)
        {
            try
            {
                List<OrderItem> currentOrder = GetCurrentOrder();

                _orderServices.AddItemToCurrentOrder(currentOrder, model.NewOrderItem.MenuItem.MenuItemId, model.NewOrderItem.Comment);

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

                _orderServices.SendOrder(order);

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
        public IActionResult RemoveItemInCurrentOrder(TakeOrderViewModel model)
        {
            try
            {
                List<OrderItem> currentOrder = GetCurrentOrder();

                _orderServices.DecreaseItemQuantityInCurrentOrder(currentOrder,model.NewOrderItem.MenuItem.MenuItemId,model.NewOrderItem.Comment);

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
        public IActionResult DeleteItem(TakeOrderViewModel model)
        {
            try
            {
                List<OrderItem> currentOrder = GetCurrentOrder();

                _orderServices.DeleteItem(currentOrder, model.NewOrderItem.MenuItem.MenuItemId, model.NewOrderItem.Comment);

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
        public IActionResult AddNote(TakeOrderViewModel model)
        {
            try
            {
                List<OrderItem> currentOrder = GetCurrentOrder();

                _orderServices.AddNote(currentOrder, model.NewOrderItem.MenuItem.MenuItemId, model.NewOrderItem.Comment);

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
