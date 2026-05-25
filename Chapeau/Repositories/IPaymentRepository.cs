using Chapeau.Models;
using System.Collections.Generic;

namespace Chapeau.Repositories
{
    public interface IPaymentRepository
    {
        List<OrderItem> Getbyid(int orderId);
        void InsertPayment(Payment payment);
        void UpdateTableStatusAfterPayment(int tableId, string status);
    }
}