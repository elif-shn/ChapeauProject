using Chapeau.Models;
using Chapeau.ViewModels;

namespace Chapeau.Services.Interfaces
{
    public interface IPaymentService
    {
        void ProcessPayment(Payment payment);
        PaymentSummaryViewModel GetOrderSummary(int orderId);
    }
}