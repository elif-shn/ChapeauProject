using Chapeau.Enums;
using Chapeau.Models;
using Chapeau.Repositories.Interfaces;
using Chapeau.Services.Interfaces;
using Chapeau.ViewModels;

namespace Chapeau.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IOrderService _orderService;
        private readonly IPaymentRepository _paymentRepository;

        public PaymentService(
            IOrderService orderService,
            IPaymentRepository paymentRepository)
        {
            _orderService = orderService;
            _paymentRepository = paymentRepository;
        }

        public PaymentViewModel GetDashboard()
        {
            PaymentViewModel vm =
                new PaymentViewModel();

            vm.Orders =
                _orderService.GetAllOrders();

            return vm;
        }

        public PaymentViewModel LoadBill(int orderId)
        {
            PaymentViewModel vm =
                new PaymentViewModel();

            vm.Orders =
                _orderService.GetAllOrders();

            vm.SelectedOrderId =
                orderId;

            vm.OrderItems =
                _orderService
                .GetOrderItemsByOrderId(orderId);

            decimal total = 0;
            decimal vat9 = 0;
            decimal vat21 = 0;

            foreach (var item in vm.OrderItems)
            {
                decimal amount =
                    item.MenuItem.MenuItemPrice
                    * item.OrderItemQuantity;

                total += amount;

                if (item.MenuItem.VatPercentage == 9)
                {
                    vat9 += amount * 9 / 109;
                }

                if (item.MenuItem.VatPercentage == 21)
                {
                    vat21 += amount * 21 / 121;
                }
            }

            vm.TotalAmount = total;
            vm.Vat9 = vat9;
            vm.Vat21 = vat21;

            return vm;
        }

        public void FinishPayment(
            int orderId,
            decimal tipAmount,
            string paymentMethod,
            string feedback)
        {
            PaymentViewModel vm =
                LoadBill(orderId);

            Payment payment =
                new Payment();

            payment.OrderId =
                orderId;

            payment.TotalAmount =
                vm.TotalAmount;

            payment.TipAmount =
                tipAmount;

            payment.Vat9 =
                vm.Vat9;

            payment.Vat21 =
                vm.Vat21;

            payment.PaymentMethod =
                Enum.Parse<PaymentMethod>(
                    paymentMethod);

            payment.Feedback =
                feedback;

            payment.PaymentDate =
                DateTime.Now;

            _paymentRepository
                .SavePayment(payment);

            _orderService
                .UpdateOrderStatus(
                    orderId,
                    OrderStatus.Completed);
        }
    }
}