using Chapeau.Enums;
using Chapeau.Models;
using Chapeau.Repositories.Interfaces;
using Microsoft.Data.SqlClient;

namespace Chapeau.Repositories
{
    public class DbOrderRepository : IOrderRepository
    {
        private readonly string _connectionString;

        public DbOrderRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ChapeauDatabase");
        }

        public Order? GetOrderById(Order order)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"SELECT OrderId, TableId, EmployeeId, OrderTime, ServedTime, OrderStatus
                                 FROM [Order]
                                 WHERE OrderId = @OrderId";

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

        public List<Order> GetRunningOrders()
        {
            List<Order> foodOrders = GetRunningOrders(true);
            List<Order> drinkOrders = GetRunningOrders(false);

            return foodOrders
                .Concat(drinkOrders)
                .GroupBy(order => order.OrderId)
                .Select(group => group.First())
                .ToList();
        }

        public List<Order> GetRunningOrders(bool isFood)
        {
            List<Order> orders = new List<Order>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"SELECT DISTINCT o.OrderId, o.TableId, o.EmployeeId, o.OrderTime, o.ServedTime, o.OrderStatus
                                 FROM [Order] o
                                 INNER JOIN OrderItem oi ON o.OrderId = oi.OrderId
                                 INNER JOIN MenuItem mi ON oi.MenuItemId = mi.MenuItemId
                                 WHERE mi.IsFood = @IsFood
                                 AND o.OrderStatus IN (@Ordered, @Pending, @Preparing)
                                 ORDER BY o.OrderTime ASC";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@IsFood", isFood);
                command.Parameters.AddWithValue("@Ordered", OrderStatus.Ordered.ToString());
                command.Parameters.AddWithValue("@Pending", OrderStatus.Pending.ToString());
                command.Parameters.AddWithValue("@Preparing", OrderStatus.Preparing.ToString());

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Order order = ReadOrder(reader);
                        order.OrderItems = GetOrderItemsByOrderId(order.OrderId, isFood);
                        orders.Add(order);
                    }
                }
            }

            return orders;
        }

        public List<Order> GetFinishedOrders()
        {
            List<Order> foodOrders = GetFinishedOrders(true);
            List<Order> drinkOrders = GetFinishedOrders(false);

            return foodOrders
                .Concat(drinkOrders)
                .GroupBy(order => order.OrderId)
                .Select(group => group.First())
                .ToList();
        }

        public List<Order> GetFinishedOrders(bool isFood)
        {
            List<Order> orders = new List<Order>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"SELECT DISTINCT o.OrderId, o.TableId, o.EmployeeId, o.OrderTime, o.ServedTime, o.OrderStatus
                                 FROM [Order] o
                                 INNER JOIN OrderItem oi ON o.OrderId = oi.OrderId
                                 INNER JOIN MenuItem mi ON oi.MenuItemId = mi.MenuItemId
                                 WHERE mi.IsFood = @IsFood
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
                        Order order = ReadOrder(reader);
                        order.OrderItems = GetOrderItemsByOrderId(order.OrderId, isFood);
                        orders.Add(order);
                    }
                }
            }

            return orders;
        }

        public void UpdateOrderStatus(Order order)
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

        public void UpdateOrderStatus(Order order, OrderStatus status)
        {
            order.OrderStatus = status;
            UpdateOrderStatus(order);
        }

        public void UpdateOrderItemStatus(OrderItem orderItem)
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

        public void UpdateOrderItemStatus(OrderItem orderItem, OrderItemStatus status)
        {
            orderItem.OrderItemStatus = status;
            UpdateOrderItemStatus(orderItem);
        }

        public void UpdateCourseStatus(Order order, Category category, OrderItemStatus status)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"UPDATE oi
                                 SET oi.OrderItemsStatus = @Status
                                 FROM OrderItem oi
                                 JOIN MenuItem mi ON oi.MenuItemId = mi.MenuItemId
                                 JOIN Menu m ON mi.MenuId = m.MenuId
                                 WHERE oi.OrderId = @OrderId
                                 AND m.Category = @Category";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@OrderId", order.OrderId);
                command.Parameters.AddWithValue("@Category", (int)category);
                command.Parameters.AddWithValue("@Status", status.ToString());

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public Order? GetActiveOrderForTable(int tableId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"SELECT OrderId, TableId, EmployeeId, OrderTime, ServedTime, OrderStatus
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

        public List<OrderItem> GetOrderItemsByOrderId(Order order)
        {
            List<OrderItem> foodItems = GetOrderItemsByOrderId(order.OrderId, true);
            List<OrderItem> drinkItems = GetOrderItemsByOrderId(order.OrderId, false);

            return foodItems.Concat(drinkItems).ToList();
        }

        private List<OrderItem> GetOrderItemsByOrderId(int orderId, bool isFood)
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

                        foreach (OrderItem item in order.OrderItems)
                        {
                            AddOrderItem(connection, transaction, newOrderId, item);
                            DecreaseItemStock(connection, transaction, item);
                        }

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception("An error occurred while saving the order and its items.", ex);
                    }
                }
            }
        }

        private int CreateNewOrder(SqlConnection connection, SqlTransaction transaction, Order order)
        {
            string query = @"INSERT INTO [Order] (TableId, EmployeeId, OrderTime, ServedTime, OrderStatus)
                             VALUES (@TableId, @EmployeeId, GETDATE(), NULL, @OrderStatus);
                             SELECT SCOPE_IDENTITY();";

            using (SqlCommand command = new SqlCommand(query, connection, transaction))
            {
                command.Parameters.AddWithValue("@TableId", order.TableId);
                command.Parameters.AddWithValue("@EmployeeId", order.Employee.EmployeeId);
                command.Parameters.AddWithValue("@OrderStatus", OrderStatus.Ordered.ToString());

                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        private void AddOrderItem(SqlConnection connection, SqlTransaction transaction, int orderId, OrderItem item)
        {
            string query = @"INSERT INTO OrderItem (OrderId, MenuItemId, OrderItemQuantity, Comment, OrderItemsStatus)
                             VALUES (@OrderId, @MenuItemId, @Quantity, @Comment, @Status)";

            using (SqlCommand command = new SqlCommand(query, connection, transaction))
            {
                command.Parameters.AddWithValue("@OrderId", orderId);
                command.Parameters.AddWithValue("@MenuItemId", item.MenuItem.MenuItemId);
                command.Parameters.AddWithValue("@Quantity", item.OrderItemQuantity);
                command.Parameters.AddWithValue("@Comment", item.Comment ?? "");
                command.Parameters.AddWithValue("@Status", OrderItemStatus.Ordered.ToString());

                command.ExecuteNonQuery();
            }
        }

        private void DecreaseItemStock(SqlConnection connection, SqlTransaction transaction, OrderItem item)
        {
            string query = @"UPDATE MenuItem
                             SET Stock = Stock - @Amount
                             WHERE MenuItemId = @MenuItemId";

            using (SqlCommand command = new SqlCommand(query, connection, transaction))
            {
                command.Parameters.AddWithValue("@Amount", item.OrderItemQuantity);
                command.Parameters.AddWithValue("@MenuItemId", item.MenuItem.MenuItemId);

                command.ExecuteNonQuery();
            }
        }

        private void AddOrUpdateOrderItem(SqlConnection connection, SqlTransaction transaction, int orderId, OrderItem item)
        {
            string query = @"IF EXISTS (
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
                                INSERT INTO OrderItem (OrderId, MenuItemId, OrderItemQuantity, Comment, OrderItemsStatus)
                                VALUES (@OrderId, @MenuItemId, @Quantity, @Comment, @Status)
                             END";

            using (SqlCommand command = new SqlCommand(query, connection, transaction))
            {
                command.Parameters.AddWithValue("@OrderId", orderId);
                command.Parameters.AddWithValue("@MenuItemId", item.MenuItem.MenuItemId);
                command.Parameters.AddWithValue("@Quantity", item.OrderItemQuantity);
                command.Parameters.AddWithValue("@Comment", item.Comment ?? "");
                command.Parameters.AddWithValue("@Status", OrderItemStatus.Ordered.ToString());

                command.ExecuteNonQuery();
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
                        throw new Exception("An error occurred while adding items to existing order.", ex);
                    }
                }
            }
        }

        public List<Order> GetRunningTableOrders(int tableId)
        {
            List<Order> orders = new List<Order>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"SELECT o.OrderId, o.TableId, o.EmployeeId, o.OrderTime, o.ServedTime, o.OrderStatus
                                 FROM [Order] o
                                 WHERE o.OrderStatus NOT IN (@Paid, @Cancelled, @Served, @Settled)
                                 AND o.TableId = @TableId
                                 ORDER BY o.OrderTime ASC";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Paid", OrderStatus.Paid.ToString());
                command.Parameters.AddWithValue("@Cancelled", OrderStatus.Cancelled.ToString());
                command.Parameters.AddWithValue("@Served", OrderStatus.Served.ToString());
                command.Parameters.AddWithValue("@Settled", OrderStatus.Settled.ToString());
                command.Parameters.AddWithValue("@TableId", tableId);

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        orders.Add(ReadOrder(reader));
                    }
                }
            }

            foreach (Order order in orders)
            {
                order.OrderItems = GetOrderItemsByOrderId(order);
            }

            return orders;
        }

        public void MarkOrderAsServed(int orderId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"UPDATE [Order]
                                 SET OrderStatus = @Status,
                                     ServedTime = GETDATE()
                                 WHERE OrderId = @OrderId";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Status", OrderStatus.Served.ToString());
                command.Parameters.AddWithValue("@OrderId", orderId);

                connection.Open();

                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected == 0)
                {
                    throw new Exception("No order was updated.");
                }
            }
        }

        public List<Order> GetActiveFoodOrders(int tableId)
        {
            return GetActiveOrdersByFoodType(tableId, true);
        }

        public List<Order> GetActiveDrinkOrders(int tableId)
        {
            return GetActiveOrdersByFoodType(tableId, false);
        }

        private List<Order> GetActiveOrdersByFoodType(int tableId, bool isFood)
        {
            List<Order> orders = new List<Order>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"SELECT DISTINCT o.OrderId, o.TableId, o.EmployeeId, o.OrderTime, o.ServedTime, o.OrderStatus
                                 FROM [Order] o
                                 JOIN OrderItem oi ON o.OrderId = oi.OrderId
                                 JOIN MenuItem mi ON oi.MenuItemId = mi.MenuItemId
                                 WHERE o.OrderStatus NOT IN (@Paid, @Cancelled, @Served, @Settled)
                                 AND mi.IsFood = @IsFood
                                 AND o.TableId = @TableId";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Paid", OrderStatus.Paid.ToString());
                command.Parameters.AddWithValue("@Cancelled", OrderStatus.Cancelled.ToString());
                command.Parameters.AddWithValue("@Served", OrderStatus.Served.ToString());
                command.Parameters.AddWithValue("@Settled", OrderStatus.Settled.ToString());
                command.Parameters.AddWithValue("@IsFood", isFood);
                command.Parameters.AddWithValue("@TableId", tableId);

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        orders.Add(ReadOrder(reader));
                    }
                }
            }

            return orders;
        }

        private Order ReadOrder(SqlDataReader reader)
        {
            Order order = new Order();

            order.OrderId = (int)reader["OrderId"];
            order.OrderTime = (DateTime)reader["OrderTime"];
            order.ServedTime = reader["ServedTime"] == DBNull.Value ? null : (DateTime?)reader["ServedTime"];
            order.OrderStatus = Enum.Parse<OrderStatus>(reader["OrderStatus"].ToString());

            order.Table = new Table
            {
                TableId = (int)reader["TableId"]
            };

            order.Employee = new Employee
            {
                EmployeeId = (int)reader["EmployeeId"]
            };

            return order;
        }

        private OrderItem ReadOrderItem(SqlDataReader reader)
        {
            OrderItem item = new OrderItem();

            item.OrderItemId = (int)reader["OrderItemId"];
            item.OrderItemQuantity = (int)reader["OrderItemQuantity"];
            item.Comment = reader["Comment"].ToString();
            item.OrderItemStatus = Enum.Parse<OrderItemStatus>(reader["OrderItemsStatus"].ToString());

            item.MenuItem = new MenuItem
            {
                MenuItemId = (int)reader["MenuItemId"],
                MenuItemName = (string)reader["MenuItemName"],
                MenuItemPrice = (decimal)reader["MenuItemPrice"],
                VatPercentage = (int)reader["VatPercentage"],
                IsFood = (bool)reader["IsFood"],
                Menu = new Menu
                {
                    MenuId = (int)reader["MenuId"],
                    Category = (Category)(int)reader["Category"],
                    Card = (Card)(int)reader["Card"]
                }
            };

            return item;
        }
    }
}