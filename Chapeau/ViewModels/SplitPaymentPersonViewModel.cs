using Chapeau.Enums;

namespace Chapeau.ViewModels
{
    public class SplitPaymentPersonViewModel
    {
        public decimal AmountToPay { get; set; }

        public decimal TipAmount { get; set; }

        public PaymentMethod PaymentMethod { get; set; }

        public string Feedback { get; set; }

        public SplitPaymentPersonViewModel()
        {
            Feedback = string.Empty;
        }
    }
}