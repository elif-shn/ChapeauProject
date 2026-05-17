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

        public List<Menu> GetAll()
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

        void IMenuRepository.Add(MenuItem item)
        {
            throw new NotImplementedException();
        }

        List<MenuItem> IMenuRepository.GetAll()
        {
            throw new NotImplementedException();
        }

        List<MenuItem> IMenuRepository.GetByFilter(int menuId, int category)
        {
            throw new NotImplementedException();
        }

        MenuItem IMenuRepository.GetById(int id)
        {
            throw new NotImplementedException();
        }

        void IMenuRepository.SetActive(int id, bool isActive)
        {
            throw new NotImplementedException();
        }

        void IMenuRepository.Update(MenuItem item)
        {
            throw new NotImplementedException();
        }
    }
}

       