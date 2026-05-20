using Chapeau.Enums;
using Chapeau.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions;

namespace Chapeau.Repositories
{
    public class DbOrderItem : IOrderItem
    {
        private readonly string? _connectionString;

        public DbOrderItem(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ChapeauDataBase");
        }

        public List<OrderItem> GetOrderItemsByOrderId(int orderId)
        {
            List<OrderItem> items = new List<OrderItem>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT oi.OrderItemId, oi.OrderItemQuantity, oi.Comment, oi.OrderItemsStatus, mi.MenuItemId,mi.MenuItemName, mi.Category FROM OrderItem oi JOIN MenuItem mi ON oi.MenuItemId = mi.MenuItemId WHERE oi.OrderId = @OrderId";

                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@OrderId", orderId);

                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    items.Add(ReadOrderItem(reader));
                }

                reader.Close();
            }

            return items;
        }
        private OrderItem ReadOrderItem(SqlDataReader reader)
        {
            MenuItem menuItem = new MenuItem();
            menuItem.MenuItemId = (int)reader["MenuItemId"];
            menuItem.MenuItemName = reader["MenuItemName"].ToString();
            menuItem.Category = (Category)(int)reader["Category"];

            return new OrderItem((int)reader["OrderItemId"], null,
                menuItem, (int)reader["OrderItemQuantity"],
                reader["Comment"].ToString(),
                reader["OrderItemsStatus"].ToString());
        }
    }
}


