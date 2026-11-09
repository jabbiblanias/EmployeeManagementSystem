using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagementSystem.Data
{
    public class DatabaseConnection
    {
        public static SqlConnection GetConnection()
        {
            string connectionString =
                ConfigurationManager.ConnectionStrings[
                    "EmployeeManagementSystem.Properties.Settings.ConnectionString"
                ]?.ConnectionString;

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException(
                    "The database connection string was not found in App.config."
                );
            }

            return new SqlConnection(connectionString);
        }
    }
}
