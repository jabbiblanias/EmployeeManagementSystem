using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace EmployeeManagementSystem
{
    internal class AdminAttendanceData
    {
        const int NUM1 = 1;
        const int NUM2 = 10;
        public string Employee_ID {  get; set; }
        public string Name {  get; set; }
        public string Profession {  get; set; }
        public DateTime Date { get; set; }
        public string ClockIn { get; set; }
        public string ClockOut { get; set; }
        public string Status { get; set; }
        public string Overtime { get; set; }

        SqlConnection connect =
            new SqlConnection(@"Data Source=LAPTOP-0OGAQKFF\SQLEXPRESS;Initial Catalog=DB_Employee;User ID=ja;Password=john");
        public List<AdminAttendanceData> employeesAttendanceListData(int entry)
        {
            int endRow = NUM2 * entry;
            int startRow = endRow-9;
            List<AdminAttendanceData> listdata = new List<AdminAttendanceData>();

            if (connect.State != ConnectionState.Open)
            {
                try
                {
                    connect.Open();

                    string selectData = $@"
                        SELECT DATE, CLOCK_IN, CLOCK_OUT, STATUS, OVERTIME, EMPLOYEE_ID, FULL_NAME, PROFESSION 
                        FROM (
                            SELECT 
                                TBL_ATTENDANCE.DATE, 
                                TBL_ATTENDANCE.CLOCK_IN, 
                                TBL_ATTENDANCE.CLOCK_OUT, 
                                CASE 
                                    WHEN CLOCK_IN > '07:30:00' THEN 'Late' 
                                    ELSE 'On Time' 
                                END AS STATUS, 
                                CASE 
                                    WHEN DATEDIFF(MINUTE, '16:30', CLOCK_OUT) > 0 THEN 
                                        CONVERT(VARCHAR, DATEADD(MINUTE, DATEDIFF(MINUTE, '16:30', CLOCK_OUT), 0), 108) 
                                    ELSE '00:00:00' 
                                END AS OVERTIME, 
                                TBL_EMPLOYEE_INFO.EMPLOYEE_ID, 
                                TBL_EMPLOYEE_INFO.FULL_NAME, 
                                TBL_EMPLOYEE_INFO.PROFESSION, 
                                ROW_NUMBER() OVER (ORDER BY TBL_ATTENDANCE.DATE DESC) AS RowNum 
                            FROM TBL_ATTENDANCE 
                            INNER JOIN TBL_USERS ON TBL_ATTENDANCE.ACCOUNT_ID = TBL_USERS.ACCOUNT_ID 
                            INNER JOIN TBL_EMPLOYEE_INFO ON TBL_USERS.ID = TBL_EMPLOYEE_INFO.ID
                        ) AS RowConstrainedResult 
                        WHERE RowNum >= {startRow} AND RowNum <= {endRow} 
                        ORDER BY DATE DESC";

                    using (SqlCommand cmd = new SqlCommand(selectData, connect))
                    {
                        SqlDataReader reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            AdminAttendanceData aad = new AdminAttendanceData();
                            aad.Employee_ID = reader["EMPLOYEE_ID"].ToString();
                            aad.Name = reader["FULL_NAME"].ToString();
                            aad.Profession = reader.GetString(reader.GetOrdinal("PROFESSION"));
                            aad.Date = (DateTime)reader["DATE"];
                            if (!reader.IsDBNull(reader.GetOrdinal("CLOCK_IN")))
                            {
                                aad.ClockIn = DateTime.Today.Add((TimeSpan)reader["CLOCK_IN"]).ToString("hh:mm tt");
                            }

                            if (!reader.IsDBNull(reader.GetOrdinal("CLOCK_OUT")))
                            {
                                aad.ClockOut = DateTime.Today.Add((TimeSpan)reader["CLOCK_OUT"]).ToString("hh:mm tt");
                            }
                            aad.Status = reader["STATUS"].ToString();
                            aad.Overtime = reader["OVERTIME"].ToString();

                            listdata.Add(aad);
                        }
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex);
                }
                finally
                {
                    connect.Close();
                }
            }
            return listdata;
        }
        public List<AdminAttendanceData> SearchEmployeeAttendanceListData(int entry, string name)
        {
            int endRow = NUM2 * entry;
            int startRow = endRow - 9;
            List<AdminAttendanceData> listdata = new List<AdminAttendanceData>();

            if (connect.State != ConnectionState.Open)
            {
                try
                {
                    connect.Open();
                    string selectData = $@"
                        SELECT FULL_NAME,DATE, CLOCK_IN, CLOCK_OUT, STATUS, OVERTIME, EMPLOYEE_ID,  PROFESSION 
                        FROM (
                            SELECT 
                                TBL_ATTENDANCE.DATE, 
                                TBL_ATTENDANCE.CLOCK_IN, 
                                TBL_ATTENDANCE.CLOCK_OUT, 
                                CASE 
                                    WHEN CLOCK_IN > '07:30:00' THEN 'Late' 
                                    ELSE 'On Time' 
                                END AS STATUS, 
                                CASE 
                                    WHEN DATEDIFF(MINUTE, '16:30', CLOCK_OUT) > 0 THEN 
                                        CONVERT(VARCHAR, DATEADD(MINUTE, DATEDIFF(MINUTE, '16:30', CLOCK_OUT), 0), 108) 
                                    ELSE '00:00:00' 
                                END AS OVERTIME, 
                                TBL_EMPLOYEE_INFO.EMPLOYEE_ID, 
                                TBL_EMPLOYEE_INFO.FULL_NAME, 
                                TBL_EMPLOYEE_INFO.PROFESSION, 
                                ROW_NUMBER() OVER (ORDER BY TBL_ATTENDANCE.DATE DESC) AS RowNum 
                            FROM TBL_ATTENDANCE 
                            INNER JOIN TBL_USERS ON TBL_ATTENDANCE.ACCOUNT_ID = TBL_USERS.ACCOUNT_ID 
                            INNER JOIN TBL_EMPLOYEE_INFO ON TBL_USERS.ID = TBL_EMPLOYEE_INFO.ID
                            WHERE FULL_NAME = @FULL_NAME
                        ) AS RowConstrainedResult 
                        WHERE RowNum >= {startRow} AND RowNum <= {endRow}
                        ORDER BY DATE DESC";

                    using (SqlCommand cmd = new SqlCommand(selectData, connect))
                    {
                        cmd.Parameters.AddWithValue("@FULL_NAME", name);
                        SqlDataReader reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            AdminAttendanceData aad = new AdminAttendanceData();
                            aad.Employee_ID = reader["EMPLOYEE_ID"].ToString();
                            aad.Name = reader["FULL_NAME"].ToString();
                            aad.Profession = reader["PROFESSION"].ToString();
                            aad.Date = (DateTime)reader["DATE"];
                            if (!reader.IsDBNull(reader.GetOrdinal("CLOCK_IN")))
                            {
                                aad.ClockIn = DateTime.Today.Add((TimeSpan)reader["CLOCK_IN"]).ToString("hh:mm tt");
                            }

                            if (!reader.IsDBNull(reader.GetOrdinal("CLOCK_OUT")))
                            {
                                aad.ClockOut = DateTime.Today.Add((TimeSpan)reader["CLOCK_OUT"]).ToString("hh:mm tt");
                            }
                            aad.Status = reader["STATUS"].ToString();
                            aad.Overtime = reader["OVERTIME"].ToString();

                            listdata.Add(aad);
                        }
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex);
                }
                finally
                {
                    connect.Close();
                }
            }
            return listdata;
        }
        public int recentNumberOfEntries()
        {
            connect.Open();
            string query = @"SELECT CEILING(COUNT(*) / 10.0) AS NumberOfGroups FROM TBL_ATTENDANCE";
            SqlCommand com = new SqlCommand(query, connect);
            int entries = Convert.ToInt32(com.ExecuteScalar());
            connect.Close();
            return entries;
        }
        public int searchNumberOfEntries(string name)
        {
            connect.Open();
            string query = @"SELECT CEILING(COUNT(*) / 10.0) AS NumberOfGroups FROM TBL_ATTENDANCE INNER JOIN TBL_USERS ON TBL_ATTENDANCE.ACCOUNT_ID = TBL_USERS.ACCOUNT_ID 
                            INNER JOIN TBL_EMPLOYEE_INFO ON TBL_USERS.ID = TBL_EMPLOYEE_INFO.ID WHERE FULL_NAME = @FULL_NAME";
            SqlCommand com = new SqlCommand(query, connect);
            com.Parameters.AddWithValue("@FULL_NAME", name);
            int entries = Convert.ToInt32(com.ExecuteScalar());
            connect.Close();
            return entries;
        }
        public AutoCompleteStringCollection employeeNamesData()
        {
            AutoCompleteStringCollection employeeNames = new AutoCompleteStringCollection();
                try
                {
                    connect.Open();

                    string selectData = "SELECT FULL_NAME FROM TBL_EMPLOYEE_INFO";

                    using (SqlCommand cmd = new SqlCommand(selectData, connect))
                    {
                        SqlDataReader reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            AdminAttendanceData aad = new AdminAttendanceData();
                            aad.Name = reader.GetString(0);

                            employeeNames.Add(aad.Name);
                        }
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex);
                }
                finally
                {
                    connect.Close();
                }
            return employeeNames;
        }
    }
}
