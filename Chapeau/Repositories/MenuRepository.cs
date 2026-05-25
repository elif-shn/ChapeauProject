using Chapeau.Enums;
using Chapeau.Models;
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

        public List<Menu> GetAllMenus()
        {
            List<Menu> menus = new List<Menu>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query =
                    "SELECT m.MenuId, m.Card, m.Category " +
                    "FROM Menu m";

                SqlCommand command = new SqlCommand(query, connection);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Menu menu = new Menu
                    {
                        MenuId = (int)reader["MenuId"],
                        Card = (Card)(int)reader["Card"],
                        Category = (Category)(int)reader["Category"],
                        MenuItems = new List<MenuItem>()
                    };

                    menus.Add(menu);
                }
            }

            return menus;
        }

        public MenuItem GetById(int id)
        {
            MenuItem item = null;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    string query =
                        "SELECT mi.MenuItemId, mi.MenuItemName, mi.MenuItemPrice, mi.MenuId, " +
                        "mi.VatPercentage, mi.Stock, mi.IsActive, " +
                        "m.Card, m.Category " +
                        "FROM MenuItem mi " +
                        "INNER JOIN Menu m ON mi.MenuId = m.MenuId " +
                        "WHERE mi.MenuItemId = @MenuItemId";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@MenuItemId", id);

                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        item = ReadMenuItem(reader);
                        item.Menu = ReadMenu(reader, (int)reader["MenuId"]);
                    }
                }
                catch (SqlException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return item;
        }
        public void Add(MenuItem item)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    int menuId = GetMenuId(item.Card, item.Category);

                    string query =
                        "INSERT INTO MenuItem (MenuItemName, MenuItemPrice, MenuId, VatPercentage, Stock, IsActive) " +
                        "VALUES (@MenuItemName, @MenuItemPrice, @MenuId, @VatPercentage, @Stock, 1)";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@MenuItemName", item.MenuItemName);
                    command.Parameters.AddWithValue("@MenuItemPrice", item.MenuItemPrice);
                    command.Parameters.AddWithValue("@MenuId", menuId);
                    command.Parameters.AddWithValue("@VatPercentage", item.VatPercentage);
                    command.Parameters.AddWithValue("@Stock", item.Stock);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        public void Update(MenuItem item)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    int menuId = GetMenuId(item.Card, item.Category);

                    string query =
                        "UPDATE MenuItem " +
                        "SET MenuItemName = @MenuItemName, " +
                        "MenuItemPrice = @MenuItemPrice, " +
                        "MenuId = @MenuId, " +
                        "VatPercentage = @VatPercentage, " +
                        "Stock = @Stock " +
                        "WHERE MenuItemId = @MenuItemId";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@MenuItemName", item.MenuItemName);
                    command.Parameters.AddWithValue("@MenuItemPrice", item.MenuItemPrice);
                    command.Parameters.AddWithValue("@MenuId", menuId);
                    command.Parameters.AddWithValue("@VatPercentage", item.VatPercentage);
                    command.Parameters.AddWithValue("@Stock", item.Stock);
                    command.Parameters.AddWithValue("@MenuItemId", item.MenuItemId);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private int GetMenuId(Card card, Category category)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT MenuId FROM Menu WHERE Card = @Card AND Category = @Category";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Card", (int)card);
                command.Parameters.AddWithValue("@Category", (int)category);

                connection.Open();
                object result = command.ExecuteScalar();

                if (result == null)
                    throw new Exception("MenuId not found for selected Card/Category");

                return (int)result;
            }
        }


        public void SetActive(int id, bool isActive)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    string query =
                        "UPDATE MenuItem SET IsActive = @IsActive " +
                        "WHERE MenuItemId = @MenuItemId";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@IsActive", isActive);
                    command.Parameters.AddWithValue("@MenuItemId", id);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    Console.WriteLine(ex.Message);
                }


            }

        }

        public void DecreaseStock(int menuItemId, int amount)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    string query =
                        "UPDATE MenuItem " +
                        "SET Stock = Stock - @Amount " +
                        "WHERE MenuItemId = @MenuItemId";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Amount", amount);
                    command.Parameters.AddWithValue("@MenuItemId", menuItemId);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
