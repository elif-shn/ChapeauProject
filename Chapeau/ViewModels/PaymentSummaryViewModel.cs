using Chapeau.Models;
using System.Collections.Generic;

namespace Chapeau.ViewModels
{
    public class PaymentSummaryViewModel
    {
        public int OrderId { get; set; }
        public List<OrderItem> OrderItems { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TipAmount { get; set; }
        public decimal Vat9 { get; set; }
        public decimal Vat21 { get; set; }
        public decimal GrandTotal { get; set; }
    }
}