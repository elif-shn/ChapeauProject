using Chapeau.Models;
using Microsoft.Data.SqlClient;
using Dapper;
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

       
        public List<MenuItem> GetAll()
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string sql = "SELECT * FROM MenuItem";
                return conn.Query<MenuItem>(sql).ToList();
            }
        }

        
        public List<MenuItem> GetByMenuId(int menuId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string sql = "SELECT * FROM MenuItem WHERE MenuId = @MenuId";
                return conn.Query<MenuItem>(sql, new { MenuId = menuId }).ToList();
            }
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
                string sql = @"INSERT INTO MenuItem (MenuItemName, MenuItemPrice, MenuId, Category, VatPercentage, Stock)
                               VALUES (@MenuItemName, @MenuItemPrice, @MenuId, @Category, @VatPercentage, @Stock)";
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
    }
}