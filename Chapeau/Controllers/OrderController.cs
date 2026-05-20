using Chapeau.Models;
using Chapeau.Services;
using Chapeau.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Chapeau.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService _orderServices;

        private readonly IOrderItemService _orderItemService;

        public OrderController(IOrderService orderServices, IOrderItemService orderItemService)
        {
            _orderServices = orderServices;

            _orderItemService = orderItemService;
        }

        public IActionResult Index()
        {
            List<RunningOrderViewModel> runningOrdersViewModel = _orderServices.GetRunningOrders();

            return View(runningOrdersViewModel);
        }

        public IActionResult RunningOrders()
        {
            List<RunningOrderViewModel> runningOrdersViewModel = _orderServices.GetRunningOrders();

            return View(runningOrdersViewModel);
        }

        public IActionResult ViewOrderItems(int id)
        {
            List<OrderItem> items = _orderItemService.GetOrderItemsByOrderId(id);

            return View("OrderItem", items);
        }
    }
}






















