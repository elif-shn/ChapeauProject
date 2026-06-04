using Chapeau.Enums;
using Chapeau.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace Chapeau.Repositories
{
    public class FinancialRepository : IFinancialRepository
    {
        private readonly string _connectionString;

        public FinancialRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ChapeauDataBase");
        }

        public FinancialSummary GetFinancialData(DateTime startDate, DateTime endDate)
        {
            FinancialSummary summary = new FinancialSummary();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                
                string incomeQuery = @"
                    SELECT 
                        m.Card,
                        COUNT(DISTINCT o.OrderId) AS TotalSales,
                        SUM(oi.OrderItemQuantity * oi.Price) AS TotalIncome
                    FROM [Order] o
                    INNER JOIN OrderItem oi ON o.OrderId = oi.OrderId
                    INNER JOIN MenuItem mi ON oi.MenuItemId = mi.MenuItemId
                    INNER JOIN Menu m ON mi.MenuId = m.MenuId
                    WHERE o.OrderTime >= @StartDate AND o.OrderTime <= @EndDate
                    AND o.OrderStatus NOT IN ('Cancelled')
                    GROUP BY m.Card";

                SqlCommand incomeCommand = new SqlCommand(incomeQuery, connection);
                incomeCommand.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = startDate;
                incomeCommand.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = endDate;

                connection.Open();
                SqlDataReader reader = incomeCommand.ExecuteReader();

                while (reader.Read())
                {
                    Card card = (Card)(int)reader["Card"];
                    int sales = (int)reader["TotalSales"];
                    decimal income = reader["TotalIncome"] == DBNull.Value ? 0 : (decimal)reader["TotalIncome"];

                    if (card == Card.Lunch)
                    {
                        summary.TotalSalesLunch = sales;
                        summary.TotalIncomeLunch = income;
                    }
                    else if (card == Card.Dinner)
                    {
                        summary.TotalSalesDinner = sales;
                        summary.TotalIncomeDinner = income;
                    }
                    else if (card == Card.Drink)
                    {
                        summary.TotalSalesDrinks = sales;
                        summary.TotalIncomeDrinks = income;
                    }
                }

                reader.Close();

             
                string tipQuery = @"
                    SELECT ISNULL(SUM(p.TipAmount), 0) AS TotalTip
                    FROM Payment p
                    INNER JOIN [Order] o ON p.OrderId = o.OrderId
                    WHERE o.OrderTime >= @StartDate AND o.OrderTime <= @EndDate";

                SqlCommand tipCommand = new SqlCommand(tipQuery, connection);
                tipCommand.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = startDate;
                tipCommand.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = endDate;

                object tipResult = tipCommand.ExecuteScalar();
                summary.TotalTip = tipResult == DBNull.Value ? 0 : (decimal)tipResult;
            }

            return summary;
        }
    }
}