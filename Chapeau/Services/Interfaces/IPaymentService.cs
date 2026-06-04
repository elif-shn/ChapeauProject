using Chapeau.ViewModels;

namespace Chapeau.Services.Interfaces
{
    public interface IPaymentService
    {
        PaymentViewModel GetDashboard();
        PaymentViewModel GetBillByTableId(int tableId);
        void ConfirmPayment(PaymentViewModel viewModel);
    }
}