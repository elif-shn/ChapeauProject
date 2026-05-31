using Chapeau.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;

namespace Chapeau.Repositories
{
    public class DbPaymentRepository : IPaymentRepository
    {
        private readonly string? _connectionString;

        public DbPaymentRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ChapeauDataBase");
        }

        public void AddPayment(Payment payment)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string sql = @"INSERT INTO Payment (OrderId, TotalAmount, TipAmount, Vat9, Vat21, PaymentMethod, Feedback, PaymentDate) 
                               VALUES (@OrderId, @TotalAmount, @TipAmount, @Vat9, @Vat21, @PaymentMethod, @Feedback, @PaymentDate)";

                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@OrderId", payment.OrderId);
                command.Parameters.AddWithValue("@TotalAmount", payment.TotalAmount);
                command.Parameters.AddWithValue("@TipAmount", payment.TipAmount);
                command.Parameters.AddWithValue("@Vat9", payment.Vat9);
                command.Parameters.AddWithValue("@Vat21", payment.Vat21);
                command.Parameters.AddWithValue("@PaymentMethod", (int)payment.PaymentMethod);
                command.Parameters.AddWithValue("@Feedback", (object)payment.Feedback ?? DBNull.Value);
                command.Parameters.AddWithValue("@PaymentDate", payment.PaymentDate);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }
    }
}