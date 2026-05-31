using Chapeau.Enums;
using Chapeau.Models;
using Chapeau.Services;
using Chapeau.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Chapeau.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService _orderServices;

        public OrderController(IOrderService orderServices)
        {
            _orderServices = orderServices;
        }
        public IActionResult Index()
        {
            List<Order> orders = _orderServices.GetRunningOrders();

            return View(orders);
        }

        [HttpPost]
        public IActionResult UpdateOrderStatus(int orderId, OrderStatus status)
        {
            try
            {
                Order? order = _orderServices.GetOrderById(orderId);

                if (order != null)
                {
                    _orderServices.UpdateOrderStatus(order, status);
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index");
            }
        }
        [HttpPost]
        public IActionResult UpdateOrderItemStatus(OrderItem orderItem, OrderItemStatus status)
        {
            try
            {
                _orderServices.UpdateOrderItemStatus(orderItem, status);

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
    }
}

