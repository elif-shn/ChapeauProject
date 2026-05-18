using System.Collections.Generic;
using Chapeau.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Chapeau.ViewModels
{
    public class PaymentViewModel
    {
        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public int OrderId { get; set; }
        public int TableId { get; set; }
        public decimal SubTotal { get; set; }
        public decimal HighVat { get; set; }
        public decimal LowVat { get; set; }
        public decimal TotalAmount { get; set; }
        public List<SelectListItem> ActiveOrders { get; set; } = new List<SelectListItem>();
        public int NumberOfSplits { get; set; } = 1;
        public decimal AmountToPayNow { get; set; }
    }
}