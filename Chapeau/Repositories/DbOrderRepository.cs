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
                string query = "SELECT OrderId, TableId, OrderTime, OrderStatus FROM [Order] WHERE OrderStatus IN ('pending', 'preparing', 'ready') ORDER BY OrderTime ASC";
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

            TimeSpan waitingTime = DateTime.Now - orderTime;

            return new RunningOrderViewModel((int)reader["OrderId"], (int)reader["TableId"], orderTime, $"{waitingTime.TotalMinutes:0} min", (string)reader["OrderStatus"]);
        }
    }
}





