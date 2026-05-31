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
                                        mi.MenuItemId, mi.MenuItemName, mi.MenuItemPrice, mi.VatPercentage, mi.MenuId,m.Card, m.Category
                                 FROM OrderItem oi 
                                 JOIN MenuItem mi ON oi.MenuItemId = mi.MenuItemId
                                 INNER JOIN Menu m ON mi.MenuId = m.MenuId
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
            using SqlConnection connection = new SqlConnection(_connectionString);

            string query = @"UPDATE OrderItem SET OrderItemsStatus = @Status WHERE OrderItemId = @OrderItemId";

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
        //For Take Order Part
        public Order CreateOrder(int tableId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"
            INSERT INTO [Order] (TableId, EmployeeId, OrderTime, ServedTime, OrderStatus)
            VALUES (@TableId, @EmployeeId, GETDATE(), NULL, 'Ordered');

            SELECT SCOPE_IDENTITY();";

                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@TableId", tableId);
                command.Parameters.AddWithValue("@EmployeeId", 1); 

                connection.Open();

                int newOrderId = Convert.ToInt32(command.ExecuteScalar());

                return GetOrderById(newOrderId);
            }
        }


        public void IncreaseOrderItemQuantity(OrderItem newOrderItem, int quantity)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"
            UPDATE OrderItem 
            SET OrderItemQuantity = OrderItemQuantity + @Quantity
            WHERE OrderId = @OrderId 
            AND MenuItemId = @MenuItemId
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

        public void AddOrderItemToOrder(OrderItem newOrderItem)

        {

            using (SqlConnection connection = new SqlConnection(_connectionString))

            {

                string query = "INSERT INTO OrderItem (OrderId, MenuItemId, OrderItemQuantity, Comment, OrderItemsStatus) " +
                               "VALUES (@OrderId, @MenuItemId, @Quantity, @Comment, 'Ordered')";
                            

                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@OrderId", newOrderItem.Order.OrderId);
                command.Parameters.AddWithValue("@Quantity", newOrderItem.OrderItemQuantity);
                command.Parameters.AddWithValue("@MenuItemId", newOrderItem.MenuItem.MenuItemId);
                command.Parameters.AddWithValue("@Comment", newOrderItem.Comment ?? "");



                connection.Open();

                command.ExecuteNonQuery();

            }

        }
        public Order GetActiveOrderForTable(int tableId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT OrderId, OrderStatus FROM [Order] WHERE TableId = @TableId " +
                               "AND OrderStatus NOT IN ('Settled', 'Cancelled', 'Paid') ";


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
                    VatPercentage = Convert.ToInt32(reader["VatPercentage"]),
                    Menu = ReadMenu(reader, (int)reader["MenuId"])
                }
            };
        }
        private Menu ReadMenu(SqlDataReader reader, int menuId)
        {
            return new Menu
            {
                MenuId = menuId,
                Card = Enum.Parse<Card>(reader["Card"].ToString()),
                Category = Enum.Parse<Category>(reader["Category"].ToString())
            };
        }
    }
}
