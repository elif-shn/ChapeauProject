using Chapeau.Services.Interfaces;
using Chapeau.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Chapeau.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        public IActionResult Index()
        {
            PaymentViewModel viewModel = _paymentService.GetDashboard();

            return View(viewModel);
        }

        public IActionResult LoadBill(int tableId)
        {
            PaymentViewModel viewModel = _paymentService.GetBillByTableId(tableId);

            return View("Index", viewModel);
        }

        [HttpPost]
        public IActionResult ConfirmPayment(PaymentViewModel viewModel)
        {
            try
            {
                viewModel.OrderItems = _paymentService.GetBillByTableId(viewModel.TableId).OrderItems;

                _paymentService.ConfirmPayment(viewModel);

                return RedirectToAction("Success");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;

                return RedirectToAction("Index");
            }
        }

        public IActionResult Success()
        {
            return View();
        }
    }
}