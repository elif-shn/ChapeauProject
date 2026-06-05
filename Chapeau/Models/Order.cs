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
        public User Employee { get; set; }
        public DateTime OrderTime { get; set; }
        public DateTime? ServedTime { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public List<OrderItem> OrderItems { get; set; }
        public string WaitingTime
        {
            get
            {
                DateTime endTime = ServedTime ?? DateTime.Now;

                int minutes = (int)(endTime - OrderTime).TotalMinutes;

                return $"{minutes} min";
            }
        }
        public Order() { }

        public Order(int orderId, int tableId, Table table, User employee, DateTime orderTime, DateTime? servedTime, OrderStatus orderStatus)
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

        /*public decimal GetTotalAmount()
        {
            if (OrderItems == null) return 0;
            return OrderItems.Sum(item => item.Price * item.OrderItemQuantity);
        }*/
    }
}