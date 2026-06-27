using Chapeau.Enums;
using Chapeau.Models;
using Chapeau.Repositories.Interfaces;
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
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        public List<Menu> GetMenus(Card? card, Category? category, bool onlyActive)
        {
            try
            {
                Dictionary<int, Menu> menus = new Dictionary<int, Menu>();

                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    string query =
                        @"SELECT mi.MenuItemId, mi.MenuItemName, mi.MenuItemPrice, mi.MenuId,
                         mi.VatPercentage, mi.Stock, mi.IsActive,
                         m.Card, m.Category
                  FROM MenuItem mi
                  INNER JOIN Menu m ON mi.MenuId = m.MenuId
                  WHERE (@Card IS NULL OR m.Card = @Card)
                  AND (@Category IS NULL OR m.Category = @Category)
                  AND (@OnlyActive = 0 OR mi.IsActive = 1)";

                    SqlCommand command = new SqlCommand(query, connection);

                    command.Parameters.AddWithValue("@Card", (object?)card ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Category", (object?)category ?? DBNull.Value);
                    command.Parameters.AddWithValue("@OnlyActive", onlyActive);

                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
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
                }

                return menus.Values.ToList();
            }
            catch (SqlException ex)
            {
                throw new Exception("Database error occurred while retrieving menus.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Unexpected error occurred while retrieving menus.", ex);
            }
        }
        //These columns are defined as NOT NULL in the database, therefore IsDBNull checks are unnecessary.
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
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query =
                    @"SELECT mi.MenuItemId, mi.MenuItemName, mi.MenuItemPrice, mi.MenuId,
              mi.VatPercentage, mi.Stock, mi.IsActive,
              m.Card, m.Category
              FROM MenuItem mi
              INNER JOIN Menu m ON mi.MenuId = m.MenuId
              WHERE mi.MenuItemId = @MenuItemId";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@MenuItemId", id);

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        int menuId = (int)reader["MenuId"];
                        Menu menu = ReadMenu(reader, menuId);
                        MenuItem item = ReadMenuItem(reader);
                        item.Menu = menu;
                        return item;
                    }
                    return null;
                }
            }
        }

        public void Add(MenuItem item, int selectedCard, int selectedCategory)
        {
            string sql = @"
        INSERT INTO MenuItem (MenuItemName, MenuItemPrice, MenuId, VatPercentage, Stock, IsActive)
        SELECT @MenuItemName, @MenuItemPrice, m.MenuId, @VatPercentage, @Stock, 1
        FROM Menu m
        WHERE m.Card = @SelectedCard AND m.Category = @SelectedCategory";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@MenuItemName", item.MenuItemName);
                    cmd.Parameters.AddWithValue("@MenuItemPrice", item.MenuItemPrice);
                    cmd.Parameters.AddWithValue("@VatPercentage", item.VatPercentage);
                    cmd.Parameters.AddWithValue("@Stock", item.Stock);

                    cmd.Parameters.AddWithValue("@SelectedCard", selectedCard);
                    cmd.Parameters.AddWithValue("@SelectedCategory", selectedCategory);

                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Update(MenuItem item, int selectedCard, int selectedCategory)
        {
            string sql = @"
        UPDATE MenuItem 
        SET MenuItemName = @MenuItemName,
            MenuItemPrice = @MenuItemPrice,
            MenuId = (SELECT m.MenuId FROM Menu m WHERE m.Card = @SelectedCard AND m.Category = @SelectedCategory),
            VatPercentage = @VatPercentage,
            Stock = @Stock
        WHERE MenuItemId = @MenuItemId";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@MenuItemId", item.MenuItemId);
                    cmd.Parameters.AddWithValue("@MenuItemName", item.MenuItemName);
                    cmd.Parameters.AddWithValue("@MenuItemPrice", item.MenuItemPrice);
                    cmd.Parameters.AddWithValue("@VatPercentage", item.VatPercentage);
                    cmd.Parameters.AddWithValue("@Stock", item.Stock);

                    cmd.Parameters.AddWithValue("@SelectedCard", selectedCard);
                    cmd.Parameters.AddWithValue("@SelectedCategory", selectedCategory);

                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }

                    cmd.ExecuteNonQuery();
                }
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