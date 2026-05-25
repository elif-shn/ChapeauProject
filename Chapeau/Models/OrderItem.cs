using Chapeau.Extensions;
using Chapeau.Enums;
namespace Chapeau.Models
{
    public class OrderItem
    {
        public int OrderItemId { get; set; }

        public Order Order { get; set; }

        public MenuItem MenuItem { get; set; }

        public int OrderItemQuantity { get; set; }

        public string Comment { get; set; }

        public OrderStatus OrderItemStatus { get; set; }
        public OrderItem()
        {
        }
        public OrderItem(int orderItemId, Order order, MenuItem menuItem, int orderItemQuantity, string comment, OrderStatus orderItemStatus)
        {
            OrderItemId = orderItemId;
            Order = order;
            MenuItem = menuItem;
            OrderItemQuantity = orderItemQuantity;
            Comment = comment;
            OrderItemStatus = orderItemStatus;
        }
    }
}
