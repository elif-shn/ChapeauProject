using Chapeau.Models;
using Chapeau.ViewModels;
using Chapeau.Enums;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Chapeau.Services.Interfaces;

namespace Chapeau.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IPaymentService _paymentService;
        private readonly IOrderService _orderService;

        public PaymentController(IPaymentService paymentService, IOrderService orderService)
        {
            _paymentService = paymentService;
            _orderService = orderService;
        }

        public IActionResult Index()
        {
            List<Order> orders = _orderService.GetAllOrders();
            return View(orders);
        }

        public JsonResult GetOrdersJson()
        {
            var orders = _orderService.GetAllOrders();
            var orderList = orders.Select(o => new { orderId = o.OrderId }).ToList();
            return Json(orderList);
        }

        public IActionResult Summary(int orderId)
        {
            return View(_paymentService.GetOrderSummary(orderId));
        }

        [HttpPost]
        public IActionResult Create(PaymentViewModel vm)
        {
            _paymentService.ProcessPayment(new Payment
            {
                OrderId = vm.OrderId,
                TotalAmount = vm.TotalAmount,
                TipAmount = vm.TipAmount,
                Vat9 = vm.Vat9,
                Vat21 = vm.Vat21,
                PaymentMethod = vm.PaymentMethod,
                Feedback = vm.Feedback,
                PaymentDate = DateTime.Now
            });

            _orderService.UpdateOrderStatus(vm.OrderId, OrderStatus.Settled);

            return RedirectToAction("Success");
        }

        public IActionResult Success()
        {
            return View();
        }
    }
}