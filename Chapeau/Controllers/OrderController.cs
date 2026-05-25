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
            List<Order> orders = _orderServices.GetAllOrders();

            return View(orders);
        }
        public IActionResult OrderItems(int orderId)
        {
            try
            {
                List<OrderItem> orderItems =
                    _orderServices.GetOrderItemsByOrderId(orderId);

            return View(orderItems);

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
    }
}

