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
        public List<Table> GetOccupiedTables()
        {
            List<Table> tables = new List<Table>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"SELECT * FROM [Table] WHERE TableStatus = 'Occupied'";

                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    tables.Add(new Table
                    {
                        TableId = Convert.ToInt32(reader["TableId"]),
                        TableCapacity = Convert.ToInt32(reader["TableCapacity"]),
                        TableStatus = Enum.Parse<TableStatus>(reader["TableStatus"].ToString())
                    });
                }
            }

            return tables;
        }

        public List<ActiveOrderViewModel> GetActiveOrders(int tableId)
        {
            List<ActiveOrderViewModel> orders =
                new List<ActiveOrderViewModel>();

            using (SqlConnection connection =
                new SqlConnection(_connectionString))
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

                SqlCommand command =
                    new SqlCommand(query, connection);

                command.Parameters.AddWithValue(
                    "@tableId",
                    tableId);

                connection.Open();

                SqlDataReader reader =
                    command.ExecuteReader();

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
                            OrderId =
                                Convert.ToInt32(
                                    reader["OrderId"]),

                            TableId = Convert.ToInt32(reader["TableId"]),

                            OrderStatus =
                                reader["OrderStatus"]
                                    .ToString(),

                            OrderType =
                                orderType
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
    }
}


    


