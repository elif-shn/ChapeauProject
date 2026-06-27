using Chapeau.Models;

namespace Chapeau.ViewModels
{
    public class SplitPaymentViewModel
    {
        public int TableId { get; set; }

        public int OrderId { get; set; }

        public decimal TotalBillAmount { get; set; }

        public decimal TotalPaidAmount { get; set; }

        public decimal RemainingAmount { get; set; }

        public int NumberOfPeople { get; set; }

        public bool IsEqualSplit { get; set; }

        public List<OrderItem> OrderItems { get; set; }

        public List<SplitPaymentPersonViewModel> Payments { get; set; }

        public string ErrorMessage { get; set; }

        public SplitPaymentViewModel()
        {
            OrderItems = new List<OrderItem>();
            Payments = new List<SplitPaymentPersonViewModel>();

            ErrorMessage = string.Empty;

            NumberOfPeople = 1;
        }
    }
}