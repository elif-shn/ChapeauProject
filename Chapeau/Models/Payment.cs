using System;
using Chapeau.Enums;

namespace Chapeau.Models
{
    public class Payment
    {
        public int PaymentID { get; set; }
        public Order? Order { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal HighVatAmount { get; set; }
        public decimal LowVatAmount { get; set; }
        public decimal TipAmount { get; set; }
        public PaymentMethod Method { get; set; }
        public string? Feedback { get; set; }
        public DateTime PaymentTime { get; set; }
    }
}