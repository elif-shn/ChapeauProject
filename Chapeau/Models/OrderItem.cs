namespace Chapeau.Models
{
        public class OrderItem
        {
            public int OrderItemId { get; set; }

            public int OrderId { get; set; }

            public int MenuItemId { get; set; }

            public int OrderItemQuantity { get; set; }

            public string Comment { get; set; }

            public string OrderItemsStatus { get; set; }
        }
    }

