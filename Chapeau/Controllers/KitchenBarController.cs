using Chapeau.Enums;
using Chapeau.Extensions;
using Chapeau.Models;
using Chapeau.Services;
using Chapeau.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chapeau.Controllers
//login as a chef with username: Emma Johnson, password: 12345
//login as a bartender with username: Bissy, password: 12345
{
    [Authorize]
    public class KitchenBarController : Controller
    {
        private readonly IOrderService _orderServices;

        public KitchenBarController(IOrderService orderServices)
        {
            _orderServices = orderServices;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "Chef")]
        public IActionResult GetRunningKitchenOrders()
        {
            try
            {
                ViewBag.FinishedAction = "GetFinishedKitchenOrders";
                ViewBag.IsFood = true;
                List<Order> orders = _orderServices.GetRunningOrders(true);
                return View("GetRunningOrders", orders);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Unable to load running kitchen orders.";
                return View("GetRunningOrders", new List<Order>());
            }
        }

        [HttpGet]
        [Authorize(Roles = "Bartender")]
        public IActionResult GetRunningBarOrders()
        {
            try
            {
                ViewBag.FinishedAction = "GetFinishedBarOrders";
                ViewBag.IsFood = false;
                List<Order> orders = _orderServices.GetRunningOrders(false);
                return View("GetRunningOrders", orders);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Unable to load running bar orders.";
                return View("GetRunningOrders", new List<Order>());
            }
        }

        [HttpGet]
        [Authorize(Roles = "Chef")]
        public IActionResult GetFinishedKitchenOrders()
        {
            try
            {
                ViewBag.RunningAction = "GetRunningKitchenOrders";
                List<Order> orders = _orderServices.GetFinishedOrders(true);
                return View("GetFinishedOrders", orders);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Unable to load finished kitchen orders.";
                return View("GetFinishedOrders", new List<Order>());
            }
        }

        [HttpGet]
        [Authorize(Roles = "Bartender")]
        public IActionResult GetFinishedBarOrders()
        {
            try
            {
                ViewBag.RunningAction = "GetRunningBarOrders";
                List<Order> orders = _orderServices.GetFinishedOrders(false);
                return View("GetFinishedOrders", orders);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Unable to load finished bar orders.";
                return View("GetFinishedOrders", new List<Order>());
            }
        }

        [HttpPost]
        public IActionResult UpdateOrderStatus(Order order, bool isFood)
        {
            try
            {
                _orderServices.UpdateOrderStatus(order, isFood);
                TempData["SuccessMessage"] = "Order status updated successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Unable to update order status.";
            }

            if (isFood)
                return RedirectToAction("GetRunningKitchenOrders");
            else
                return RedirectToAction("GetRunningBarOrders");
        }

        [HttpPost]
        public IActionResult UpdateOrderItemStatus(OrderItem orderItem, Order order, bool isFood)
        {
            try
            {
                _orderServices.UpdateOrderItemStatus(orderItem, order, isFood);
                TempData["SuccessMessage"] = "Order item status updated successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.InnerException?.Message ?? ex.Message;
            }

            if (isFood)
                return RedirectToAction("GetRunningKitchenOrders");
            else
                return RedirectToAction("GetRunningBarOrders");
        }

        [HttpPost]
        public IActionResult UpdateCourseStatus(Order order, Category category, OrderItemStatus status)
        {
            try
            {
                _orderServices.UpdateCourseStatus(order, category, status, true);
                TempData["SuccessMessage"] = "Course status updated successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Unable to update course status.";
            }

            return RedirectToAction("GetRunningKitchenOrders");
        }
    }
}