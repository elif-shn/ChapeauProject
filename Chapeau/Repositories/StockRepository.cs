using Chapeau.Enums;
using Chapeau.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace Chapeau.Repositories
{
    public class StockRepository : IStockRepository
    {
        private readonly string _connectionString;

        public StockRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

       
        public List<Menu> GetAllByFilter(Card? card, Category? category)
        {
            Dictionary<int, Menu> menus = new Dictionary<int, Menu>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query =
                    "SELECT mi.MenuItemId, mi.MenuItemName, mi.MenuItemPrice, mi.MenuId, " +
                    "mi.VatPercentage, mi.Stock, mi.IsActive, " +
                    "m.Card, m.Category " +
                    "FROM MenuItem mi " +
                    "INNER JOIN Menu m ON mi.MenuId = m.MenuId " +
                    "WHERE (@Card IS NULL OR m.Card = @Card) " +
                    "AND (@Category IS NULL OR m.Category = @Category)";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.Add("@Card", SqlDbType.Int).Value = (object?)card ?? DBNull.Value;
                command.Parameters.Add("@Category", SqlDbType.Int).Value = (object?)category ?? DBNull.Value;

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    int menuId = (int)reader["MenuId"];

                    if (!menus.ContainsKey(menuId))
                    {
                        menus[menuId] = ReadMenu(reader, menuId);
                    }

                    MenuItem item = ReadMenuItem(reader);
                    item.Menu = menus[menuId];
                    menus[menuId].MenuItems.Add(item);
                }
            }

            return menus.Values.ToList();
        }

        public void UpdateStock(int menuItemId, int newStock)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "UPDATE MenuItem SET Stock = @Stock WHERE MenuItemId = @MenuItemId";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@MenuItemId", menuItemId);
                command.Parameters.AddWithValue("@Stock", newStock);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        private Menu ReadMenu(SqlDataReader reader, int menuId)
        {
            return new Menu
            {
                MenuId = menuId,
                Card = (Card)(int)reader["Card"],
                Category = (Category)(int)reader["Category"],
                MenuItems = new List<MenuItem>()
            };
        }

        private MenuItem ReadMenuItem(SqlDataReader reader)
        {
            return new MenuItem
            {
                MenuItemId = (int)reader["MenuItemId"],
                MenuItemName = (string)reader["MenuItemName"],
                MenuItemPrice = (decimal)reader["MenuItemPrice"],
                VatPercentage = (int)reader["VatPercentage"],
                Stock = (int)reader["Stock"],
                IsActive = (bool)reader["IsActive"]
            };
        }
    }
}