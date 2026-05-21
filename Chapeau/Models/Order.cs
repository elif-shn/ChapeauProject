using Chapeau.Extensions;
namespace Chapeau.Models
{
    public class Order
    {
        public int OrderId { get; set; }

        public Table Table { get; set; }

        public User Employee { get; set; }

        public DateTime OrderTime { get; set; }

        public DateTime? ServedTime { get; set; }

        public string OrderStatus { get; set; }
        public Order()
        {
        }
        public Order(int orderId, Table table, User employee, DateTime orderTime, DateTime? servedTime, string orderStatus)
        {
            OrderId = orderId;
            Table = table;
            Employee = employee;
            OrderTime = orderTime;
            ServedTime = servedTime;
            OrderStatus = orderStatus;
        }

    }
}


