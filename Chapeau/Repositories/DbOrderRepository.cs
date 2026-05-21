using Chapeau.Models;
using Chapeau.ViewModels;
using Chapeau.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;

namespace Chapeau.Repositories
{
    public class DbOrderRepository : IOrderRepository
    {
        private readonly string? _connectionString;

        public DbOrderRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ChapeauDatabase");
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
            Table table = new Table();
            table.TableId = (int)reader["TableId"];
            User employee = new User();
            employee.Id = (int)reader["EmployeeId"];
            Order order = new Order((int)reader["OrderId"], table, employee, (DateTime)reader["OrderTime"],
            reader["ServedTime"] as DateTime?, reader["OrderStatus"].ToString());
            string displayTime = reader["WaitingTime"].ToString();

            return new RunningOrderViewModel(order, table, order.OrderTime, displayTime, order.OrderStatus);
        }
    }
}



