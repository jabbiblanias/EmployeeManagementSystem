using EmployeeManagementSystem.Data;
using EmployeeManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace EmployeeManagementSystem.Repositories
{
    public class EmployeeRepository
    {
        public bool EmployeeIdExists(string employeeId)
        {
            using (SqlConnection connect = DatabaseConnection.GetConnection())
            {
                string query = @"
                    SELECT COUNT(*)
                    FROM TBL_EMPLOYEE_INFO
                    WHERE EMPLOYEE_ID = @EMPLOYEE_ID
                    AND ARCHIVE_DATE IS NULL";

                using (SqlCommand cmd = new SqlCommand(query, connect))
                {
                    cmd.Parameters.Add("@EMPLOYEE_ID", SqlDbType.VarChar, 50)
                        .Value = employeeId;

                    connect.Open();

                    int count = Convert.ToInt32(cmd.ExecuteScalar());

                    return count > 0;
                }
            }
        }

        public void AddEmployee(Employee employee)
        {
            using (SqlConnection connect = DatabaseConnection.GetConnection())
            {
                string query = @"
                    INSERT INTO TBL_EMPLOYEE_INFO
                    (
                        EMPLOYEE_ID,
                        FULL_NAME,
                        DATEOFBIRTH,
                        GENDER,
                        PRIMARY_CONTACT_NUM,
                        PROFESSION,
                        IMAGE,
                        SALARY,
                        DATE_CREATED
                    )
                    VALUES
                    (
                        @EMPLOYEE_ID,
                        @FULL_NAME,
                        @DATEOFBIRTH,
                        @GENDER,
                        @PRIMARY_CONTACT_NUM,
                        @PROFESSION,
                        @IMAGE,
                        @SALARY,
                        @DATE_CREATED
                    )";

                using (SqlCommand cmd = new SqlCommand(query, connect))
                {
                    cmd.Parameters.Add("@EMPLOYEE_ID", SqlDbType.VarChar, 50)
                        .Value = employee.EmployeeID;

                    cmd.Parameters.Add("@FULL_NAME", SqlDbType.VarChar, 150)
                        .Value = employee.FullName;

                    cmd.Parameters.Add("@DATEOFBIRTH", SqlDbType.Date)
                        .Value = employee.DateofBirth;

                    cmd.Parameters.Add("@GENDER", SqlDbType.VarChar, 20)
                        .Value = employee.Gender;

                    cmd.Parameters.Add("@PRIMARY_CONTACT_NUM", SqlDbType.VarChar, 30)
                        .Value = employee.PrimaryContactNum;

                    cmd.Parameters.Add("@PROFESSION", SqlDbType.VarChar, 100)
                        .Value = employee.Profession;

                    cmd.Parameters.Add("@IMAGE", SqlDbType.VarChar, 500)
                        .Value = employee.Image;

                    cmd.Parameters.Add("@SALARY", SqlDbType.Decimal)
                        .Value = employee.Salary;

                    cmd.Parameters.Add("@DATE_CREATED", SqlDbType.Date)
                        .Value = employee.DateCreated;

                    connect.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void UpdateEmployee(Employee employee)
        {
            using (SqlConnection connect = DatabaseConnection.GetConnection())
            {
                string query = @"
            UPDATE TBL_EMPLOYEE_INFO
            SET
                FULL_NAME = @FULL_NAME,
                DATEOFBIRTH = @DATEOFBIRTH,
                GENDER = @GENDER,
                PRIMARY_CONTACT_NUM = @PRIMARY_CONTACT_NUM,
                PROFESSION = @PROFESSION,
                IMAGE = @IMAGE,
                DATE_MODIFIED = @DATE_MODIFIED
            WHERE EMPLOYEE_ID = @EMPLOYEE_ID";

                using (SqlCommand cmd = new SqlCommand(query, connect))
                {
                    cmd.Parameters.Add("@FULL_NAME", SqlDbType.VarChar, 150)
                        .Value = employee.FullName;

                    cmd.Parameters.Add("@DATEOFBIRTH", SqlDbType.Date)
                        .Value = employee.DateofBirth;

                    cmd.Parameters.Add("@GENDER", SqlDbType.VarChar, 20)
                        .Value = employee.Gender;

                    cmd.Parameters.Add("@PRIMARY_CONTACT_NUM", SqlDbType.VarChar, 30)
                        .Value = employee.PrimaryContactNum;

                    cmd.Parameters.Add("@PROFESSION", SqlDbType.VarChar, 100)
                        .Value = employee.Profession;

                    cmd.Parameters.Add("@IMAGE", SqlDbType.VarChar, 500)
                        .Value = employee.Image;

                    cmd.Parameters.Add("@DATE_MODIFIED", SqlDbType.Date)
                        .Value = employee.DateModified;

                    cmd.Parameters.Add("@EMPLOYEE_ID", SqlDbType.VarChar, 50)
                        .Value = employee.EmployeeID;

                    connect.Open();

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected == 0)
                    {
                        throw new InvalidOperationException(
                            "Employee was not found or is archived.");
                    }
                }
            }
        }
        public void UpdateSalary(string employeeID, int salary)
        {
            using (SqlConnection connect = DatabaseConnection.GetConnection())
            {
                connect.Open();
                DateTime today = DateTime.Today;

                string updateData = "UPDATE TBL_EMPLOYEE_INFO SET SALARY = @SALARY WHERE EMPLOYEE_ID = @EMPLOYEE_ID";

                using (SqlCommand cmd = new SqlCommand(updateData, connect))
                {
                    cmd.Parameters.AddWithValue("@SALARY", salary);
                    cmd.Parameters.AddWithValue("@EMPLOYEE_ID", employeeID);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void ArchiveEmployee(string employeeId)
        {
            using (SqlConnection connect = DatabaseConnection.GetConnection())
            {
                string query = @"
            UPDATE TBL_EMPLOYEE_INFO
            SET ARCHIVE_DATE = @ARCHIVE_DATE,
                STATUS = @STATUS
            WHERE EMPLOYEE_ID = @EMPLOYEE_ID";

                using (SqlCommand cmd = new SqlCommand(query, connect))
                {
                    cmd.Parameters.Add("@ARCHIVE_DATE", SqlDbType.Date)
                        .Value = DateTime.Today;

                    cmd.Parameters.Add("@STATUS", SqlDbType.VarChar, 20)
                        .Value = "inactive";

                    cmd.Parameters.Add("@EMPLOYEE_ID", SqlDbType.VarChar, 50)
                        .Value = employeeId;

                    connect.Open();

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected == 0)
                        throw new InvalidOperationException(
                            "Employee was not found or is already archived.");
                }
            }
        }

        public void UnarchiveEmployee(string employeeId)
        {
            using (SqlConnection connect = DatabaseConnection.GetConnection())
            {
                string query = @"
            UPDATE TBL_EMPLOYEE_INFO
            SET ARCHIVE_DATE = NULL,
                STATUS = @STATUS
            WHERE EMPLOYEE_ID = @EMPLOYEE_ID
            AND ARCHIVE_DATE IS NOT NULL";

                using (SqlCommand cmd = new SqlCommand(query, connect))
                {
                    cmd.Parameters.Add("@STATUS", SqlDbType.VarChar, 20)
                        .Value = "active";

                    cmd.Parameters.Add("@EMPLOYEE_ID", SqlDbType.VarChar, 50)
                        .Value = employeeId;

                    connect.Open();

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected == 0)
                        throw new InvalidOperationException(
                            "Employee was not found or is not archived.");
                }
            }
        }
        public List<Employee> employeeListData()
        {
            List<Employee> listdata = new List<Employee>();

            using (SqlConnection connect = DatabaseConnection.GetConnection())
            {
                connect.Open();

                string selectData = "SELECT * FROM TBL_EMPLOYEE_INFO WHERE STATUS = 'active'";

                using (SqlCommand cmd = new SqlCommand(selectData, connect))
                {
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Employee ed = new Employee();
                        ed.ID = (int)reader["ID"];
                        ed.EmployeeID = reader["EMPLOYEE_ID"].ToString();
                        ed.Name = reader["FULL_NAME"].ToString();
                        ed.DateofBirth = (DateTime)reader["DATEOFBIRTH"];
                        ed.Gender = reader["GENDER"].ToString();
                        ed.Contact = reader["PRIMARY_CONTACT_NUM"].ToString();
                        ed.Profession = reader["PROFESSION"].ToString();
                        ed.Image = reader["IMAGE"].ToString();

                        listdata.Add(ed);
                    }
                }
            }
            return listdata;
        }
        
        public List<Employee> ArchiveEmployeeListData()
        {
            List<Employee> listdata = new List<Employee>();

            using (SqlConnection connect = DatabaseConnection.GetConnection())
            {
                connect.Open();

                string selectData = "SELECT * FROM TBL_EMPLOYEE_INFO WHERE STATUS = 'inactive'";

                using (SqlCommand cmd = new SqlCommand(selectData, connect))
                {
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Employee ed = new Employee();
                        ed.ID = (int)reader["ID"];
                        ed.EmployeeID = reader["EMPLOYEE_ID"].ToString();
                        ed.Name = reader["FULL_NAME"].ToString();
                        ed.DateofBirth = (DateTime)reader["DATEOFBIRTH"];
                        ed.Gender = reader["GENDER"].ToString();
                        ed.Contact = reader["PRIMARY_CONTACT_NUM"].ToString();
                        ed.Profession = reader["PROFESSION"].ToString();
                        ed.Image = reader["IMAGE"].ToString();

                        listdata.Add(ed);
                    }
                }
            }
            return listdata;
        }

        public List<Employee> SalaryEmployeeListData()
        {
            List<Employee> listdata = new List<Employee>();

            using (SqlConnection connect = DatabaseConnection.GetConnection())
            {
                connect.Open();

                string selectData = "SELECT * FROM TBL_EMPLOYEE_INFO WHERE STATUS = 'active'";

                using (SqlCommand cmd = new SqlCommand(selectData, connect))
                {
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Employee ed = new Employee();
                        ed.EmployeeID = reader["EMPLOYEE_ID"].ToString();
                        ed.Name = reader["FULL_NAME"].ToString();
                        ed.Profession = reader["PROFESSION"].ToString();
                        ed.Salary = (int)reader["SALARY"];

                        listdata.Add(ed);
                    }
                }
            }
            return listdata;
        }

        public Employee EmployeeDetail(User user)
        {
            using (SqlConnection connect = DatabaseConnection.GetConnection())
            {
                connect.Open();
                string selectData = "SELECT TBL_EMPLOYEE_INFO.IMAGE, TBL_EMPLOYEE_INFO.FULL_NAME, TBL_EMPLOYEE_INFO.PROFESSION, TBL_EMPLOYEE_INFO.ID FROM TBL_EMPLOYEE_INFO INNER JOIN TBL_USERS ON TBL_EMPLOYEE_INFO.ID = TBL_USERS.ID WHERE TBL_USERS.ID = @ID";
                using (SqlCommand cmd = new SqlCommand(selectData, connect))
                {
                    cmd.Parameters.AddWithValue("@ID", user.ID);
                    SqlDataReader reader = cmd.ExecuteReader();
                    Employee employee = new Employee();
                    if (reader.Read())
                    {
                        employee.Image = reader.GetString(0);
                        employee.FullName = reader.GetString(1);
                        employee.Profession = reader.GetString(2);
                    }
                    return employee;
                }
            }
        }
    }
}
