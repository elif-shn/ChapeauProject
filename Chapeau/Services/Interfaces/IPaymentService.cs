using Chapeau.ViewModels;

namespace Chapeau.Services.Interfaces
{
    public interface IPaymentService
    {
        PaymentViewModel GetDashboard();

        PaymentViewModel LoadBill(int orderId);

        void FinishPayment(
            int orderId,
            decimal tipAmount,
            string paymentMethod,
            string feedback);
    }
}