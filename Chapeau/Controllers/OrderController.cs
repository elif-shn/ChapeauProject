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
            List<OrderItem> orderItems = _orderServices.GetOrderItemsByOrderId(orderId);

            return View(orderItems);
        }
        [HttpPost]
        public IActionResult UpdateStatus(int orderId,   OrderStatus status)
        {
           _orderServices.UpdateOrderStatus(orderId,status);

            return RedirectToAction("Index");
        }
    }
}
