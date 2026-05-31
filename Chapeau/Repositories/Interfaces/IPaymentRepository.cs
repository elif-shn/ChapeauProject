using Chapeau.Models;

namespace Chapeau.Repositories.Interfaces
{
    public interface IPaymentRepository
    {
        List<Table> GetAllTables();
        Order? GetActiveOrderByTableId(int tableId);
        List<OrderItem> GetOrderItemsByOrderId(int orderId);
        void SavePayment(Payment payment);
        void UpdateOrderStatusToPaid(int orderId);
        void UpdateTableStatusToFree(int tableId);
    }
}