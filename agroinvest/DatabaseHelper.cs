using System.Data.SqlClient;

namespace AgroInvestApp
{
    public static class DatabaseHelper
    {
        
        private static readonly string connectionString = @"Server=(localdb)\MSSQLLocalDB;Database=agroinvest;Integrated Security=True;";

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }
    }
}