using Chapeau.Models;
using Chapeau.Repositories.Interfaces;
using Microsoft.Data.SqlClient;


namespace Chapeau.Repositories
{
    public class TableRepository : ITableRepository
    {
        private readonly string _connectionString;

        public TableRepository(IConfiguration configuration)
        {
            _connectionString =
                configuration.GetConnectionString("ChapeauDataBase");
            
        }

        public List<Table> GetAllTables()
        {
            List<Table> tables = new List<Table>();

            using (SqlConnection connection =
                   new SqlConnection(_connectionString))
            {
                string query =
                    @"SELECT *
                      FROM [Table]";

                SqlCommand command =
                    new SqlCommand(query, connection);

                connection.Open();

                SqlDataReader reader =
                    command.ExecuteReader();

                while (reader.Read())
                {
                    Table table = new Table
                    {
                        TableId =
                            Convert.ToInt32(
                                reader["TableId"]),

                        TableCapacity =
                            Convert.ToInt32(
                                reader["TableCapacity"]),

                        TableStatus =
                            reader["TableStatus"]
                                .ToString()
                    };

                    tables.Add(table);
                }
            }

            return tables;
        }
       
    }
}
