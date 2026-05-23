using Chapeau.Enums;
using Chapeau.Models;
using Chapeau.Repositories;
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
                try
                {
                    string query = @"SELECT o.OrderId, o.TableId, o.EmployeeId, o.OrderTime, o.WaitingTime, o.ServedTime, o.OrderStatus
                                  FROM [Order] o JOIN [Table] t ON o.TableId = t.TableId
                                  WHERE o.OrderStatus NOT IN ('Completed', 'Cancelled') ORDER BY o.OrderTime ASC";
                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();

                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Order order = ReadOrder(reader);
                        orders.Add(order);
                    }
                }
                catch (SqlException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return orders;
        }

        public Order? GetOrderById(int id)
        {
            Order order = null;
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"SELECT o.OrderId, o.TableId, o.EmployeeId, o.OrderTime, o.WaitingTime, o.ServedTime, o.OrderStatus
                                 FROM [Order] o
                                 WHERE o.OrderId = @OrderId";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@OrderId", id);
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    order = ReadOrder(reader);
                }
            }

            return order;
        }
        public void UpdateOrderStatus(int orderId, OrderStatus status)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    string query = @"UPDATE [Order] SET OrderStatus = @Status WHERE OrderId = @OrderId";

                    SqlCommand command = new SqlCommand(query, connection);

                    command.Parameters.AddWithValue("@Status", status.ToString());

                    command.Parameters.AddWithValue("@OrderId", orderId);

                    connection.Open();

                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected == 0)
                    {
                        throw new Exception("Order not found.");
                    }
                }
                catch (SqlException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
        public List<OrderItem> GetOrderItemsByOrderId(int orderId)
        {
            List<OrderItem> items = new List<OrderItem>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    string query = @"SELECT oi.OrderItemId, oi.OrderItemQuantity, oi.Comment, oi.OrderItemStatus, mi.MenuItemId, mi.MenuItemName 
                                   FROM OrderItem oi JOIN MenuItem mi ON oi.MenuItemId = mi.MenuItemId
                                   WHERE oi.OrderId = @OrderId";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@OrderId", orderId);
                        
                    connection.Open();

                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        OrderItem item = ReadOrderItem(reader);
                        items.Add(item);
                    }

                    reader.Close();
                }
                catch (SqlException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return items;
        }
        public void UpdateOrderItemStatus(int orderItemId, OrderStatus status)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);

            string query = @"UPDATE OrderItem SET OrderItemStatus = @Status WHERE OrderItemId = @OrderItemId";

            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@Status", status.ToString());
            command.Parameters.AddWithValue("@OrderItemId", orderItemId);

            connection.Open();
            command.ExecuteNonQuery();
        }
        public List<Order> GetFinishedOrders()
        {
            List<Order> orders = new List<Order>();

            using (SqlConnection connection =
                   new SqlConnection(_connectionString))
            {
                string query = @"SELECT o.OrderId, o.TableId, o.EmployeeId, o.OrderTime, o.WaitingTime, o.ServedTime, o.OrderStatus
                               FROM [Order] o JOIN [Table] t ON o.TableId = t.TableId
                               WHERE o.OrderStatus IN ('Served', 'Completed') ORDER BY o.OrderTime DESC";
                SqlCommand command =
                    new SqlCommand(query, connection);

                connection.Open();

                SqlDataReader reader =
                    command.ExecuteReader();

                while (reader.Read())
                {
                    orders.Add(ReadOrder(reader));
                }

                reader.Close();
            }

            return orders;
        }

        private Order ReadOrder(SqlDataReader reader)
        {
            Table table = new Table();
            User employee = new User();
            Order order = new Order();
            table.TableId = (int)reader["TableId"];
            employee.Id = (int)reader["EmployeeId"];
            order.OrderId = (int)reader["OrderId"];
            order.Table = table;
            order.Employee = employee;

            order.OrderTime = (DateTime)reader["OrderTime"];
            order.WaitingTime = reader["WaitingTime"].ToString();
            order.ServedTime = reader["ServedTime"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["ServedTime"];
            order.OrderStatus = Enum.Parse<OrderStatus>(reader["OrderStatus"].ToString());

            return order;
        }
        private OrderItem ReadOrderItem(SqlDataReader reader)
        {
            return new OrderItem
            {
                OrderItemId = (int)reader["OrderItemId"],
                OrderItemQuantity = (int)reader["OrderItemQuantity"],
                Comment = reader["Comment"].ToString(),
                OrderItemStatus = Enum.Parse<OrderStatus>(reader["OrderItemStatus"].ToString()),
                MenuItem = new MenuItem
                {
                    MenuItemId = (int)reader["MenuItemId"],
                    MenuItemName = (string)reader["MenuItemName"],                 
                }
            };
        }
        public int CreateOrder(int tableId)

        {

            using (SqlConnection connection = new SqlConnection(_connectionString))

            {

                string query = "INSERT INTO [Order] (TableId, EmployeeId, OrderTime, ServedTime, OrderStatus) " +

                                "VALUES (@TableId, @EmployeeId, GETDATE(), NULL, 'Ordered') " +

                                "SELECT SCOPE_IDENTITY();";



                SqlCommand command =

                    new SqlCommand(query, connection);



                command.Parameters.AddWithValue("@TableId", tableId);

                command.Parameters.AddWithValue("@EmployeeId", 1);



                connection.Open();



                return Convert.ToInt32(command.ExecuteScalar());

            }

        }

        public bool OrderItemExists(int orderId, int menuItemId, string comment)

        {

            using (SqlConnection connection = new SqlConnection(_connectionString))

            {

                string query = "SELECT 1 FROM OrderItem " +

                               "WHERE OrderId = @OrderId " +

                               "AND MenuItemId = @MenuItemId " +

                               "AND Comment = @Comment";



                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@OrderId", orderId);

                command.Parameters.AddWithValue("@MenuItemId", menuItemId);

                command.Parameters.AddWithValue("@Comment", comment ?? "");



                connection.Open();

                object result = command.ExecuteScalar();



                return result != null;

            }

        }

        public void IncreaseQuantity(int orderId, int menuItemId)

        {

            using (SqlConnection connection = new SqlConnection(_connectionString))

            {

                string query = "UPDATE OrderItem SET OrderItemQuantity = OrderItemQuantity + 1 " +

                               "WHERE OrderId = @OrderId " +

                               "AND MenuItemId = @MenuItemId";



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

                string query = "INSERT INTO OrderItem (OrderId, MenuItemId, OrderItemQuantity, Comment, OrderItemsStatus) " +

                               "VALUES (@OrderId, @MenuItemId, 1, @Comment, 'Ordered')";



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
                string query = "SELECT TOP 1 * FROM [Order] WHERE TableId = @TableId " +
                               "AND OrderStatus NOT IN ('Completed', 'Cancelled') ";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@TableId", tableId);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    return new Order
                    {
                        OrderId = (int)reader["OrderId"],
                        OrderStatus = Enum.Parse<OrderStatus>(reader["OrderStatus"].ToString())
                    };
                }
            }
            return null;
        }
    }
} 













