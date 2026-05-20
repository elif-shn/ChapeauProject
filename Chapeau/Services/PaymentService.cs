using System.Collections.Generic;
using Chapeau.Models;
using Chapeau.Repositories;

namespace Chapeau.Services
{
    public class PaymentService(IPaymentRepository paymentRepository)
    {
        private readonly IPaymentRepository _paymentRepository = paymentRepository;

        public List<OrderItem> GetOrderBillDetails(int orderId)
        {
            return _paymentRepository.GetOrderItemsByOrderId(orderId);
        }

        public void ProcessPayment(Payment payment)
        {
            _paymentRepository.InsertPayment(payment);
            _paymentRepository.UpdateTableStatusAfterPayment(payment.Order?.Table.TableId ?? 1, "Free");
        }
    }
}