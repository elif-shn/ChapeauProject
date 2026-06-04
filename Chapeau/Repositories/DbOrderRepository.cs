using Chapeau.Enums;
using Chapeau.Models;
using Chapeau.Repositories.Interfaces;
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
            List<Order> orders = new List<Order>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"SELECT o.OrderId, o.TableId, o.EmployeeId, o.OrderTime, o.WaitingTime, o.ServedTime, o.OrderStatus
                         FROM [Order] o
                         WHERE o.OrderStatus NOT IN ('Completed', 'Cancelled', 'Paid', 'Served')";

                SqlCommand command = new SqlCommand(query, connection);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read()) orders.Add(ReadOrder(reader));
                }
                catch (SqlException ex) { throw new Exception("Failed to get running orders.", ex); }
            }

            foreach (Order order in orders)
            {
                order.OrderItems = GetOrderItemsByOrderId(order);
            }

            return orders;
        }
        public List<Order> GetFinishedOrders()
        {
            List<Order> orders = new List<Order>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"SELECT o.OrderId, o.TableId, o.EmployeeId, o.OrderTime, o.WaitingTime, o.ServedTime, o.OrderStatus
                         FROM [Order] o
                         WHERE o.OrderStatus IN ('Served', 'Completed')
                         ORDER BY o.OrderTime DESC";

                SqlCommand command = new SqlCommand(query, connection);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read()) orders.Add(ReadOrder(reader));
                }
                catch (SqlException ex) { throw new Exception("Failed to get finished orders.", ex); }
            }

            foreach (Order order in orders)
            {
                order.OrderItems = GetOrderItemsByOrderId(order);
            }

            return orders;
        }



        public List<OrderItem> GetOrderItemsByOrderId(Order order)
        {
            List<OrderItem> items = new List<OrderItem>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"SELECT oi.OrderItemId, oi.OrderItemQuantity, oi.Comment, oi.OrderItemsStatus,
                                        mi.MenuItemId, mi.MenuItemName, mi.MenuItemPrice, mi.VatPercentage,
                                        mi.MenuId, m.Card, m.Category
                                 FROM OrderItem oi
                                 JOIN MenuItem mi ON oi.MenuItemId = mi.MenuItemId
                                 JOIN Menu m      ON mi.MenuId = m.MenuId
                                 WHERE oi.OrderId = @OrderId";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@OrderId", order.OrderId);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    items.Add(ReadOrderItem(reader));
                }
            }

            return items;
        }

        public Order? GetOrderById(Order order)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"SELECT OrderId, TableId, EmployeeId, OrderTime, WaitingTime, ServedTime, OrderStatus
                                 FROM [Order] WHERE OrderId = @OrderId";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@OrderId", order.OrderId);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    return ReadOrder(reader);
                }
            }

            return null;
        }

        public void UpdateOrderStatus(Order order, OrderStatus status)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "UPDATE [Order] SET OrderStatus = @Status WHERE OrderId = @OrderId";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Status", status.ToString());
                command.Parameters.AddWithValue("@OrderId", order.OrderId);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public void UpdateOrderItemStatus(OrderItem orderItem, OrderItemStatus status)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "UPDATE OrderItem SET OrderItemsStatus = @Status WHERE OrderItemId = @OrderItemId";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Status", status.ToString());
                command.Parameters.AddWithValue("@OrderItemId", orderItem.OrderItemId);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public Order CreateOrder(int tableId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"INSERT INTO [Order] (TableId, EmployeeId, OrderTime, ServedTime, OrderStatus)
                                 VALUES (@TableId, @EmployeeId, GETDATE(), NULL, 'Ordered');
                                 SELECT SCOPE_IDENTITY();";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@TableId", tableId);
                command.Parameters.AddWithValue("@EmployeeId", 1);
                connection.Open();

                int newOrderId = Convert.ToInt32(command.ExecuteScalar());
                return GetOrderById(new Order { OrderId = newOrderId });
            }
        }

        public Order GetActiveOrderForTable(int tableId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"SELECT OrderId, TableId, EmployeeId, OrderTime, WaitingTime, ServedTime, OrderStatus
                                 FROM [Order]
                                 WHERE TableId = @TableId
                                 AND OrderStatus NOT IN ('Settled', 'Cancelled', 'Paid','Served')";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@TableId", tableId);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    return ReadOrder(reader);
                }
            }

            return null;
        }

        public void AddOrderItemToOrder(OrderItem newOrderItem)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"INSERT INTO OrderItem (OrderId, MenuItemId, OrderItemQuantity, Comment, OrderItemsStatus)
                                 VALUES (@OrderId, @MenuItemId, @Quantity, @Comment, 'Ordered')";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@OrderId", newOrderItem.Order.OrderId);
                command.Parameters.AddWithValue("@MenuItemId", newOrderItem.MenuItem.MenuItemId);
                command.Parameters.AddWithValue("@Quantity", newOrderItem.OrderItemQuantity);
                command.Parameters.AddWithValue("@Comment", newOrderItem.Comment ?? "");
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public void IncreaseOrderItemQuantity(OrderItem newOrderItem, int quantity)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"UPDATE OrderItem SET OrderItemQuantity = OrderItemQuantity + @Quantity
                                 WHERE OrderId = @OrderId AND MenuItemId = @MenuItemId
                                 AND (Comment = @Comment OR (Comment IS NULL AND @Comment = ''))";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@OrderId", newOrderItem.Order.OrderId);
                command.Parameters.AddWithValue("@MenuItemId", newOrderItem.MenuItem.MenuItemId);
                command.Parameters.AddWithValue("@Quantity", quantity);
                command.Parameters.AddWithValue("@Comment", newOrderItem.Comment ?? "");
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

    
        //Private Helpers Methods//
        private Order ReadOrder(SqlDataReader reader)
        {
            Order order = new Order();
            order.OrderId = (int)reader["OrderId"];
            order.OrderTime = (DateTime)reader["OrderTime"];
            order.WaitingTime = reader["WaitingTime"].ToString();
            order.ServedTime = reader["ServedTime"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["ServedTime"];
            order.OrderStatus = Enum.Parse<OrderStatus>(reader["OrderStatus"].ToString());
            order.Table = ReadTable(reader);
            order.Employee = ReadEmployee(reader);
            return order;
        }

        private Table ReadTable(SqlDataReader reader)
        {
            Table table = new Table();
            table.TableId = (int)reader["TableId"];
            return table;
        }

        private User ReadEmployee(SqlDataReader reader)
        {
            User employee = new User();
            employee.Id = (int)reader["EmployeeId"];
            return employee;
        }

        private OrderItem ReadOrderItem(SqlDataReader reader)
        {
            OrderItem orderItem = new OrderItem();
            orderItem.OrderItemId = (int)reader["OrderItemId"];
            orderItem.OrderItemQuantity = (int)reader["OrderItemQuantity"];
            orderItem.Comment = reader["Comment"]?.ToString() ?? "";
            orderItem.OrderItemStatus = Enum.Parse<OrderItemStatus>(reader["OrderItemsStatus"].ToString());
            orderItem.MenuItem = ReadMenuItem(reader);
            return orderItem;
        }

        private MenuItem ReadMenuItem(SqlDataReader reader)
        {
            MenuItem menuItem = new MenuItem();
            menuItem.MenuItemId = (int)reader["MenuItemId"];
            menuItem.MenuItemName = (string)reader["MenuItemName"];
            menuItem.MenuItemPrice = Convert.ToDecimal(reader["MenuItemPrice"]);
            menuItem.VatPercentage = Convert.ToInt32(reader["VatPercentage"]);
            menuItem.Menu = ReadMenu(reader);
            return menuItem;
        }

        private Menu ReadMenu(SqlDataReader reader)
        {
            Menu menu = new Menu();
            menu.MenuId = (int)reader["MenuId"];
            menu.Card = Enum.Parse<Card>(reader["Card"].ToString());
            menu.Category = Enum.Parse<Category>(reader["Category"].ToString());
            return menu;
        }
    }
}

