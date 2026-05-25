using Chapeau.Enums;
using Chapeau.Models;

public class Order
{
    public int OrderId { get; set; }
    public Table Table { get; set; }
    public User Employee { get; set; }
    public string WaitingTime { get; set; }
    public DateTime OrderTime { get; set; }
    public DateTime?ServedTime { get; set; }
    public OrderStatus OrderStatus { get; set; }
    public List<OrderItem> OrderItems { get; set; }

    public Order(){}
    public Order(int orderId, Table table, User employee, DateTime orderTime, DateTime? servedTime, OrderStatus orderStatus)
    {
        OrderId = orderId;
        Table = table;
        Employee = employee;
        OrderTime = orderTime;
        ServedTime = servedTime;
        OrderStatus = orderStatus;
        OrderItems = new List<OrderItem>();
    }


}