using Chapeau.Enums;
using Chapeau.Models;

namespace Chapeau.ViewModels
{
    public class PaymentViewModel
    {
        public List<Table> Tables { get; set; }
        public List<OrderItem> OrderItems { get; set; }

        public int TableId { get; set; }
        public int OrderId { get; set; }

        public decimal SubTotal { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TipAmount { get; set; }
        public decimal Vat9 { get; set; }
        public decimal Vat21 { get; set; }

        public PaymentMethod PaymentMethod { get; set; }
        public string Feedback { get; set; }
        public string ErrorMessage { get; set; }

        public PaymentViewModel()
        {
            Tables = new List<Table>();
            OrderItems = new List<OrderItem>();
            PaymentMethod = PaymentMethod.Cash;
            Feedback = "";
            ErrorMessage = "";
        }
    }
}