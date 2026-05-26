using Chapeau.Models;

namespace Chapeau.Repositories
{
    public interface IPaymentRepository
    {
        void AddPayment(Payment payment);
    }
}