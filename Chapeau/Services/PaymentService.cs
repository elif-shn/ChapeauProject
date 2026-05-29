using Chapeau.Models;
using Chapeau.Repositories.Interfaces;
using Chapeau.Services.Interfaces;
using Chapeau.ViewModels;

namespace Chapeau.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IOrderRepository _orderRepository;

        public PaymentService(IPaymentRepository paymentRepository, IOrderRepository orderRepository)
        {
            _paymentRepository = paymentRepository;
            _orderRepository = orderRepository;
        }

        public void ProcessPayment(Payment payment)
        {
            _paymentRepository.AddPayment(payment);
        }

        public PaymentSummaryViewModel GetOrderSummary(int orderId)
        {
            var items = _orderRepository.GetOrderItemsByOrderId(orderId);
            decimal total = 0;
            decimal vat9 = 0;
            decimal vat21 = 0;

            foreach (var item in items)
            {
                decimal price = item.MenuItem.MenuItemPrice;
                int vatRate = item.MenuItem.VatPercentage;
                decimal itemTotal = price * item.OrderItemQuantity;

                total += itemTotal;

                if (vatRate == 9) vat9 += (itemTotal * 0.09m);
                else if (vatRate == 21) vat21 += (itemTotal * 0.21m);
            }

            return new PaymentSummaryViewModel
            {
                OrderId = orderId,
                OrderItems = items,
                TotalAmount = total,
                Vat9 = vat9,
                Vat21 = vat21,
                GrandTotal = total + vat9 + vat21
            };
        }
    }
}