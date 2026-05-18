using Chapeau.Enums;
using Chapeau.Models;
using Chapeau.ViewModels;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
namespace Chapeau.Repositories
{
    public class MenuRepository : IMenuRepository
    {
        private string _connectionString;

        public MenuRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ChapeauDataBase");
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

        public List<MenuItem> GetAllByFilter(int? menuId, Category? category)
        {
            List<MenuItem> menuItems = new List<MenuItem>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT mi.MenuItemId, mi.MenuItemName, mi.MenuItemPrice, mi.MenuId, mi.Category, mi.VatPercentage, mi.Stock, mi.IsActive, m.MenuName " +
                "FROM MenuItem mi " +
                "JOIN Menu m ON mi.MenuId = m.MenuId "+
                "WHERE (@MenuId IS NULL OR mi.MenuId = @MenuId) " +
                "AND (@Category IS NULL OR mi.Category = @Category)";

                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.Add("@MenuId", SqlDbType.Int).Value = (object?)menuId ?? DBNull.Value;
                command.Parameters.Add("@Category", SqlDbType.Int).Value = (object?)category ?? DBNull.Value;
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
            Menu menu = new Menu
            {
                MenuId = (int)reader["MenuId"],
                MenuName = (string)reader["MenuName"]
            };
            Category category = (Category)(int)reader["Category"];
            int vatPercentage = (int)reader["VatPercentage"];
            int stock = (int)reader["Stock"];
            bool isActive = (bool)reader["isActive"];
            return new MenuItem(id, name, price, menu, category, vatPercentage, stock, isActive);
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