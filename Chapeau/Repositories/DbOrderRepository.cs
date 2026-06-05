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
            try
            {
                List<Order> orders = new List<Order>();

                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    string query = $@"SELECT o.OrderId, o.TableId, o.EmployeeId, o.OrderTime, o.ServedTime, o.OrderStatus 
                  FROM [Order] o WHERE o.OrderStatus NOT IN ('{OrderStatus.Paid}', '{OrderStatus.Cancelled}', '{OrderStatus.Served}', '{OrderStatus.Settled}')
                  ORDER BY o.OrderTime ASC";


                    SqlCommand command = new SqlCommand(query, connection);

                    connection.Open();

                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        orders.Add(ReadOrder(reader));
                    }
                }

                foreach (Order order in orders)
                {
                    order.OrderItems = GetOrderItemsByOrderId(order);
                }

                return orders;
            }
            catch (SqlException ex)
            {
                throw new Exception("Failed to retrieve running orders.", ex);
            }
        }
        public List<Order> GetFinishedOrders()
        {
            try
            {
                List<Order> orders = new List<Order>();

                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    string query = $@"SELECT o.OrderId, o.TableId, o.EmployeeId, o.OrderTime, o.ServedTime, o.OrderStatus
                                   FROM [Order] o WHERE o.OrderStatus IN ('{OrderStatus.Served}','{OrderStatus.Settled}') 
                                   ORDER BY o.OrderTime ASC";

                    SqlCommand command = new SqlCommand(query, connection);

                    connection.Open();

                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        orders.Add(ReadOrder(reader));
                    }
                }

                foreach (Order order in orders)
                {
                    order.OrderItems = GetOrderItemsByOrderId(order);
                }

                return orders;
            }
            catch (SqlException ex)
            {
                throw new Exception("Failed to retrieve finished orders.", ex);
            }
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
                                 JOIN Menu m ON mi.MenuId = m.MenuId
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
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    string query = @"SELECT OrderId, TableId, EmployeeId, OrderTime, ServedTime, OrderStatus FROM [Order]
                                   WHERE OrderId = @OrderId";

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
            catch (SqlException ex)
            {
                throw new Exception("Failed to retrieve order.", ex);
            }
        }

        

        public Order CreateOrderWithItems(Order order)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        string orderQuery = $@"INSERT INTO [Order] (TableId, EmployeeId, OrderTime, ServedTime, OrderStatus)
                                      VALUES (@TableId, @EmployeeId, GETDATE(), NULL,  '{OrderStatus.Ordered}');
                                      SELECT SCOPE_IDENTITY();";

                        int newOrderId;
                        using (SqlCommand orderCommand = new SqlCommand(orderQuery, connection, transaction))
                        {
                            orderCommand.Parameters.AddWithValue("@TableId", order.TableId);
                            orderCommand.Parameters.AddWithValue("@EmployeeId", order.Employee?.Id ?? 1);

                            newOrderId = Convert.ToInt32(orderCommand.ExecuteScalar());
                        }

                        if (order.OrderItems != null && order.OrderItems.Count > 0)
                        {
                            string itemQuery = $@"INSERT INTO OrderItem (OrderId, MenuItemId, OrderItemQuantity, Comment, OrderItemsStatus)
                                         VALUES (@OrderId, @MenuItemId, @Quantity, @Comment, '{OrderStatus.Ordered}')";

                            foreach (var item in order.OrderItems)
                            {
                                using (SqlCommand itemCommand = new SqlCommand(itemQuery, connection, transaction))
                                {
                                    itemCommand.Parameters.AddWithValue("@OrderId", newOrderId);
                                    itemCommand.Parameters.AddWithValue("@MenuItemId", item.MenuItem.MenuItemId);
                                    itemCommand.Parameters.AddWithValue("@Quantity", item.OrderItemQuantity);
                                    itemCommand.Parameters.AddWithValue("@Comment", item.Comment ?? "");

                                    itemCommand.ExecuteNonQuery();
                                }
                            }
                        }
                        transaction.Commit();
                        order.OrderId = newOrderId;
                        return GetOrderById(order);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();

                        throw new Exception("An error occurred while saving the order and its items; all operations have been rolled back.", ex);
                    }
                }
            }
        }
        public Order GetActiveOrderForTable(int tableId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = $@"
                            SELECT OrderId, TableId, EmployeeId, OrderTime, WaitingTime, ServedTime, OrderStatus
                            FROM [Order]
                            WHERE TableId = @TableId
                            AND OrderStatus NOT IN (
                                '{OrderStatus.Settled}',
                                '{OrderStatus.Cancelled}',
                                '{OrderStatus.Paid}',
                                '{OrderStatus.Served}'
                            )";

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

        public void AddItemsToExistingOrder(int orderId, List<OrderItem> items)
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
                            // 1. Aşama: Ürün bu siparişte var mı kontrolü (COUNT ile)
                            string checkQuery = @"
                        SELECT COUNT(OrderItemId)
                        FROM OrderItem
                        WHERE OrderId = @OrderId 
                        AND MenuItemId = @MenuItemId
                        AND (Comment = @Comment OR (Comment IS NULL AND @Comment = ''))";

                            bool itemExists = false;
                            using (SqlCommand checkCmd = new SqlCommand(checkQuery, connection, transaction))
                            {
                                checkCmd.Parameters.AddWithValue("@OrderId", orderId);
                                checkCmd.Parameters.AddWithValue("@MenuItemId", item.MenuItem.MenuItemId);
                                checkCmd.Parameters.AddWithValue("@Comment", item.Comment ?? "");

                                itemExists = (int)checkCmd.ExecuteScalar() > 0;
                            }

                            if (itemExists)
                            {
                                // 2. Aşama: Varsa UPDATE
                                string updateQuery = @"
                            UPDATE OrderItem 
                            SET OrderItemQuantity = OrderItemQuantity + @Qty
                            WHERE OrderId = @OrderId 
                            AND MenuItemId = @MenuItemId
                            AND (Comment = @Comment OR (Comment IS NULL AND @Comment = ''))";

                                using (SqlCommand updateCmd = new SqlCommand(updateQuery, connection, transaction))
                                {
                                    updateCmd.Parameters.AddWithValue("@OrderId", orderId);
                                    updateCmd.Parameters.AddWithValue("@MenuItemId", item.MenuItem.MenuItemId);
                                    updateCmd.Parameters.AddWithValue("@Qty", item.OrderItemQuantity);
                                    updateCmd.Parameters.AddWithValue("@Comment", item.Comment ?? "");

                                    updateCmd.ExecuteNonQuery();
                                }
                            }
                            else
                            {
                                // 3. Aşama: Yoksa INSERT
                                string insertQuery = @"
                            INSERT INTO OrderItem (OrderId, MenuItemId, OrderItemQuantity, Comment, OrderItemsStatus)
                            VALUES (@OrderId, @MenuItemId, @Qty, @Comment, 'Ordered')";

                                using (SqlCommand insertCmd = new SqlCommand(insertQuery, connection, transaction))
                                {
                                    insertCmd.Parameters.AddWithValue("@OrderId", orderId);
                                    insertCmd.Parameters.AddWithValue("@MenuItemId", item.MenuItem.MenuItemId);
                                    insertCmd.Parameters.AddWithValue("@Qty", item.OrderItemQuantity);
                                    insertCmd.Parameters.AddWithValue("@Comment", item.Comment ?? "");

                                    insertCmd.ExecuteNonQuery();
                                }
                            }
                        }

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
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

