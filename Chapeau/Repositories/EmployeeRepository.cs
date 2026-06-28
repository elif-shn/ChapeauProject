using Chapeau.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using Chapeau.Enums;

namespace Chapeau.Repositories
{
    public class EmployeeRepository : IEmployeeRepository 
    {
        private readonly string _connectionString;

        public EmployeeRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ChapeauDataBase");
        }

        public List<Employee> GetAll()
        {
            List<Employee> employees = new List<Employee>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT EmployeeId, EmployeeName, EmployeeNumber, EmployeeOccupation, EmployeePassword, IsActive FROM Employee";
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();  
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    employees.Add(ReadEmployee(reader));
                }
            }
            return employees;
        }

        public Employee GetById(int id)
        {
            Employee employee = null;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT EmployeeId, EmployeeName, EmployeeNumber, EmployeeOccupation, EmployeePassword, IsActive FROM Employee WHERE EmployeeId = @EmployeeId";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@EmployeeId", id);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    employee = ReadEmployee(reader);
                }
            }
            return employee;
        }

        public Employee? GetByUsernameAndPassword(string username, string password)
        {
            Employee? employee = null;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query =
                    @"SELECT EmployeeId, EmployeeName, EmployeeNumber, EmployeeOccupation, EmployeePassword, IsActive
              FROM Employee
              WHERE EmployeeName = @username
              AND EmployeeNumber = @password
              AND IsActive = 1";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@password", password);

                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    employee = ReadEmployee(reader);
                }
            }

            return employee;
        }


        public void Add(Employee employee)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"INSERT INTO Employee (EmployeeName, EmployeeNumber, EmployeeOccupation, EmployeePassword, IsActive)
                                 VALUES (@EmployeeName, @EmployeeNumber, @EmployeeOccupation, @EmployeePassword, 1)";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@EmployeeName", employee.EmployeeName);
                command.Parameters.AddWithValue("@EmployeeNumber", employee.EmployeeNumber);
                command.Parameters.AddWithValue("@EmployeeOccupation", employee.EmployeeOccupation.ToString());
                command.Parameters.AddWithValue("@EmployeePassword", employee.EmployeePassword);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public void Update(Employee employee)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"UPDATE Employee 
                                 SET EmployeeName = @EmployeeName,
                                     EmployeeNumber = @EmployeeNumber,
                                     EmployeeOccupation = @EmployeeOccupation,
                                     EmployeePassword = @EmployeePassword
                                 WHERE EmployeeId = @EmployeeId";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@EmployeeId", employee.EmployeeId);
                command.Parameters.AddWithValue("@EmployeeName", employee.EmployeeName);
                command.Parameters.AddWithValue("@EmployeeNumber", employee.EmployeeNumber);
                command.Parameters.AddWithValue("@EmployeeOccupation", employee.EmployeeOccupation.ToString());
                command.Parameters.AddWithValue("@EmployeePassword", employee.EmployeePassword);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public void SetActive(int id, bool isActive)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "UPDATE Employee SET IsActive = @IsActive WHERE EmployeeId = @EmployeeId";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@EmployeeId", id);
                command.Parameters.AddWithValue("@IsActive", isActive);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

       
        public bool EmployeeNumberExists(string employeeNumber)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT COUNT(*) FROM Employee WHERE EmployeeNumber = @EmployeeNumber";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@EmployeeNumber", employeeNumber);
                connection.Open();
                int count = (int)command.ExecuteScalar();
                return count > 0;
            }
        }

        private Employee ReadEmployee(SqlDataReader reader)
        {
            return new Employee
            {
                EmployeeId = (int)reader["EmployeeId"],
                EmployeeName = reader["EmployeeName"].ToString() ?? string.Empty,
                EmployeeNumber = reader["EmployeeNumber"].ToString() ?? string.Empty,
                EmployeeOccupation = Enum.Parse<EmployeeRole>(
                    reader["EmployeeOccupation"].ToString() ?? string.Empty,
                    ignoreCase: true
                ),
                EmployeePassword = reader["EmployeePassword"].ToString() ?? string.Empty,
                IsActive = (bool)reader["IsActive"]
            };
        }

       
    }
}            