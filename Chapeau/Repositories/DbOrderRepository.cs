using Chapeau.Enums;
using Chapeau.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Chapeau.Repositories
{
    public class DbOrderRepository : IOrderRepository
    {
        private readonly string? _connectionString;

        public DbOrderRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ChapeauDatabase");
        }

        public List<Order> GetAllOrders()
        {
            List<Order> orders = new List<Order>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT OrderId, TableId, OrderStatus FROM [Order] WHERE OrderStatus NOT IN ('Completed', 'Cancelled')";
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    orders.Add(new Order
                    {
                        OrderId = (int)reader["OrderId"],
                        Table = new Table { TableId = (int)reader["TableId"] },
                        OrderStatus = Enum.Parse<OrderStatus>(reader["OrderStatus"].ToString())
                    });
                }
            }
            return orders;
        }

        public Order? GetOrderById(int id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM [Order] WHERE OrderId = @OrderId";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@OrderId", id);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read()) return new Order { OrderId = (int)reader["OrderId"] };
            }
            return null;
        }

        public void UpdateOrderStatus(int orderId, OrderStatus status)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "UPDATE [Order] SET OrderStatus = @Status WHERE OrderId = @OrderId";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Status", status.ToString());
                command.Parameters.AddWithValue("@OrderId", orderId);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public List<OrderItem> GetOrderItemsByOrderId(int orderId)
        {
            List<OrderItem> items = new List<OrderItem>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"SELECT oi.OrderItemId, oi.OrderItemQuantity, oi.Comment, oi.OrderItemsStatus, 
                                        mi.MenuItemId, mi.MenuItemName, mi.MenuItemPrice, mi.VatPercentage 
                                 FROM OrderItem oi 
                                 JOIN MenuItem mi ON oi.MenuItemId = mi.MenuItemId
                                 WHERE oi.OrderId = @OrderId";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@OrderId", orderId);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read()) items.Add(ReadOrderItem(reader));
            }
            return items;
        }

        public void UpdateOrderItemStatus(int orderItemId, OrderStatus status)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "UPDATE OrderItem SET OrderItemsStatus = @Status WHERE OrderItemId = @OrderItemId";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Status", status.ToString());
                command.Parameters.AddWithValue("@OrderItemId", orderItemId);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public List<Order> GetFinishedOrders()
        {
            List<Order> orders = new List<Order>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT OrderId, TableId, OrderStatus FROM [Order] WHERE OrderStatus IN ('Served', 'Completed')";
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    orders.Add(new Order { OrderId = (int)reader["OrderId"] });
                }
            }
            return orders;
        }

        public int CreateOrder(int tableId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "INSERT INTO [Order] (TableId, OrderTime, OrderStatus) VALUES (@TableId, GETDATE(), 'Ordered'); SELECT SCOPE_IDENTITY();";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@TableId", tableId);
                connection.Open();
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        public bool OrderItemExists(int orderId, int menuItemId, string comment)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT 1 FROM OrderItem WHERE OrderId = @OrderId AND MenuItemId = @MenuItemId AND Comment = @Comment";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@OrderId", orderId);
                command.Parameters.AddWithValue("@MenuItemId", menuItemId);
                command.Parameters.AddWithValue("@Comment", comment ?? "");
                connection.Open();
                return command.ExecuteScalar() != null;
            }
        }

        public void IncreaseQuantity(int orderId, int menuItemId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "UPDATE OrderItem SET OrderItemQuantity = OrderItemQuantity + 1 WHERE OrderId = @OrderId AND MenuItemId = @MenuItemId";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@OrderId", orderId);
                command.Parameters.AddWithValue("@MenuItemId", menuItemId);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public void AddOrderItem(int orderId, int menuItemId, string comment)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "INSERT INTO OrderItem (OrderId, MenuItemId, OrderItemQuantity, Comment, OrderItemsStatus) VALUES (@OrderId, @MenuItemId, 1, @Comment, 'Ordered')";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@OrderId", orderId);
                command.Parameters.AddWithValue("@MenuItemId", menuItemId);
                command.Parameters.AddWithValue("@Comment", comment ?? "");
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public Order GetActiveOrderForTable(int tableId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT TOP 1 OrderId, OrderStatus FROM [Order] WHERE TableId = @TableId AND OrderStatus NOT IN ('Completed', 'Cancelled')";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@TableId", tableId);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read()) return new Order { OrderId = (int)reader["OrderId"] };
            }
            return null;
        }

        private OrderItem ReadOrderItem(SqlDataReader reader)
        {
            return new OrderItem
            {
                OrderItemId = (int)reader["OrderItemId"],
                OrderItemQuantity = (int)reader["OrderItemQuantity"],
                Comment = reader["Comment"]?.ToString() ?? "",
                OrderItemStatus = Enum.Parse<OrderStatus>(reader["OrderItemsStatus"].ToString()),
                MenuItem = new MenuItem
                {
                    MenuItemId = (int)reader["MenuItemId"],
                    MenuItemName = (string)reader["MenuItemName"],
                    MenuItemPrice = Convert.ToDecimal(reader["MenuItemPrice"]),
                    VatPercentage = Convert.ToInt32(reader["VatPercentage"])
                }
            };
        }
    }
}