using Chapeau.Enums;
using Chapeau.Models;
using Microsoft.Data.SqlClient;

namespace Chapeau.Repositories
{
    public class TakeOrderRepository :ITakeOrderRepository
    {
        private readonly string _connectionString;

        public TakeOrderRepository(IConfiguration configuration)
        {
            _connectionString =configuration.GetConnectionString("ChapeauDataBase");
        }
        public Order GetActiveOrderByTable(int tableId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query ="SELECT TOP 1 * FROM [Order] WHERE TableId = @TableId "
                              +"AND OrderStatus != 'Paid' "
                              +"ORDER BY OrderId DESC";

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
        public int CreateOrder(int tableId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "INSERT INTO [Order] (TableId, EmployeeId, OrderTime, ServedTime, OrderStatus) " +
                                "VALUES (@TableId, @EmployeeId, GETDATE(), NULL, 'Pending') " +
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
                string query ="SELECT 1 FROM OrderItem "+
                              "WHERE OrderId = @OrderId "+
                              "AND MenuItemId = @MenuItemId "+
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
                string query = "UPDATE OrderItem  SET OrderItemQuantity = OrderItemQuantity + 1 "+
                               "WHERE OrderId = @OrderId "+
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
            using (SqlConnection connection =
                new SqlConnection(_connectionString))
            {
                string query = "INSERT INTO OrderItem (OrderId, MenuItemId, OrderItemQuantity,Comment,OrderItemsStatus) "+
                               "VALUES (@OrderId,@MenuItemId,1,@Comment,'Pending')";

                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@OrderId", orderId);

                command.Parameters.AddWithValue("@MenuItemId", menuItemId);
                command.Parameters.AddWithValue("@Comment", comment);

                connection.Open();

                command.ExecuteNonQuery();
            }
        }
        public void DecreaseStock(int menuItemId, int amount)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "UPDATE MenuItem " +
                                "SET Stock = Stock - @Amount " +
                                "WHERE MenuItemId = @MenuItemId";

                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@Amount", amount);
                command.Parameters.AddWithValue("@MenuItemId", menuItemId);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }
    }
}
