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
        public OrderItemStatus OrderItemStatus { get; set; }

        public OrderItem()
        {
        }

        public OrderItem(int orderItemId, Order order, MenuItem menuItem, int orderItemQuantity, string comment, OrderItemStatus orderItemStatus)
        {
            OrderItemId = orderItemId;
            Order = order;
            MenuItem = menuItem;
            OrderItemQuantity = orderItemQuantity;
            Comment = comment;
            OrderItemStatus = orderItemStatus;
        }

        public void Increase()
        {
            OrderItemQuantity++;
        }

        public void Decrease()
        {
            OrderItemQuantity--;
        }

        public void Prepare()
        {
            if (OrderItemStatus != OrderItemStatus.Ordered)
            {
                throw new InvalidOperationException("Only ordered items can be set to preparing.");
            }

            OrderItemStatus = OrderItemStatus.Preparing;
        }

        public void MarkAsReady()
        {
            if (OrderItemStatus != OrderItemStatus.Preparing)
            {
                throw new InvalidOperationException("Only preparing items can be marked as ready.");
            }

            OrderItemStatus = OrderItemStatus.Ready;
        }
    }
}