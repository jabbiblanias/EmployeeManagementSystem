using EmployeeManagementSystem.Data;
using EmployeeManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EmployeeManagementSystem.Repositories
{
    internal class UserRepository
    {
        public User GetByUsername(string username)
        {
            const string query = @"SELECT * FROM TBL_USERS WHERE USERNAME = @USERNAME";

            using(SqlConnection connection = DatabaseConnection.GetConnection())
            {
                using(SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add("@USERNAME", System.Data.SqlDbType.NVarChar, 50).Value = username;

                    connection.Open();

                    using(SqlDataReader reader = command.ExecuteReader())
                    {
                        if(reader.Read())
                        {
                            return new User
                            {
                                ID = reader.GetInt32(reader.GetOrdinal("ID")),
                                Username = reader.GetString(reader.GetOrdinal("USERNAME")),
                                Password = reader.GetString(reader.GetOrdinal("PASSWORD")),
                                Role = reader.GetString(reader.GetOrdinal("ROLE"))
                            };
                        }
                    }
                }
                
            }
            return null;
        }


        public void CreateUser(int id, string username, string passwordHash)
        {
            using (SqlConnection connection = DatabaseConnection.GetConnection())
            {
                connection.Open();

                string query = @"
            INSERT INTO TBL_USERS
            (ID, USERNAME, PASSWORD, DATE_REGISTER)
            VALUES
            (@ID, @USERNAME, @PASSWORD, @DATE_REGISTER)";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add("@ID", System.Data.SqlDbType.Int).Value = id;

                    command.Parameters.Add(
                        "@USERNAME",
                        System.Data.SqlDbType.NVarChar,
                        50
                    ).Value = username;

                    command.Parameters.Add(
                        "@PASSWORD",
                        System.Data.SqlDbType.NVarChar,
                        500
                    ).Value = passwordHash;

                    command.Parameters.Add(
                        "@DATE_REGISTER",
                        System.Data.SqlDbType.Date
                    ).Value = DateTime.Today;

                    command.ExecuteNonQuery();
                }
            }
        }

        public bool EmployeeIdExists(int id)
        {
            using (SqlConnection connection = DatabaseConnection.GetConnection())
            {
                connection.Open();

                string query = @"
            SELECT CASE
                WHEN EXISTS (
                    SELECT 1
                    FROM TBL_EMPLOYEE_INFO
                    WHERE ID = @ID
                )
                THEN 1 ELSE 0
            END";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add("@ID", System.Data.SqlDbType.Int).Value = id;

                    return Convert.ToInt32(command.ExecuteScalar()) == 1;
                }
            }
        }

        public bool UserIdExists(int id)
        {
            using (SqlConnection connection = DatabaseConnection.GetConnection())
            {
                connection.Open();

                string query = @"
            SELECT CASE
                WHEN EXISTS (
                    SELECT 1
                    FROM TBL_USERS
                    WHERE ID = @ID
                )
                THEN 1 ELSE 0
            END";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add("@ID", System.Data.SqlDbType.Int).Value = id;

                    return Convert.ToInt32(command.ExecuteScalar()) == 1;
                }
            }
        }

        public bool UsernameExists(string username)
        {
            using (SqlConnection connection = DatabaseConnection.GetConnection())
            {
                connection.Open();

                string query = @"
            SELECT CASE
                WHEN EXISTS (
                    SELECT 1
                    FROM TBL_USERS
                    WHERE USERNAME = @USERNAME
                )
                THEN 1 ELSE 0
            END";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add(
                        "@USERNAME",
                        System.Data.SqlDbType.NVarChar,
                        50
                    ).Value = username;

                    return Convert.ToInt32(command.ExecuteScalar()) == 1;
                }
            }
        }

        public bool UpdateUser(int accountId, string username, string passwordHash)
        {
            using (SqlConnection connection = DatabaseConnection.GetConnection())
            {
                connection.Open();

                string query = @"
            UPDATE TBL_USERS
            SET USERNAME = @USERNAME,
                PASSWORD = @PASSWORD
            WHERE ID = @ACCOUNT_ID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add(
                        "@USERNAME",
                        SqlDbType.NVarChar,
                        50
                    ).Value = username;

                    command.Parameters.Add(
                        "@PASSWORD",
                        SqlDbType.NVarChar,
                        255
                    ).Value = passwordHash;

                    command.Parameters.Add(
                        "@ACCOUNT_ID",
                        SqlDbType.Int
                    ).Value = accountId;

                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        public void UpdateContactInfo(User user, string email, string primaryContactNum, string secondaryContactNum, string emergencyContactNum, string currentAddress)
        {
            using (SqlConnection connect = DatabaseConnection.GetConnection())
            {
                connect.Open();

                string updateData = @"UPDATE TBL_EMPLOYEE_INFO
                    SET 
                        EMAIL_ADD = @EMAIL_ADD, 
                        PRIMARY_CONTACT_NUM = @PRIMARY_CONTACT_NUM, 
                        SECONDARY_CONTACT_NUM = @SECONDARY_CONTACT_NUM, 
                        EMERGENCY_CONTACT_NUM = @EMERGENCY_CONTACT_NUM, 
                        CURRENT_ADD = @CURRENT_ADD
                    FROM 
                        TBL_EMPLOYEE_INFO
                        INNER JOIN 
                        TBL_USERS ON TBL_EMPLOYEE_INFO.ID = TBL_USERS.ID
                    WHERE 
                        TBL_USERS.ACCOUNT_ID = @ACCOUNT_ID";

                using (SqlCommand cmd = new SqlCommand(updateData, connect))
                {
                    cmd.Parameters.AddWithValue("@EMAIL_ADD", email);
                    cmd.Parameters.AddWithValue("@PRIMARY_CONTACT_NUM", primaryContactNum);
                    cmd.Parameters.AddWithValue("@SECONDARY_CONTACT_NUM", secondaryContactNum);
                    cmd.Parameters.AddWithValue("@EMERGENCY_CONTACT_NUM", emergencyContactNum);
                    cmd.Parameters.AddWithValue("@CURRENT_ADD", currentAddress);
                    cmd.Parameters.AddWithValue("@ACCOUNT_ID", user.ID);

                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
