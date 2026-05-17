using Chapeau.Enums;
using Chapeau.Models;
using Chapeau.ViewModels;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
namespace Chapeau.Repositories
{
    public class MenuRepository : IMenuRepository
    {
        private string _connectionString;

        public MenuRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        /* public List<MenuItem> GetAll()
         {
             using (SqlConnection conn = new SqlConnection(_connectionString))
             {
                 string sql = "SELECT * FROM MenuItem";
                 return conn.Query<MenuItem>(sql).ToList();
             }
         }

         public List<MenuItem> GetByFilter(int menuId, int category)
         {
             using (SqlConnection conn = new SqlConnection(_connectionString))
             {
                 string sql = "SELECT * FROM MenuItem WHERE 1=1";
                 if (menuId != 0) sql += " AND MenuId = @MenuId";
                 if (category != 0) sql += " AND Category = @Category";
                 return conn.Query<MenuItem>(sql, new { MenuId = menuId, Category = category }).ToList();
             }
         }*/

        public List<MenuItem> GetAllByFilter(MenuViewModel menuViewModel)
        {
            List<MenuItem> menuItems = new List<MenuItem>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT MenuItemId, MenuItemName, MenuItemPrice, MenuId, Category, VatPercentage, Stock, IsActive " +
                "FROM MenuItem " +
                "WHERE (@MenuId IS NULL OR MenuId = @MenuId) " +
                "AND (@Category IS NULL OR Category = @Category)";

                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@MenuId", (object?)menuViewModel.SelectedMenuId ?? DBNull.Value);
                command.Parameters.AddWithValue("@Category", (object?)menuViewModel.SelectedCategory ?? DBNull.Value);
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
            bool isActive = (bool)reader["isActive"];
            return new MenuItem(id, name, price, menuId, category, vatPercentage, stock, isActive);
        }

        public MenuItem GetById(int id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string sql = "SELECT * FROM MenuItem WHERE MenuItemId = @MenuItemId";
                return conn.QuerySingleOrDefault<MenuItem>(sql, new { MenuItemId = id });
            }
        }

        public void Add(MenuItem item)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string sql = @"INSERT INTO MenuItem (MenuItemName, MenuItemPrice, MenuId, Category, VatPercentage, Stock, IsActive)
                               VALUES (@MenuItemName, @MenuItemPrice, @MenuId, @Category, @VatPercentage, @Stock, 1)";
                conn.Execute(sql, item);
            }
        }

        public void Update(MenuItem item)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string sql = @"UPDATE MenuItem 
                               SET MenuItemName = @MenuItemName,
                                   MenuItemPrice = @MenuItemPrice,
                                   MenuId = @MenuId,
                                   Category = @Category,
                                   VatPercentage = @VatPercentage,
                                   Stock = @Stock
                               WHERE MenuItemId = @MenuItemId";
                conn.Execute(sql, item);
            }
        }

        public void SetActive(int id, bool isActive)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string sql = "UPDATE MenuItem SET IsActive = @IsActive WHERE MenuItemId = @MenuItemId";
                conn.Execute(sql, new { MenuItemId = id, IsActive = isActive });
            }
        }
    }
}