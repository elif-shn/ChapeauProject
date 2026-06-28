using System;
using System.Collections.Generic;
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

        public Order()
        {
            OrderItems = new List<OrderItem>();
        }

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

        // Method to update the order status based on the statuses of its order items
        public void UpdateOrderStatus()
        {
            bool allReady = true;
            bool anyPreparing = false;

            foreach (OrderItem item in OrderItems)
            {
                if (item.OrderItemStatus != OrderItemStatus.Ready)
                {
                    allReady = false;
                }

                if (item.OrderItemStatus == OrderItemStatus.Preparing)
                {
                    anyPreparing = true;
                }
            }

            if (allReady)
            {
                OrderStatus = OrderStatus.Ready;
            }
            else if (anyPreparing)
            {
                OrderStatus = OrderStatus.Preparing;
            }
            else
            {
                OrderStatus = OrderStatus.Ordered;
            }
        }
    }
}