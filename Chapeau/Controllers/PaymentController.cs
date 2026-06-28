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
                PaymentViewModel billViewModel = _paymentService.GetBillByTableId(viewModel.TableId);

                billViewModel.TipAmount = viewModel.TipAmount;
                billViewModel.PaymentMethod = viewModel.PaymentMethod;
                billViewModel.Feedback = viewModel.Feedback ?? string.Empty;

                _paymentService.ConfirmPayment(billViewModel);

                return RedirectToAction(nameof(Success));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;

                return RedirectToAction(nameof(Index));
            }
        }

        public IActionResult SplitEqual(int tableId)
        {
            SplitPaymentViewModel viewModel = _paymentService.GetSplitPaymentByTableId(tableId);
            viewModel.IsEqualSplit = true;

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult SplitEqual(SplitPaymentViewModel viewModel)
        {
            try
            {
                _paymentService.ConfirmSplitEqualPayment(viewModel);

                return RedirectToAction(nameof(Success));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;

                return RedirectToAction(nameof(SplitEqual), new { tableId = viewModel.TableId });
            }
        }

        public IActionResult SplitDifferent(int tableId)
        {
            SplitPaymentViewModel viewModel = _paymentService.GetSplitPaymentByTableId(tableId);
            viewModel.IsEqualSplit = false;

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult SplitDifferent(SplitPaymentViewModel viewModel)
        {
            try
            {
                _paymentService.ConfirmSplitDifferentPayment(viewModel);

                return RedirectToAction(nameof(Success));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;

                return RedirectToAction(nameof(SplitDifferent), new { tableId = viewModel.TableId });
            }
        }

        public IActionResult Success()
        {
            return View();
        }
    }
}