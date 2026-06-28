using Chapeau.Services.Interfaces;
using Chapeau.ViewModels;
using Microsoft.AspNetCore.Authorization;
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

        [Authorize(Roles = "Waiter,Manager")]
        public IActionResult Index()
        {
            PaymentViewModel viewModel = _paymentService.GetDashboard();

            return View(viewModel);
        }

        [Authorize(Roles = "Waiter,Manager")]
        public IActionResult LoadBill(int tableId)
        {
            PaymentViewModel viewModel = _paymentService.GetBillByTableId(tableId);

            return View("Index", viewModel);
        }

        [HttpPost]
        [Authorize(Roles = "Waiter,Manager")]
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

        [Authorize(Roles = "Waiter,Manager")]
        public IActionResult SplitEqual(int tableId)
        {
            SplitPaymentViewModel viewModel = _paymentService.GetSplitPaymentByTableId(tableId);
            viewModel.IsEqualSplit = true;

            return View(viewModel);
        }

        [HttpPost]
        [Authorize(Roles = "Waiter,Manager")]
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

        [Authorize(Roles = "Waiter,Manager")]
        public IActionResult SplitDifferent(int tableId, int numberOfPeople = 1)
        {
            SplitPaymentViewModel viewModel = _paymentService.GetSplitPaymentByTableId(tableId);
            viewModel.IsEqualSplit = false;
            viewModel.NumberOfPeople = numberOfPeople;

            viewModel.Payments.Clear();

            for (int i = 0; i < numberOfPeople; i++)
            {
                viewModel.Payments.Add(new SplitPaymentPersonViewModel());
            }

            return View(viewModel);
        }

        [HttpPost]
        [Authorize(Roles = "Waiter,Manager")]
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

        [Authorize(Roles = "Waiter,Manager")]
        public IActionResult Success()
        {
            return View();
        }
    }
}