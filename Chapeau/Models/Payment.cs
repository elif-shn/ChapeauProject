using Chapeau.Enums;

namespace Chapeau.Models
{
    public class Payment
    {
        public int PaymentId { get; set; }
        public int OrderId { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TipAmount { get; set; }
        public decimal Vat9 { get; set; }
        public decimal Vat21 { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public string Feedback { get; set; }
        public DateTime PaymentDate { get; set; }
    }
}