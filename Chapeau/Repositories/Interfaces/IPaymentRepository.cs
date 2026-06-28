using Chapeau.Models;

namespace Chapeau.Repositories.Interfaces
{
    public interface IPaymentRepository
    {
        Order? GetActiveOrderByTableId(int tableId);

        List<OrderItem> GetOrderItemsByOrderId(int orderId);

        void SavePayment(Payment payment);

        decimal GetTotalPaidByOrderId(int orderId);

        void UpdateOrderStatusToPaid(int orderId);
    }
}