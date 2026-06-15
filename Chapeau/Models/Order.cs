using System;
using System.Collections.Generic;
using System.Linq;
using Chapeau.Enums;

namespace Chapeau.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public int TableId { get; set; }
        public Table Table { get; set; }
        public Employee Employee { get; set; }
        public DateTime OrderTime { get; set; }
        public DateTime? ServedTime { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public List<OrderItem> OrderItems { get; set; }
        public TimeSpan WaitingTime
        {
            get { return DateTime.Now - OrderTime; }
        }
        public Order() { }

        public Order(int orderId, int tableId, Table table, Employee employee, DateTime orderTime, DateTime? servedTime, OrderStatus orderStatus)
        {
            OrderId = orderId;
            TableId = tableId;
            Table = table;
            Employee = employee;
            OrderTime = orderTime;
            ServedTime = servedTime;
            OrderStatus = orderStatus;
            OrderItems = new List<OrderItem>();
        }


    }
}