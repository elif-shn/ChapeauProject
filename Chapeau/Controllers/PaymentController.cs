using Chapeau.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Chapeau.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(
            IPaymentService paymentService)
        {
            _paymentService =
                paymentService;
        }

        public IActionResult Index()
        {
            return View(
                _paymentService
                .GetDashboard());
        }

        [HttpPost]
        public IActionResult LoadBill(
            int selectedOrderId)
        {
            return View(
                "Index",
                _paymentService
                .LoadBill(selectedOrderId));
        }

        [HttpPost]
        public IActionResult Confirm(
            int selectedOrderId,
            decimal tipAmount,
            string paymentMethod,
            string feedback)
        {
            _paymentService
                .FinishPayment(
                    selectedOrderId,
                    tipAmount,
                    paymentMethod,
                    feedback);

            return RedirectToAction(
                "Success");
        }

        public IActionResult Success()
        {
            return View();
        }
    }
}