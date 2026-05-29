using Chapeau.Models;

namespace Chapeau.ViewModels
{
    public class PaymentViewModel
    {
        public List<Order> Orders { get; set; }

        public int SelectedOrderId { get; set; }

        public List<OrderItem> OrderItems { get; set; }

        public decimal TotalAmount { get; set; }

        public decimal Vat9 { get; set; }

        public decimal Vat21 { get; set; }

        public decimal TipAmount { get; set; }

        public string PaymentMethod { get; set; }

        public string Feedback { get; set; }
    }
}