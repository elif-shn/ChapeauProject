using Chapeau.Models;
using Chapeau.Repositories.Interfaces;
using Microsoft.Data.SqlClient;

namespace Chapeau.Repositories
{
    public class DbPaymentRepository : IPaymentRepository
    {
        private readonly string _connectionString;

        public DbPaymentRepository(IConfiguration configuration)
        {
            _connectionString =
                configuration.GetConnectionString("ChapeauDatabase");
        }

        public void SavePayment(Payment payment)
        {
            using (SqlConnection connection =
                new SqlConnection(_connectionString))
            {
                string query =
                @"INSERT INTO Payment
                (
                    OrderId,
                    TotalAmount,
                    TipAmount,
                    Vat9,
                    Vat21,
                    PaymentMethod,
                    Feedback,
                    PaymentDate
                )
                VALUES
                (
                    @OrderId,
                    @TotalAmount,
                    @TipAmount,
                    @Vat9,
                    @Vat21,
                    @PaymentMethod,
                    @Feedback,
                    GETDATE()
                )";

                SqlCommand command =
                    new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@OrderId", payment.OrderId);
                command.Parameters.AddWithValue("@TotalAmount", payment.TotalAmount);
                command.Parameters.AddWithValue("@TipAmount", payment.TipAmount);
                command.Parameters.AddWithValue("@Vat9", payment.Vat9);
                command.Parameters.AddWithValue("@Vat21", payment.Vat21);

                command.Parameters.AddWithValue(
                    "@PaymentMethod",
                    (int)payment.PaymentMethod);

                command.Parameters.AddWithValue(
                    "@Feedback",
                    payment.Feedback ?? "");

                connection.Open();

                command.ExecuteNonQuery();
            }
        }
    }
}