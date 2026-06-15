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
            try
            {
                List<Table> tables = new List<Table>();

                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    string query = @"SELECT TableId, TableCapacity, TableStatus FROM [Table]";


                    SqlCommand command = new SqlCommand(query, connection);
                    

                    connection.Open();

                    SqlDataReader reader = command.ExecuteReader();


                    while (reader.Read())
                    {
                        tables.Add(ReadTable(reader));
                    }
                }

                return tables;
            }
            catch (SqlException ex)
            {
                throw new Exception("Database error occurred while fetching tables.", ex);
            }
            
        }

        public void UpdateTableStatus(Table table)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"
                                    UPDATE [Table]
                                    SET TableStatus = @status
                                    WHERE TableId = @tableId";

                SqlCommand command = new SqlCommand(query, connection);
                

                command.Parameters.AddWithValue("@tableId", table.TableId);
                

                command.Parameters.AddWithValue("@status", table.TableStatus.ToString());
                

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

        private Table ReadTable(SqlDataReader reader)
        {
            Table table = new Table();
            table.TableId = Convert.ToInt32(reader["TableId"]);
            table.TableCapacity = Convert.ToInt32(reader["TableCapacity"]);
            table.TableStatus = Enum.Parse<TableStatus>(reader["TableStatus"].ToString());
            return table;                
        }


        /*
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
        // Private Method Helpers
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

        */
    }
}


    


