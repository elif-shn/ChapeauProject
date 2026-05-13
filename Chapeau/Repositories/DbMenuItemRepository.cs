using Chapeau.Enums;
using Chapeau.Models;
using Chapeau.ViewModels;
using Microsoft.Data.SqlClient;

namespace Chapeau.Repositories
{
    public class DbMenuItemRepository : IMenuItemRepository
    {
        private readonly string? _connectionString;
        public DbMenuItemRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ChapeauDataBase");
        }
        public List<MenuItem> GetAllByFilter(MenuFilterViewModel menuFilterViewModel)
        {
            List<MenuItem> menuItems = new List<MenuItem>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT MenuItemId, MenuItemName, MenuItemPrice, MenuId, Category, VatPercentage, Stock " +
                "FROM MenuItem " +
                "WHERE (@MenuId IS NULL OR MenuId = @MenuId) " +
                "AND (@Category IS NULL OR Category = @Category)";

                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@MenuId", (object?)menuFilterViewModel.SelectedMenuId ?? DBNull.Value);
                command.Parameters.AddWithValue("@Category", (object?)menuFilterViewModel.SelectedCategory ?? DBNull.Value);
                command.Connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    MenuItem menuItem = ReadMenuItem(reader);
                    menuItems.Add(menuItem);
                }
                reader.Close();
            }

            return menuItems;
        }
        private MenuItem ReadMenuItem(SqlDataReader reader)
        {
            int id = (int)reader["MenuItemId"];
            string name = (string)reader["MenuItemName"];
            decimal price = (decimal)reader["MenuItemPrice"];
            int menuId = (int)reader["MenuId"];
            Category category = (Category)(int)reader["Category"];
            int vatPercentage = (int)reader["VatPercentage"];
            int stock = (int)reader["Stock"];
            return new MenuItem(id, name, price, menuId, category, vatPercentage, stock);
        }
    }
}
