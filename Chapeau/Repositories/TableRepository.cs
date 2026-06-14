using Chapeau.Enums;
using Chapeau.Models;
using Chapeau.Repositories.Interfaces;
using Chapeau.ViewModels;
using Microsoft.Data.SqlClient;


namespace Chapeau.Repositories
{
    public class TableRepository : ITableRepository
    {
        private readonly string _connectionString;

        public TableRepository(IConfiguration configuration)
        {
            _connectionString =
                configuration.GetConnectionString("ChapeauDataBase");

        }

        public List<Table> GetAllTables()
        {
            List<Table> tables = new List<Table>();

            using (SqlConnection connection =
                   new SqlConnection(_connectionString))
            {
                string query = @"SELECT TableId, TableCapacity, TableStatus FROM [Table]";


                SqlCommand command =
                    new SqlCommand(query, connection);

                connection.Open();

                SqlDataReader reader =
                    command.ExecuteReader();

                while (reader.Read())
                {
                    Table table = new Table
                    {
                        TableId =
                            Convert.ToInt32(
                                reader["TableId"]),

                        TableCapacity =
                            Convert.ToInt32(
                                reader["TableCapacity"]),

                        TableStatus = Enum.Parse<TableStatus>(reader["TableStatus"].ToString())

                    };

                    tables.Add(table);
                }
            }

            return tables;
        }
       
        public List<ActiveOrderViewModel> GetActiveOrders(int tableId)
        {
            List<ActiveOrderViewModel> orders = new List<ActiveOrderViewModel>();
            

            using (SqlConnection connection = new SqlConnection(_connectionString)) 
            {
                string query = @"
                            SELECT
                                o.TableId,
                                o.OrderStatus,
                                o.OrderId,

                                MAX(
                                    CASE
                                        WHEN m.Card IN (1,2)
                                        THEN 1
                                        ELSE 0
                                    END
                                ) AS HasFood,

                                MAX(
                                    CASE
                                        WHEN m.Card = 3
                                        THEN 1
                                        ELSE 0
                                    END
                                ) AS HasDrink

                            FROM [Order] o

                            JOIN OrderItem oi
                                ON o.OrderId = oi.OrderId

                            JOIN MenuItem mi
                                ON oi.MenuItemId = mi.MenuItemId

                            JOIN Menu m
                                ON mi.MenuId = m.MenuId

                            WHERE
                                o.OrderStatus
                                    NOT IN ('Served', 'Paid')

                                AND o.TableId = @tableId

                            GROUP BY
                                o.TableId,
                                o.OrderStatus,
                                o.OrderId";

                SqlCommand command = new SqlCommand(query, connection);

                

                command.Parameters.AddWithValue("@tableId", tableId);
                

                connection.Open();

                SqlDataReader reader = command.ExecuteReader();
                

                while (reader.Read())
                {
                    bool hasFood = 
                        Convert.ToInt32(
                            reader["HasFood"]) == 1;

                    bool hasDrink =
                        Convert.ToInt32(
                            reader["HasDrink"]) == 1;

                    string orderType;

                    if (hasFood && hasDrink)
                    {
                        orderType = "Food & Drink";
                    }
                    else if (hasFood)
                    {
                        orderType = "Food";
                    }
                    else
                    {
                        orderType = "Drink";
                    }

                    orders.Add(
                        new ActiveOrderViewModel
                        {
                            OrderId = Convert.ToInt32(reader["OrderId"]),
                            

                            TableId = Convert.ToInt32(reader["TableId"]),

                            OrderStatus = reader["OrderStatus"].ToString(),
                            

                            OrderType = orderType                           
                        });
                }
            }

            return orders;
        }

        public void MarkOrderAsServed(int orderId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"
                UPDATE [Order]
                SET
                    OrderStatus = 'Served',
                    ServedTime = GETDATE()
                WHERE OrderId = @orderId";

                SqlCommand command =
                    new SqlCommand(query, connection);

                command.Parameters.AddWithValue(
                    "@orderId",
                    orderId);

                connection.Open();

                int noOfRowsAffected = command.ExecuteNonQuery();
                if (noOfRowsAffected == 0)
                {
                    throw new Exception("No record updated!");
                }
            }

        }

        public bool HasActiveOrders(int tableId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
                
            {
                string query = @"
                                SELECT COUNT(*)
                                FROM [Order]
                                WHERE TableId = @tableId
                                  AND OrderStatus NOT IN ('Served', 'Paid')";

                SqlCommand command = new SqlCommand(query, connection);
                

                command.Parameters.AddWithValue("@tableId", tableId);
                

                connection.Open();

                int count = Convert.ToInt32(command.ExecuteScalar());
                

                return count > 0;
            }
        }

        public void UpdateTableStatus(Table table)
        {
            using (SqlConnection connection =
                new SqlConnection(_connectionString))
            {
                string query = @"
                                    UPDATE [Table]
                                    SET TableStatus = @status
                                    WHERE TableId = @tableId";

                SqlCommand command =
                    new SqlCommand(query, connection);

                command.Parameters.AddWithValue(
                    "@tableId",
                    table.TableId);

                command.Parameters.AddWithValue(
                    "@status",
                    table.TableStatus.ToString());

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
        

        
        public List<Order> GetRunningTableOrders(int tableId)
        {
            try
            {
                List<Order> orders = new List<Order>();

                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    string query = $@"SELECT o.OrderId, o.TableId, o.EmployeeId, o.OrderTime, o.ServedTime, o.OrderStatus 
                  FROM [Order] o WHERE o.OrderStatus NOT IN ('{OrderStatus.Paid}', '{OrderStatus.Cancelled}', '{OrderStatus.Served}', '{OrderStatus.Settled}')
                                                     AND o.TableId = @tableId
                  ORDER BY o.OrderTime ASC";


                    SqlCommand command = new SqlCommand(query, connection);

                    command.Parameters.AddWithValue("@tableId", tableId);

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
            orderItem.MenuItem = new MenuItem()
            {
                MenuItemId = (int)reader["MenuItemId"],
                

                MenuItemName = reader["MenuItemName"].ToString(),
                

                MenuItemPrice = (decimal)reader["MenuItemPrice"],
                

                VatPercentage = Convert.ToInt32(reader["VatPercentage"])
            
            };
            return orderItem;
        }
        

    }
}


    


