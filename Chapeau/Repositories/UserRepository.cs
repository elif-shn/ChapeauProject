using Microsoft.Data.SqlClient;
using Chapeau.Models;

namespace Chapeau.Repositories
{
    public class UserRepository : IUserRepository
    {
        
        private readonly string _connectionString;

        public UserRepository(IConfiguration configuration)
        {
            _connectionString =
                configuration.GetConnectionString("ChapeauDataBase");
        }

        public User? GetByUsernameAndPassword(string username, string password)

        {
            User? user = null;

            using (SqlConnection connection =
                   new SqlConnection(_connectionString))
            {
                string query =
                    @"SELECT *
                      FROM Employee
                      WHERE EmployeeName = @username
                      AND EmployeeNumber = @password";

                SqlCommand command =
                    new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@password", password);

                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    user = new User
                    {
                        Id = Convert.ToInt32(reader["EmployeeId"]),
                        Username = reader["EmployeeName"].ToString(),
                        Password = reader["EmployeeNumber"].ToString(),
                        Role = reader["EmployeeOccupation"].ToString()
                    };
                }
            }

            return user;
        }
    }
}

