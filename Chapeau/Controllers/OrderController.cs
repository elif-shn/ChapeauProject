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
        [Authorize(Roles = "Waiter")]
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
        [Authorize(Roles = "Waiter")]
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
        [Authorize(Roles = "Waiter")]
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
