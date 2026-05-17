
namespace Chapeau.Models
{
        public class Order
        {
            public int OrderId { get; set; }
            public int TableId { get; set; }
            public int EmployeeId { get; set; }
            public DateTime OrderTime { get; set; }
            public DateTime? ServedTime { get; set; }
            public string OrderStatus { get; set; }
        public Order(int orderId, int tableId, int employeeId, DateTime orderTime, DateTime? servedTime, string orderStatus)
        {
            OrderId = orderId;
            TableId = tableId;
            EmployeeId = employeeId;
            OrderTime = orderTime;
            ServedTime = servedTime;
            OrderStatus = orderStatus;


        }

        }
}


