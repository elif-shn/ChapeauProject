using Chapeau.Enums;
using Chapeau.Models;
using Dapper;
using Microsoft.Data.SqlClient;
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
        private Menu ReadMenu(SqlDataReader reader, int menuId)
        {
            Menu menu = new Menu();

            menu.MenuId = menuId;
            menu.Card = (Card)(int)reader["Card"];
            menu.Category = (Category)(int)reader["Category"];
            menu.MenuItems = new List<MenuItem>();

            return menu;
        }
        private MenuItem ReadMenuItem(SqlDataReader reader)
        {
            MenuItem item = new MenuItem();

            item.MenuItemId = (int)reader["MenuItemId"];
            item.MenuItemName = (string)reader["MenuItemName"];
            item.MenuItemPrice = (decimal)reader["MenuItemPrice"];
            item.VatPercentage = (int)reader["VatPercentage"];
            item.Stock = (int)reader["Stock"];
            item.IsActive = (bool)reader["IsActive"];

            return item;
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
