using Chapeau.Models;
using Chapeau.Repositories.Interfaces;
using Chapeau.Services.Interfaces;
using Chapeau.ViewModels;

namespace Chapeau.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;

        public PaymentService(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        public PaymentViewModel GetDashboard()
        {
            PaymentViewModel viewModel = new PaymentViewModel();

            viewModel.Tables = _paymentRepository.GetAllTables();

            return viewModel;
        }

        public PaymentViewModel GetBillByTableId(int tableId)
        {
            PaymentViewModel viewModel = GetDashboard();

            Order? order = _paymentRepository.GetActiveOrderByTableId(tableId);

            if (order == null)
            {
                viewModel.TableId = tableId;
                viewModel.ErrorMessage = "No active order found for this table.";
                return viewModel;
            }

            List<OrderItem> orderItems = _paymentRepository.GetOrderItemsByOrderId(order.OrderId);

            viewModel.TableId = tableId;
            viewModel.OrderId = order.OrderId;
            viewModel.OrderItems = orderItems;

            CalculateBill(viewModel);

            return viewModel;
        }

        public void ConfirmPayment(PaymentViewModel viewModel)
        {
            CalculateBill(viewModel);

            Payment payment = new Payment();

            payment.Order = new Order { OrderId = viewModel.OrderId };
            payment.TotalAmount = viewModel.TotalAmount;
            payment.TipAmount = viewModel.TipAmount;
            payment.Vat9 = viewModel.Vat9;
            payment.Vat21 = viewModel.Vat21;
            payment.PaymentMethod = viewModel.PaymentMethod;
            payment.Feedback = viewModel.Feedback ?? "";
            payment.PaymentDate = DateTime.Now;

            _paymentRepository.SavePayment(payment);
            _paymentRepository.UpdateOrderStatusToPaid(viewModel.OrderId);
            _paymentRepository.UpdateTableStatusToFree(viewModel.TableId);
        }

        private void CalculateBill(PaymentViewModel viewModel)
        {
            decimal subTotal = 0;
            decimal vat9 = 0;
            decimal vat21 = 0;

            foreach (OrderItem item in viewModel.OrderItems)
            {
                decimal itemTotal = item.MenuItem.MenuItemPrice * item.OrderItemQuantity;

                subTotal += itemTotal;

                if (item.MenuItem.VatPercentage == 9)
                {
                    vat9 += itemTotal - (itemTotal / 1.09m);
                }

                if (item.MenuItem.VatPercentage == 21)
                {
                    vat21 += itemTotal - (itemTotal / 1.21m);
                }
            }

            viewModel.SubTotal = Math.Round(subTotal, 2);
            viewModel.Vat9 = Math.Round(vat9, 2);
            viewModel.Vat21 = Math.Round(vat21, 2);
            viewModel.TotalAmount = Math.Round(subTotal + viewModel.TipAmount, 2);
        }
    }
}