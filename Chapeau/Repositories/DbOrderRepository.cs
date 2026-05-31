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

        public List<Order> GetRunningOrders()
        {
            List<Order> orders = new();

            using (SqlConnection connection = new(_connectionString))
            {
                try
                {
                    string query = @"SELECT o.OrderId, o.TableId, o.EmployeeId, o.OrderTime, o.WaitingTime, o.ServedTime, o.OrderStatus
                                  FROM [Order] o JOIN [Table] t ON o.TableId = t.TableId
                                  WHERE o.OrderStatus NOT IN ('Served', 'Cancelled') ORDER BY o.OrderTime ASC";
                    SqlCommand command = new(query, connection);
                    connection.Open();

                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Order order = ReadOrder(reader);

                        order.OrderItems = GetOrderItemsByOrderId(order.OrderId);

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
            using (SqlConnection connection = new(_connectionString))
            {
                string query = @"SELECT o.OrderId, o.TableId, o.EmployeeId, o.OrderTime, o.WaitingTime, o.ServedTime, o.OrderStatus
                                 FROM [Order] o
                                 WHERE o.OrderId = @OrderId";

                SqlCommand command = new(query, connection);
                command.Parameters.AddWithValue("@OrderId", id);
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    order = ReadOrder(reader);
                }
            }

            return order;
        }
        public void UpdateOrderStatus(Order order, OrderStatus newStatus)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    string query = @"UPDATE [Order] SET OrderStatus = @Status WHERE OrderId = @OrderId";

                    SqlCommand command = new SqlCommand(query, connection);

                    command.Parameters.AddWithValue("@Status", newStatus.ToString());
                    command.Parameters.AddWithValue("@OrderId", order.OrderId);

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
            List<OrderItem> items = new();

            using (SqlConnection connection = new(_connectionString))
            {
                string query = @"SELECT oi.OrderItemId, oi.OrderItemQuantity, oi.Comment, oi.OrderItemsStatus, 
                                        mi.MenuItemId, mi.MenuItemName, mi.MenuItemPrice, mi.VatPercentage, mi.MenuId,m.Card, m.Category
                                 FROM OrderItem oi 
                                 JOIN MenuItem mi ON oi.MenuItemId = mi.MenuItemId
                                 INNER JOIN Menu m ON mi.MenuId = m.MenuId
                                 WHERE oi.OrderId = @OrderId";


                SqlCommand command = new(query, connection);
                command.Parameters.AddWithValue("@OrderId", orderId);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    items.Add(ReadOrderItem(reader));
                }

                return items;
            }
        }

        public void UpdateOrderItemStatus(OrderItem orderitem, OrderItemStatus status)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);

            string query = @"UPDATE OrderItem SET OrderItemsStatus = @Status WHERE OrderItemId = @OrderItemId";

            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@Status", status.ToString());
            command.Parameters.AddWithValue("@OrderItemId", orderitem.OrderItemId);

            connection.Open();
            command.ExecuteNonQuery();
        }
        public List<Order> GetFinishedOrders()
        {
            List<Order> orders = new();

            using (SqlConnection connection =
                   new(_connectionString))
            {
                string query = @"SELECT o.OrderId, o.TableId, o.EmployeeId, o.OrderTime, o.WaitingTime, o.ServedTime, o.OrderStatus
                               FROM [Order] o JOIN [Table] t ON o.TableId = t.TableId
                               WHERE o.OrderStatus IN ('Served', 'Completed') ORDER BY o.OrderTime DESC";
                SqlCommand command =
                    new(query, connection);

                connection.Open();

                SqlDataReader reader =
                    command.ExecuteReader();
                while (reader.Read())
                {
                    Order order = ReadOrder(reader);

                    order.OrderItems = GetOrderItemsByOrderId(order.OrderId);

                    orders.Add(order);
                }
            }

            return orders;
        }
        private Order ReadOrder(SqlDataReader reader)
        {
            Order order = new();
            order.OrderId = (int)reader["OrderId"];
            order.Table = new Table { TableId = (int)reader["TableId"] };
            order.Employee = new User { Id = (int)reader["EmployeeId"] };
            order.OrderTime = (DateTime)reader["OrderTime"];
            order.WaitingTime = reader["WaitingTime"].ToString();
            order.ServedTime = reader["ServedTime"] == DBNull.Value ? null : (DateTime)reader["ServedTime"];
            order.OrderStatus = Enum.Parse<OrderStatus>(reader["OrderStatus"].ToString());
            return order;
        }
        private OrderItem ReadOrderItem(SqlDataReader reader)
        {
            OrderItem orderItem = new();

            orderItem.OrderItemId = (int)reader["OrderItemId"];
            orderItem.OrderItemQuantity = (int)reader["OrderItemQuantity"];
            orderItem.Comment = reader["Comment"].ToString();

            orderItem.OrderItemStatus =  Enum.Parse<OrderItemStatus>(reader["OrderItemsStatus"].ToString());

            orderItem.MenuItem = new MenuItem
            {
                MenuItemId = (int)reader["MenuItemId"],
                MenuItemName = reader["MenuItemName"].ToString(),

                Menu = new Menu
                {
                    Category = Enum.Parse<Category>(reader["Category"].ToString()
         )
                }
            };

            return orderItem;
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

        public Order? GetActiveOrderForTable(int tableId)
        {
            using (SqlConnection connection = new(_connectionString))
            {
                string query = "SELECT OrderId, TableId, EmployeeId, OrderTime, WaitingTime, ServedTime, OrderStatus FROM [Order] " +
                               "WHERE TableId = @TableId AND OrderStatus NOT IN ('Completed', 'Cancelled')";

                SqlCommand command = new(query, connection);
                command.Parameters.AddWithValue("@TableId", tableId);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();


                if (reader.Read())
                {
                    return ReadOrder(reader);
                }
                else
                {
                    return null;
                }
            }
        }

       
    }
}

























































































































































































































































































































































































































































































































































































































































































































































































































