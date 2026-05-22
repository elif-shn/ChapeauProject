using Chapeau.Enums;
using Chapeau.Models;
using Microsoft.Data.SqlClient;

namespace Chapeau.Repositories
{
    public class DbMenuListRepository : IMenuListRepository

    {
        private readonly string _connectionString;

        public DbMenuListRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ChapeauDataBase");
        }

        public List<Menu> GetAllMenus()
        {
            List<Menu> menus = new List<Menu>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT MenuId, category FROM Menu";

                SqlCommand command = new SqlCommand(query, connection);
                command.Connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    menus.Add(new Menu
                    {
                        MenuId = (int)reader["MenuId"],
                        Category = (Category)(int)reader["Category"]
                    });
                }

                reader.Close();
            }

            return menus;
        }
    }
}

       