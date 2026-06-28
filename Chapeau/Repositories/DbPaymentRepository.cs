using Chapeau.Enums;
using Chapeau.Models;
using Chapeau.Repositories.Interfaces;
using Microsoft.Data.SqlClient;

namespace Chapeau.Repositories
{
    public class DbPaymentRepository : IPaymentRepository
    {
        private readonly string _connectionString;

        public DbPaymentRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ChapeauDataBase");
        }

        public List<Table> GetAllTables()
        {
            List<Table> tables = new List<Table>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT TableId, TableCapacity, TableStatus FROM [Table]";

                SqlCommand command = new SqlCommand(query, connection);

                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Table table = new Table();

                    table.TableId = Convert.ToInt32(reader["TableId"]);
                    table.TableCapacity = Convert.ToInt32(reader["TableCapacity"]);
                    table.TableStatus = Enum.Parse<TableStatus>(reader["TableStatus"].ToString());

                    tables.Add(table);
                }

                reader.Close();
            }

            return tables;
        }

        public Order? GetActiveOrderByTableId(int tableId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query =
                    "SELECT TOP 1 OrderId, TableId, OrderTime, OrderStatus " +
                    "FROM [Order] " +
                    "WHERE TableId = @TableId " +
                    "AND OrderStatus NOT IN ('Paid', 'Cancelled') " +
                    "ORDER BY OrderTime DESC";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@TableId", tableId);

                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    Order order = new Order();

                    order.OrderId = Convert.ToInt32(reader["OrderId"]);
                    order.TableId = Convert.ToInt32(reader["TableId"]);
                    order.OrderTime = Convert.ToDateTime(reader["OrderTime"]);

                    string status = reader["OrderStatus"].ToString() ?? "Ordered";
                    order.OrderStatus = Enum.Parse<OrderStatus>(status);

                    order.Table = new Table();
                    order.Table.TableId = order.TableId;

                    reader.Close();

                    return order;
                }

                reader.Close();
            }

            return null;
        }

        public List<OrderItem> GetOrderItemsByOrderId(int orderId)
        {
            List<OrderItem> orderItems = new List<OrderItem>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query =
                    "SELECT oi.OrderItemId, oi.OrderItemQuantity, oi.Comment, oi.OrderItemsStatus, " +
                    "mi.MenuItemId, mi.MenuItemName, mi.MenuItemPrice, mi.VatPercentage " +
                    "FROM OrderItem oi " +
                    "INNER JOIN MenuItem mi ON oi.MenuItemId = mi.MenuItemId " +
                    "WHERE oi.OrderId = @OrderId";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@OrderId", orderId);

                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    OrderItem orderItem = new OrderItem();

                    orderItem.OrderItemId = Convert.ToInt32(reader["OrderItemId"]);
                    orderItem.OrderItemQuantity = Convert.ToInt32(reader["OrderItemQuantity"]);
                    orderItem.Comment = reader["Comment"].ToString() ?? "";

                    string status = reader["OrderItemsStatus"].ToString() ?? "Ordered";
                    orderItem.OrderItemStatus = Enum.Parse<OrderItemStatus>(status);

                    MenuItem menuItem = new MenuItem();

                    menuItem.MenuItemId = Convert.ToInt32(reader["MenuItemId"]);
                    menuItem.MenuItemName = reader["MenuItemName"].ToString() ?? "";
                    menuItem.MenuItemPrice = Convert.ToDecimal(reader["MenuItemPrice"]);
                    menuItem.VatPercentage = Convert.ToInt32(reader["VatPercentage"]);

                    orderItem.MenuItem = menuItem;

                    orderItems.Add(orderItem);
                }

                reader.Close();
            }

            return orderItems;
        }

        public void SavePayment(Payment payment)
        {
            if (payment.Order == null)
            {
                throw new Exception("Order not found.");
            }

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query =
                    "INSERT INTO Payment " +
                    "(OrderId, TotalAmount, TipAmount, Vat9, Vat21, PaymentMethod, Feedback, PaymentDate) " +
                    "VALUES " +
                    "(@OrderId, @TotalAmount, @TipAmount, @Vat9, @Vat21, @PaymentMethod, @Feedback, @PaymentDate)";

                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@OrderId", payment.Order.OrderId);
                command.Parameters.AddWithValue("@TotalAmount", payment.TotalAmount);
                command.Parameters.AddWithValue("@TipAmount", payment.TipAmount);
                command.Parameters.AddWithValue("@Vat9", payment.Vat9);
                command.Parameters.AddWithValue("@Vat21", payment.Vat21);
                command.Parameters.AddWithValue("@PaymentMethod", (int)payment.PaymentMethod);
                command.Parameters.AddWithValue("@Feedback", payment.Feedback ?? "");
                command.Parameters.AddWithValue("@PaymentDate", payment.PaymentDate);

                connection.Open();

                command.ExecuteNonQuery();
            }
        }

        public void UpdateOrderStatusToPaid(int orderId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "UPDATE [Order] SET OrderStatus = @OrderStatus WHERE OrderId = @OrderId";

                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@OrderStatus", OrderStatus.Paid.ToString());
                command.Parameters.AddWithValue("@OrderId", orderId);

                connection.Open();

                command.ExecuteNonQuery();
            }
        }

        public void UpdateTableStatusToFree(int tableId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "UPDATE [Table] SET TableStatus = @TableStatus WHERE TableId = @TableId";

                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@TableStatus", "Free");
                command.Parameters.AddWithValue("@TableId", tableId);

                connection.Open();

                command.ExecuteNonQuery();
            }
        }
    }
}