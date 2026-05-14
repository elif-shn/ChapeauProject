using Chapeau.Models;
using Microsoft.Data.SqlClient;

namespace Chapeau.Repositories
{
    public class DbMenuRepository: IMenuRepository
    {
        private readonly string _connectionString;

        public DbMenuRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ChapeauDataBase");
        }

        public List<Menu> GetAllMenus()
        {
            List<Menu> menus = new List<Menu>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT MenuId, MenuName FROM Menu";

                SqlCommand command = new SqlCommand(query, connection);
                command.Connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    menus.Add(new Menu
                    {
                        MenuId = (int)reader["MenuId"],
                        MenuName = (string)reader["MenuName"]
                    });
                }

                reader.Close();
            }

            return menus;
        }
    }
}
