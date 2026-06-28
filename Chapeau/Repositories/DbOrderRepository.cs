using Chapeau.Enums;
using Chapeau.Models;
using Microsoft.Data.SqlClient;

namespace Chapeau.Repositories
{
    public class DbOrderRepository : IOrderRepository
    {
        private readonly string _connectionString;

        public DbOrderRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public List<Order> GetRunningOrders(bool isFood)
        {
            List<Order> orders = new List<Order>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    string query = @"
                SELECT DISTINCT o.OrderId, o.TableId, o.EmployeeId, o.OrderTime, o.ServedTime, o.OrderStatus
                FROM [Order] o
                INNER JOIN OrderItem oi ON o.OrderId = oi.OrderId
                INNER JOIN MenuItem mi ON oi.MenuItemId = mi.MenuItemId
                WHERE mi.IsFood = @IsFood
                AND o.OrderStatus IN (@Ordered, @Preparing)
                ORDER BY o.OrderTime ASC";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@IsFood", isFood);
                    command.Parameters.AddWithValue("@Ordered", OrderStatus.Ordered.ToString());
                    command.Parameters.AddWithValue("@Preparing", OrderStatus.Preparing.ToString());

                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            orders.Add(ReadOrder(reader));
                        }
                    }

                    foreach (Order order in orders)
                    {
                        order.OrderItems = GetOrderItemsByOrderId(order.OrderId, isFood);
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("Error occurred while fetching running orders.", ex);
            }

            return orders;
        }

        public List<Order> GetFinishedOrders(bool isFood)
        {
            List<Order> orders = new List<Order>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    string query = @"SELECT DISTINCT o.OrderId, o.TableId, o.EmployeeId, o.OrderTime, o.ServedTime, o.OrderStatus   FROM [Order] o
                                  INNER JOIN OrderItem oi ON o.OrderId = oi.OrderId
                                  INNER JOIN MenuItem mi ON oi.MenuItemId = mi.MenuItemId
                                  WHERE mi.IsFood = @IsFood
                                  AND CAST(o.OrderTime AS DATE) = CAST(GETDATE() AS DATE)
                                  AND o.OrderStatus IN (@Ready, @Served)
                                  ORDER BY o.OrderTime DESC";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@IsFood", isFood);
                    command.Parameters.AddWithValue("@Ready", OrderStatus.Ready.ToString());
                    command.Parameters.AddWithValue("@Served", OrderStatus.Served.ToString());

                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            orders.Add(ReadOrder(reader));
                        }
                    }

                    foreach (Order order in orders)
                    {
                        order.OrderItems = GetOrderItemsByOrderId(order.OrderId, isFood);
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("Error occurred while fetching finished orders.", ex);
            }

            return orders;
        }



        public void UpdateOrderStatus(Order order)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    string query = "UPDATE [Order] SET OrderStatus = @OrderStatus WHERE OrderId = @OrderId";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@OrderId", order.OrderId);
                    command.Parameters.AddWithValue("@OrderStatus", order.OrderStatus.ToString());

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("Database error while updating order status.", ex);
            }
        }


        public void UpdateOrderItemStatus(OrderItem orderItem)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    string query = "UPDATE OrderItem SET OrderItemsStatus = @Status WHERE OrderItemId = @OrderItemId";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@OrderItemId", orderItem.OrderItemId);
                    command.Parameters.AddWithValue("@Status", orderItem.OrderItemStatus.ToString());

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("Database error while updating order item status.", ex);
            }
        }
        public void UpdateAllOrderItemsStatus(Order order, bool isFood, OrderItemStatus status)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    string query = @"UPDATE OrderItem SET OrderItemsStatus = @Status  FROM OrderItem oi
                                   INNER JOIN MenuItem mi ON mi.MenuItemId = oi.MenuItemId
                                   WHERE oi.OrderId = @OrderId
                                   AND mi.IsFood = @IsFood";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@OrderId", order.OrderId);
                    command.Parameters.AddWithValue("@IsFood", isFood);
                    command.Parameters.AddWithValue("@Status", status.ToString());

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("Database error while updating all order items status.", ex);
            }
        }

        public void UpdateCourseStatus(Order order, Category category, OrderItemStatus status)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    string query = @" UPDATE oi SET oi.OrderItemsStatus = @Status FROM OrderItem oi
                        JOIN MenuItem mi ON oi.MenuItemId = mi.MenuItemId
                        JOIN Menu m ON mi.MenuId = m.MenuId
                        WHERE oi.OrderId = @OrderId
                        AND m.Category = @Category
                        AND mi.IsFood = 1";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@OrderId", order.OrderId);
                    command.Parameters.AddWithValue("@Category", (int)category);
                    command.Parameters.AddWithValue("@Status", status.ToString());

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("Database error while updating course status.", ex);
            }
        }

        public Order GetOrderById(Order order)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    string query = "SELECT OrderId, TableId, EmployeeId, OrderTime, ServedTime, OrderStatus FROM [Order] WHERE OrderId = @OrderId";

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
                throw new Exception("Error occurred while fetching the order by ID.", ex);
            }
        }
        public Order? GetActiveOrderForTable(int tableId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    string query = @"SELECT OrderId, TableId, EmployeeId, OrderTime, WaitingTime, ServedTime, OrderStatus
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

        public List<OrderItem> GetOrderItemsByOrderId(int orderId, bool isFood)
        {
            try
            {
                List<OrderItem> items = new List<OrderItem>();

                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    string query = @"SELECT oi.OrderItemId, oi.OrderItemQuantity, oi.Comment, oi.OrderItemsStatus,
                                mi.MenuItemId, mi.MenuItemName, mi.MenuItemPrice, mi.VatPercentage, mi.IsFood,
                                m.MenuId, m.Category, m.Card
                               FROM OrderItem oi
                               INNER JOIN MenuItem mi ON oi.MenuItemId = mi.MenuItemId
                               INNER JOIN Menu m ON mi.MenuId = m.MenuId
                               WHERE oi.OrderId = @OrderId
                               AND mi.IsFood = @IsFood";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@OrderId", orderId);
                    command.Parameters.AddWithValue("@IsFood", isFood);

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

        public List<OrderItem> GetOrderItemsByOrderIdNoFilter(Order order)
        {
            try
            {
                List<OrderItem> items = new List<OrderItem>();

                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    string query = @"SELECT oi.OrderItemId, oi.OrderItemQuantity, oi.Comment, oi.OrderItemsStatus,
                                    mi.MenuItemId, mi.MenuItemName, mi.MenuItemPrice, mi.VatPercentage, mi.IsFood,
                                    m.MenuId, m.Card, m.Category
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
                            foreach (OrderItem item in order.OrderItems)
                            {
                                AddOrderItem(connection, transaction, newOrderId, item);
                                DecreaseItemStock(connection, transaction, item);
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
            try
            {
                string orderQuery = @"
                                INSERT INTO [Order] 
                                (TableId, EmployeeId, OrderTime, ServedTime, OrderStatus)
                                VALUES 
                                (@TableId, @EmployeeId, GETDATE(), NULL, @OrderStatus);
                                SELECT SCOPE_IDENTITY();;";

                using (SqlCommand orderCommand = new SqlCommand(orderQuery, connection, transaction))
                {
                    orderCommand.Parameters.AddWithValue("@TableId", order.TableId);
                    orderCommand.Parameters.AddWithValue("@EmployeeId", order.Employee.EmployeeId);
                    orderCommand.Parameters.AddWithValue("@OrderStatus", OrderStatus.Ordered.ToString());

                    int newOrderId = Convert.ToInt32(orderCommand.ExecuteScalar());
                    return newOrderId;
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("Database error occurred while creating a new order.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while creating a new order.", ex);
            }
        }

        private void AddOrderItem(SqlConnection connection, SqlTransaction transaction, int orderId, OrderItem item)
        {
            try
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
            catch (SqlException ex)
            {
                throw new Exception("Database error occurred while adding order item.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while adding order item.", ex);
            }
        }

        private void DecreaseItemStock(SqlConnection connection, SqlTransaction transaction, OrderItem item)
        {
            string query = "UPDATE MenuItem SET Stock = Stock - @Amount WHERE MenuItemId = @MenuItemId";

            using (SqlCommand cmd = new SqlCommand(query, connection, transaction))
            {
                cmd.Parameters.AddWithValue("@Amount", item.OrderItemQuantity);
                cmd.Parameters.AddWithValue("@MenuItemId", item.MenuItem.MenuItemId);
                cmd.ExecuteNonQuery();
            }
        }

        private void AddOrUpdateOrderItem(SqlConnection connection, SqlTransaction transaction, int orderId, OrderItem item)
        {
            try
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
            catch (SqlException ex)
            {
                throw new Exception("Database error occurred while adding or updating an order item.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while adding or updating an order item.", ex);
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
                        foreach (OrderItem item in items)
                        {
                            AddOrUpdateOrderItem(connection, transaction, order.OrderId, item);
                            DecreaseItemStock(connection, transaction, item);
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
        public Order? GetRunningTableOrder(int tableId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    string query = @"
                SELECT TOP 1 o.OrderId, o.TableId, o.EmployeeId, o.OrderTime, o.ServedTime, o.OrderStatus
                FROM [Order] o
                WHERE o.OrderStatus NOT IN (@Paid, @Cancelled, @Served, @Settled)
                AND o.TableId = @tableId
                ORDER BY o.OrderTime ASC";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@tableId", tableId);
                    command.Parameters.AddWithValue("@Paid", OrderStatus.Paid.ToString());
                    command.Parameters.AddWithValue("@Cancelled", OrderStatus.Cancelled.ToString());
                    command.Parameters.AddWithValue("@Served", OrderStatus.Served.ToString());
                    command.Parameters.AddWithValue("@Settled", OrderStatus.Settled.ToString());

                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Order order = ReadOrder(reader);
                            order.OrderItems = GetOrderItemsByOrderIdNoFilter(order);
                            return order;
                        }
                    }
                }

                return null;
            }
            catch (SqlException ex)
            {
                throw new Exception("Failed to retrieve running order.", ex);
            }
        }


        public void MarkOrderAsServed(int orderId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"  UPDATE [Order]
                              SET OrderStatus = 'Served', ServedTime = GETDATE()
                              WHERE OrderId = @orderId;

                              UPDATE OrderItem
                              SET OrderItemsStatus = 'Served'
                              WHERE OrderId = @orderId";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@orderId", orderId);

                connection.Open();

                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected == 0)
                {
                    throw new Exception("No record updated!");
                }
            }
        }

        public Order? GetActiveFoodOrDrinkOrder(int tableId, int isFood)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    string query = @"SELECT DISTINCT o.OrderId, o.TableId, o.EmployeeId, o.OrderTime, o.ServedTime, o.OrderStatus
                            FROM [Order] o
                            JOIN OrderItem oi ON o.OrderId = oi.OrderId
                            JOIN MenuItem mi ON oi.MenuItemId = mi.MenuItemId
                            WHERE o.TableId = @TableId
                            AND mi.IsFood = @IsFood
                            AND o.OrderStatus NOT IN (@Paid, @Cancelled, @Served, @Settled)";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@TableId", tableId);
                    command.Parameters.AddWithValue("@IsFood", isFood);
                    command.Parameters.AddWithValue("@Paid", OrderStatus.Paid.ToString());
                    command.Parameters.AddWithValue("@Cancelled", OrderStatus.Cancelled.ToString());
                    command.Parameters.AddWithValue("@Served", OrderStatus.Served.ToString());
                    command.Parameters.AddWithValue("@Settled", OrderStatus.Settled.ToString());

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
                throw new Exception("Database error occurred while fetching active food or drink order.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Unexpected error occurred while fetching active food or drink order.", ex);
            }
        }

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

        private Employee ReadEmployee(SqlDataReader reader)
        {
            Employee employee = new Employee();
            employee.EmployeeId = (int)reader["EmployeeId"];
            return employee;
        }

        private OrderItem ReadOrderItem(SqlDataReader reader)
        {
            OrderItem orderItem = new OrderItem();
            orderItem.OrderItemId = (int)reader["OrderItemId"];
            orderItem.OrderItemQuantity = (int)reader["OrderItemQuantity"];
            orderItem.Comment = reader.IsDBNull(reader.GetOrdinal("Comment")) ? string.Empty : reader["Comment"].ToString();
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
            menuItem.IsFood = reader["IsFood"] == DBNull.Value ? false : (bool)reader["IsFood"];
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