using Chapeau.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace Chapeau.Repositories
{
    public class DbPaymentRepository : IPaymentRepository
    {
        private readonly string _connectionString;

        public DbPaymentRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        public List<OrderItem> GetOrderItemsByOrderId(int orderId)
        {
            var list = new List<OrderItem>();
            using var connection = new SqlConnection(_connectionString);
            string query = "SELECT MenuItemId, OrderItemQuantity, Comment, OrderItemsStatus FROM OrderItem WHERE OrderId = @OrderId";
            var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@OrderId", orderId);

            connection.Open();
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new OrderItem
                {
                    MenuItemId = (int)reader["MenuItemId"],
                    OrderItemQuantity = (int)reader["OrderItemQuantity"],
                    Comment = reader["Comment"].ToString()!,
                    OrderItemsStatus = reader["OrderItemsStatus"].ToString()!
                });
            }
            return list;
        }

        public void InsertPayment(Payment payment)
        {
            using var connection = new SqlConnection(_connectionString);
            string query = "INSERT INTO Payment (OrderId, TotalAmount, HighVatAmount, LowVatAmount, TipAmount, Method, Feedback, PaymentTime) VALUES (@OrderId, @TotalAmount, @HighVatAmount, @LowVatAmount, @TipAmount, @Method, @Feedback, @PaymentTime)";
            var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@OrderId", payment.Order.OrderId);
            command.Parameters.AddWithValue("@TotalAmount", payment.TotalAmount);
            command.Parameters.AddWithValue("@HighVatAmount", payment.HighVatAmount);
            command.Parameters.AddWithValue("@LowVatAmount", payment.LowVatAmount);
            command.Parameters.AddWithValue("@TipAmount", payment.TipAmount);
            command.Parameters.AddWithValue("@Method", (int)payment.Method);
            command.Parameters.AddWithValue("@Feedback", payment.Feedback);
            command.Parameters.AddWithValue("@PaymentTime", payment.PaymentTime);

            connection.Open();
            command.ExecuteNonQuery();
        }

        public void UpdateTableStatusAfterPayment(int tableId, string status)
        {
            using var connection = new SqlConnection(_connectionString);
            string query = "UPDATE [Table] SET TableStatus = @Status WHERE TableId = @TableId";
            var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Status", status);
            command.Parameters.AddWithValue("@TableId", tableId);

            connection.Open();
            command.ExecuteNonQuery();
        }

        public void UpdateOrderStatus(int orderId, string status)
        {
            using var connection = new SqlConnection(_connectionString);
            string query = "UPDATE [Order] SET OrderStatus = @Status WHERE OrderId = @OrderId";
            var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Status", status);
            command.Parameters.AddWithValue("@OrderId", orderId);

            connection.Open();
            command.ExecuteNonQuery();
        }
    }
}