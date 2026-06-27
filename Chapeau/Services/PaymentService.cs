using Chapeau.Enums;
using Chapeau.Models;
using Chapeau.Repositories.Interfaces;
using Chapeau.Services.Interfaces;
using Chapeau.ViewModels;

namespace Chapeau.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly ITableRepository _tableRepository;

        public PaymentService(IPaymentRepository paymentRepository, ITableRepository tableRepository)
        {
            _paymentRepository = paymentRepository;
            _tableRepository = tableRepository;
        }

        public PaymentViewModel GetDashboard()
        {
            PaymentViewModel viewModel = new PaymentViewModel();

            viewModel.Tables = _tableRepository.GetAllTables();

            return viewModel;
        }

        public PaymentViewModel GetBillByTableId(int tableId)
        {
            PaymentViewModel viewModel = GetDashboard();

            Table? selectedTable = viewModel.Tables.FirstOrDefault(t => t.TableId == tableId);

            if (selectedTable == null)
            {
                viewModel.TableId = tableId;
                viewModel.ErrorMessage = "Table not found.";

                return viewModel;
            }

            if (selectedTable.TableStatus == TableStatus.Free)
            {
                viewModel.TableId = tableId;
                viewModel.ErrorMessage = "This table is free. The bill has already been paid or there is no active order.";

                return viewModel;
            }

            Order? order = _paymentRepository.GetActiveOrderByTableId(tableId);

            if (order == null)
            {
                viewModel.TableId = tableId;
                viewModel.ErrorMessage = "No active order found for this table.";

                return viewModel;
            }

            viewModel.TableId = tableId;
            viewModel.OrderId = order.OrderId;
            viewModel.OrderItems = _paymentRepository.GetOrderItemsByOrderId(order.OrderId);

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
            payment.Feedback = viewModel.Feedback ?? string.Empty;
            payment.PaymentDate = DateTime.Now;

            _paymentRepository.SavePayment(payment);
            _paymentRepository.UpdateOrderStatusToPaid(viewModel.OrderId);

            Table table = new Table();
            table.TableId = viewModel.TableId;
            table.TableStatus = TableStatus.Free;

            _tableRepository.UpdateTableStatus(table);
        }

        public SplitPaymentViewModel GetSplitPaymentByTableId(int tableId)
        {
            PaymentViewModel paymentViewModel = GetBillByTableId(tableId);

            decimal totalPaid = _paymentRepository.GetTotalPaidByOrderId(paymentViewModel.OrderId);
            decimal remainingAmount = Math.Round(paymentViewModel.SubTotal - totalPaid, 2);

            SplitPaymentViewModel splitViewModel = new SplitPaymentViewModel();

            splitViewModel.TableId = paymentViewModel.TableId;
            splitViewModel.OrderId = paymentViewModel.OrderId;
            splitViewModel.OrderItems = paymentViewModel.OrderItems;
            splitViewModel.TotalBillAmount = paymentViewModel.SubTotal;
            splitViewModel.TotalPaidAmount = Math.Round(totalPaid, 2);
            splitViewModel.RemainingAmount = remainingAmount;

            if (splitViewModel.RemainingAmount < 0)
            {
                splitViewModel.RemainingAmount = 0;
            }

            for (int i = 0; i < 4; i++)
            {
                splitViewModel.Payments.Add(new SplitPaymentPersonViewModel());
            }

            return splitViewModel;
        }

        public void ConfirmSplitEqualPayment(SplitPaymentViewModel viewModel)
        {
            if (viewModel.NumberOfPeople <= 0)
            {
                throw new Exception("Number of people must be higher than 0.");
            }

            SplitPaymentViewModel currentBill = GetSplitPaymentByTableId(viewModel.TableId);

            if (currentBill.RemainingAmount <= 0)
            {
                throw new Exception("This order is already fully paid.");
            }

            PaymentViewModel fullBill = GetBillByTableId(viewModel.TableId);

            decimal amountPerPerson = Math.Round(currentBill.RemainingAmount / viewModel.NumberOfPeople, 2);
            decimal totalBaseAmount = 0;

            for (int i = 0; i < viewModel.NumberOfPeople; i++)
            {
                decimal baseAmount = amountPerPerson;

                if (i == viewModel.NumberOfPeople - 1)
                {
                    baseAmount = currentBill.RemainingAmount - totalBaseAmount;
                }

                totalBaseAmount += baseAmount;

                SplitPaymentPersonViewModel person = new SplitPaymentPersonViewModel();

                if (viewModel.Payments != null && i < viewModel.Payments.Count)
                {
                    person = viewModel.Payments[i];
                }

                Payment payment = new Payment();

                payment.Order = new Order { OrderId = currentBill.OrderId };
                payment.TotalAmount = Math.Round(baseAmount + person.TipAmount, 2);
                payment.TipAmount = Math.Round(person.TipAmount, 2);
                payment.Vat9 = CalculatePartialVat(fullBill.Vat9, baseAmount, fullBill.SubTotal);
                payment.Vat21 = CalculatePartialVat(fullBill.Vat21, baseAmount, fullBill.SubTotal);
                payment.PaymentMethod = person.PaymentMethod;
                payment.Feedback = person.Feedback ?? string.Empty;
                payment.PaymentDate = DateTime.Now;

                _paymentRepository.SavePayment(payment);
            }

            FinishOrderIfFullyPaid(currentBill.OrderId, currentBill.TableId, currentBill.TotalBillAmount);
        }

        public void ConfirmSplitDifferentPayment(SplitPaymentViewModel viewModel)
        {
            SplitPaymentViewModel currentBill = GetSplitPaymentByTableId(viewModel.TableId);

            if (currentBill.RemainingAmount <= 0)
            {
                throw new Exception("This order is already fully paid.");
            }

            PaymentViewModel fullBill = GetBillByTableId(viewModel.TableId);

            foreach (SplitPaymentPersonViewModel person in viewModel.Payments)
            {
                if (person.AmountToPay <= 0 && person.TipAmount <= 0)
                {
                    continue;
                }

                Payment payment = new Payment();

                payment.Order = new Order { OrderId = currentBill.OrderId };
                payment.TotalAmount = Math.Round(person.AmountToPay + person.TipAmount, 2);
                payment.TipAmount = Math.Round(person.TipAmount, 2);
                payment.Vat9 = CalculatePartialVat(fullBill.Vat9, person.AmountToPay, fullBill.SubTotal);
                payment.Vat21 = CalculatePartialVat(fullBill.Vat21, person.AmountToPay, fullBill.SubTotal);
                payment.PaymentMethod = person.PaymentMethod;
                payment.Feedback = person.Feedback ?? string.Empty;
                payment.PaymentDate = DateTime.Now;

                _paymentRepository.SavePayment(payment);
            }

            FinishOrderIfFullyPaid(currentBill.OrderId, currentBill.TableId, currentBill.TotalBillAmount);
        }

        private void FinishOrderIfFullyPaid(int orderId, int tableId, decimal totalBillAmount)
        {
            decimal totalPaid = _paymentRepository.GetTotalPaidByOrderId(orderId);

            if (totalPaid >= totalBillAmount)
            {
                _paymentRepository.UpdateOrderStatusToPaid(orderId);

                Table table = new Table();
                table.TableId = tableId;
                table.TableStatus = TableStatus.Free;

                _tableRepository.UpdateTableStatus(table);
            }
        }

        private decimal CalculatePartialVat(decimal fullVatAmount, decimal paymentAmount, decimal fullBillAmount)
        {
            if (fullBillAmount <= 0)
            {
                return 0;
            }

            decimal percentage = paymentAmount / fullBillAmount;

            return Math.Round(fullVatAmount * percentage, 2);
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