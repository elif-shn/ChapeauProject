using Chapeau.ViewModels;
using Microsoft.Data.SqlClient;

namespace Chapeau.Repositories
{
    public class DbOrderRepository : IOrderRepository
    {
        private readonly string? _connectionString;

        public DbOrderRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ChapeauDataBase");
        }

        public List<RunningOrderViewModel> GetRunningOrder()
        {
            List<RunningOrderViewModel> orders = new List<RunningOrderViewModel>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM [Order] WHERE OrderStatus NOT IN ('Completed', 'Served', 'Cancelled') ORDER BY OrderTime ASC";
                SqlCommand command = new SqlCommand(query, connection);

                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    RunningOrderViewModel order = ReadRunningOrder(reader);

                    orders.Add(order);
                }
                reader.Close();
            }
            return orders;
        }
        private RunningOrderViewModel ReadRunningOrder(SqlDataReader reader)
        {
            DateTime orderTime = (DateTime)reader["OrderTime"];
            string displayTime = reader["WaitingTime"].ToString();
            return new RunningOrderViewModel((int)reader["OrderId"], (int)reader["TableId"], orderTime, displayTime, reader["OrderStatus"].ToString());

        }
    }
}





