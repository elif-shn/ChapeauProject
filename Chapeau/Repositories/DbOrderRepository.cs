using Chapeau.Enums;
using Chapeau.Models;
using Chapeau.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Xml.Linq;

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

        public List<OrderItem> GetOrderItemsByOrderId(Order order)
        {
            try
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

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            items.Add(ReadOrderItem(reader));
                        }
                    }
                }

                return items;
            }
            catch (SqlException ex)
            {
                throw new Exception("Database error occurred while fetching order items.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while fetching order items.", ex);
            }
        }
        public Order? GetOrderById(Order order)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    string query = @"SELECT OrderId, TableId, EmployeeId, OrderTime, WaitingTime, ServedTime, OrderStatus
                             FROM [Order] WHERE OrderId = @OrderId";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@OrderId", order.OrderId);

                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return ReadOrder(reader);
                        }
                    }
                }

                return null;
            }
            catch (SqlException ex)
            {
                throw new Exception("Database error occurred while fetching the order by ID.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while fetching the order by ID.", ex);
            }
        }
        public Order GetActiveOrderForTable(int tableId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    string query = @"
                SELECT OrderId, TableId, EmployeeId, OrderTime, WaitingTime, ServedTime, OrderStatus
                FROM [Order]
                WHERE TableId = @TableId
                AND OrderStatus NOT IN (@Settled, @Cancelled, @Paid, @Served)";

                    SqlCommand command = new SqlCommand(query, connection);

                    command.Parameters.AddWithValue("@TableId", tableId);
                    command.Parameters.AddWithValue("@Settled", OrderStatus.Settled.ToString());
                    command.Parameters.AddWithValue("@Cancelled", OrderStatus.Cancelled.ToString());
                    command.Parameters.AddWithValue("@Paid", OrderStatus.Paid.ToString());
                    command.Parameters.AddWithValue("@Served", OrderStatus.Served.ToString());

                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return ReadOrder(reader);
                        }
                    }
                }

                return null;
            }
            catch (SqlException ex)
            {
                throw new Exception("Database error occurred while getting active order for table.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Unexpected error occurred while getting active order for table.", ex);
            }
        }
        public void CreateOrderWithItems(Order order)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        int newOrderId = CreateNewOrder(connection, transaction, order);

                        if (order.OrderItems != null && order.OrderItems.Count > 0)
                        {
                            foreach (var item in order.OrderItems)
                            {
                                AddOrderItem(connection, transaction, newOrderId, item);
                            }
                        }
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();

                        throw new Exception("An error occurred while saving the order and its items; all operations have been rolled back.", ex);
                    }
                }
            }
        }
        private int CreateNewOrder(SqlConnection connection, SqlTransaction transaction, Order order)
        {
            string orderQuery = @"
                                INSERT INTO [Order] 
                                (TableId, EmployeeId, OrderTime, ServedTime, OrderStatus)
                                VALUES 
                                (@TableId, @EmployeeId, GETDATE(), NULL, @OrderStatus);
                                SELECT SCOPE_IDENTITY();";

            using (SqlCommand orderCommand = new SqlCommand(orderQuery, connection, transaction))
            {
                orderCommand.Parameters.AddWithValue("@TableId", order.TableId);
                orderCommand.Parameters.AddWithValue("@EmployeeId", order.Employee?.Id ?? 1);
                orderCommand.Parameters.AddWithValue("@OrderStatus", OrderStatus.Ordered.ToString());

                int newOrderId = Convert.ToInt32(orderCommand.ExecuteScalar());
                return newOrderId;
            }

        }
        private void AddOrderItem(SqlConnection connection, SqlTransaction transaction, int orderId, OrderItem item)
        {
            string query = @"
            INSERT INTO OrderItem
                                (OrderId, MenuItemId, OrderItemQuantity, Comment, OrderItemsStatus)
                                VALUES
                                (@OrderId, @MenuItemId, @Quantity, @Comment, @Status)";
            using (SqlCommand cmd = new SqlCommand(query, connection, transaction))
            {
                cmd.Parameters.AddWithValue("@OrderId", orderId);
                cmd.Parameters.AddWithValue("@MenuItemId", item.MenuItem.MenuItemId);
                cmd.Parameters.AddWithValue("@Quantity", item.OrderItemQuantity);
                cmd.Parameters.AddWithValue("@Comment", item.Comment ?? "");
                cmd.Parameters.AddWithValue("@Status", OrderItemStatus.Ordered.ToString());

                cmd.ExecuteNonQuery();
            }
        }
        private void AddOrUpdateOrderItem(SqlConnection connection, SqlTransaction transaction, int orderId, OrderItem item)
        {
            string query = @"
                            IF EXISTS (
                                SELECT 1 FROM OrderItem
                                WHERE OrderId = @OrderId
                                  AND MenuItemId = @MenuItemId
                                  AND (Comment = @Comment OR (Comment IS NULL AND @Comment = ''))
                            )
                            BEGIN
                                UPDATE OrderItem
                                SET OrderItemQuantity = OrderItemQuantity + @Quantity
                                WHERE OrderId = @OrderId
                                  AND MenuItemId = @MenuItemId
                                  AND (Comment = @Comment OR (Comment IS NULL AND @Comment = ''))
                            END
                            ELSE
                            BEGIN
                                INSERT INTO OrderItem
                                (OrderId, MenuItemId, OrderItemQuantity, Comment, OrderItemsStatus)
                                VALUES
                                (@OrderId, @MenuItemId, @Quantity, @Comment, @Status)
                            END";

            using (SqlCommand cmd = new SqlCommand(query, connection, transaction))
            {
                cmd.Parameters.AddWithValue("@OrderId", orderId);
                cmd.Parameters.AddWithValue("@MenuItemId", item.MenuItem.MenuItemId);
                cmd.Parameters.AddWithValue("@Quantity", item.OrderItemQuantity);
                cmd.Parameters.AddWithValue("@Comment", item.Comment ?? "");
                cmd.Parameters.AddWithValue("@Status", OrderItemStatus.Ordered.ToString());

                cmd.ExecuteNonQuery();
            }
        }
        public void AddItemsToExistingOrder(Order order, List<OrderItem> items)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        foreach (var item in items)
                        {
                            AddOrUpdateOrderItem(connection, transaction, order.OrderId, item);
                        }

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception("An error occurred while saving the order and its items; all operations have been rolled back.", ex);
                    }
                }
            }
        }
        //Private Helpers Methods//
        private Order ReadOrder(SqlDataReader reader)
        {
            Order order = new Order();
            order.OrderId = (int)reader["OrderId"];
            order.OrderTime = (DateTime)reader["OrderTime"];
            order.WaitingTime = reader.IsDBNull(reader.GetOrdinal("WaitingTime"))? string.Empty : reader["WaitingTime"].ToString();
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
            orderItem.Comment = reader.IsDBNull(reader.GetOrdinal("Comment"))? string.Empty : reader["Comment"].ToString();
            orderItem.OrderItemStatus = Enum.Parse<OrderItemStatus>(reader["OrderItemsStatus"].ToString());
            orderItem.MenuItem = ReadMenuItem(reader);
            return orderItem;
        }

        private MenuItem ReadMenuItem(SqlDataReader reader)
        {
            MenuItem menuItem = new MenuItem();
            menuItem.MenuItemId = (int)reader["MenuItemId"];
            menuItem.MenuItemName = (string)reader["MenuItemName"];
            menuItem.MenuItemPrice = (decimal)reader["MenuItemPrice"];
            menuItem.VatPercentage = (int)reader["VatPercentage"];
            menuItem.Menu = ReadMenu(reader);
            return menuItem;
        }

        private Menu ReadMenu(SqlDataReader reader)
        {
            Menu menu = new Menu();
            menu.MenuId = (int)reader["MenuId"]; 
            menu.Card = (Card)(int)reader["Card"];
            menu.Category = (Category)(int)reader["Category"];
            return menu;
        }
    }
}

