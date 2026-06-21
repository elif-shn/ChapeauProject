using Chapeau.Enums;
using Chapeau.Extensions;
using Chapeau.Models;
using Chapeau.Services;
using Chapeau.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chapeau.Controllers
{
     [Authorize(Roles = "Kitchen,Bar")]
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
        public IActionResult UpdateCourseStatus(Order order, Category category, OrderItemStatus status)
        {
            try
            {
                _orderServices.UpdateCourseStatus(order, category, status);

                TempData["SuccessMessage"] = "Course status updated successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction("GetRunningKitchenOrders");
        }
       
    }
}
