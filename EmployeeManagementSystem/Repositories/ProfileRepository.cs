using System;
using System.Data;
using System.Data.SqlClient;
using EmployeeManagementSystem.Models;
using EmployeeManagementSystem.Data;

namespace EmployeeManagementSystem.Repositories
{
    public class ProfileRepository
    {
        public Employee GetUserProfile(int accountId)
        {
            const string query = @"
                SELECT E.*
                FROM TBL_EMPLOYEE_INFO E
                INNER JOIN TBL_USERS U
                    ON E.ID = U.ID
                WHERE U.ID = @ID";

            using (SqlConnection connection =
                DatabaseConnection.GetConnection())
            using (SqlCommand command =
                new SqlCommand(query, connection))
            {
                command.Parameters.Add(
                    "@ID", SqlDbType.Int
                ).Value = accountId;

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (!reader.Read())
                        return null;

                    return new Employee
                    {
                        FullName = reader["FULL_NAME"].ToString(),
                        SecContactNum = reader["SECONDARY_CONTACT_NUM"].ToString(),
                        EmailAdd = reader["EMAIL_ADD"].ToString(),
                        EmergencyContactNum = reader["EMERGENCY_CONTACT_NUM"].ToString(),
                        CurrentAdd = reader["CURRENT_ADD"].ToString(),
                        EmployeeID = reader["EMPLOYEE_ID"].ToString(),
                        Gender = reader["GENDER"].ToString(),

                        DateofBirth = reader["DATEOFBIRTH"] == DBNull.Value
                            ? DateTime.MinValue
                            : Convert.ToDateTime(reader["DATEOFBIRTH"]),

                        Profession = reader["PROFESSION"].ToString(),
                        PrimaryContactNum = reader["PRIMARY_CONTACT_NUM"].ToString(),
                        Image = reader["IMAGE"] == DBNull.Value
                            ? null
                            : reader["IMAGE"].ToString()
                    };
                }
            }
        }
        public Employee GetEmployeeProfile(string employeeId)
        {
            const string query = @"SELECT * FROM TBL_EMPLOYEE_INFO WHERE EMPLOYEE_ID = @EMPLOYEE_ID";

            using (SqlConnection connection =
                DatabaseConnection.GetConnection())
            using (SqlCommand command =
                new SqlCommand(query, connection))
            {
                command.Parameters.Add(
                    "@EMPLOYEE_ID", SqlDbType.NVarChar
                ).Value = employeeId;

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (!reader.Read())
                        return null;

                    return new Employee
                    {
                        FullName = reader["FULL_NAME"].ToString(),
                        SecContactNum = reader["SECONDARY_CONTACT_NUM"].ToString(),
                        EmailAdd = reader["EMAIL_ADD"].ToString(),
                        EmergencyContactNum = reader["EMERGENCY_CONTACT_NUM"].ToString(),
                        CurrentAdd = reader["CURRENT_ADD"].ToString(),
                        EmployeeID = reader["EMPLOYEE_ID"].ToString(),
                        Gender = reader["GENDER"].ToString(),

                        DateofBirth = reader["DATEOFBIRTH"] == DBNull.Value
                            ? DateTime.MinValue
                            : Convert.ToDateTime(reader["DATEOFBIRTH"]),

                        Profession = reader["PROFESSION"].ToString(),
                        PrimaryContactNum = reader["PRIMARY_CONTACT_NUM"].ToString(),
                        Status = reader["STATUS"].ToString(),

                        DateCreated = reader["DATE_CREATED"] == DBNull.Value
                            ? DateTime.MinValue
                            : Convert.ToDateTime(reader["DATE_CREATED"]),

                        DateModified = reader["DATE_MODIFIED"] == DBNull.Value
                            ? DateTime.MinValue
                            : Convert.ToDateTime(reader["DATE_MODIFIED"]),

                        Image = reader["IMAGE"] == DBNull.Value
                            ? null
                            : reader["IMAGE"].ToString()
                    };
                }
            }
        }
    }
}
