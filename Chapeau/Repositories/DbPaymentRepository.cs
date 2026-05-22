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

        List<OrderItem> IPaymentRepository.Getbyid(int orderId)
        {
            throw new NotImplementedException();
        }
    }
}