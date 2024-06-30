using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace EmployeeManagementSystem
{
    internal class EmployeeAttendanceData
    {
        LoginView loginView;
        public DateTime Date {  get; set; }
        public string ClockIn {  get; set; }
        public string ClockOut { get; set; }
        public string Status {  get; set; }
        public string Overtime {  get; set; }

        public EmployeeAttendanceData(LoginView loginView)
        {
            this.loginView = loginView;
        }
        SqlConnection connect =
            new SqlConnection(@"Data Source=LAPTOP-0OGAQKFF\SQLEXPRESS;Initial Catalog=DB_Employee;User ID=ja;Password=john");
        public List<EmployeeAttendanceData> employeeAttendanceListData()
        {
            List<EmployeeAttendanceData> listdata = new List<EmployeeAttendanceData>();

            if (connect.State != ConnectionState.Open)
            {
                try
                {
                    connect.Open();

                    string selectData = "SELECT DATE, CLOCK_IN, CLOCK_OUT, " +
                        "CASE " +
                        "WHEN CLOCK_IN > '07:30:00' THEN 'Late' " +
                        "ELSE 'On Time' " +
                        "END AS STATUS, " +
                        "CASE " +
                        "WHEN DATEDIFF(MINUTE, '16:30', CLOCK_OUT) > 0 THEN " +
                        "CONVERT(VARCHAR, DATEADD(MINUTE, DATEDIFF(MINUTE, '16:30', CLOCK_OUT), 0), 108) " +
                        "ELSE '00:00:00' " +
                        "END AS OVERTIME " +
                        "FROM " +
                        "TBL_ATTENDANCE WHERE ACCOUNT_ID = @ACCOUNT_ID ORDER BY DATE DESC";

                    using (SqlCommand cmd = new SqlCommand(selectData, connect))
                    {
                        cmd.Parameters.AddWithValue("@ACCOUNT_ID", loginView.ID);
                        SqlDataReader reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            EmployeeAttendanceData ead = new EmployeeAttendanceData(loginView);
                            ead.Date = (DateTime)reader["DATE"];
                            if (!reader.IsDBNull(reader.GetOrdinal("CLOCK_IN")))
                            {
                                ead.ClockIn = DateTime.Today.Add((TimeSpan)reader["CLOCK_IN"]).ToString("hh:mm tt");
                            }

                            if (!reader.IsDBNull(reader.GetOrdinal("CLOCK_OUT")))
                            {
                                ead.ClockOut = DateTime.Today.Add((TimeSpan)reader["CLOCK_OUT"]).ToString("hh:mm tt");
                            }
                            ead.Status = reader["STATUS"].ToString();
                            ead.Overtime = reader["OVERTIME"].ToString();

                            listdata.Add(ead);
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
        public List<EmployeeAttendanceData> currentDayAttendanceListData(string date)
        {
            List<EmployeeAttendanceData> listdata = new List<EmployeeAttendanceData>();

            if (connect.State != ConnectionState.Open)
            {
                try
                {
                    connect.Open();

                    string selectData = "SELECT DATE, CLOCK_IN, CLOCK_OUT, " +
                        "CASE " +
                        "WHEN CLOCK_IN > '07:30:00' THEN 'Late' " +
                        "ELSE 'On Time' " +
                        "END AS STATUS, " +
                        "CASE " +
                        "WHEN DATEDIFF(MINUTE, '16:30', CLOCK_OUT) > 0 THEN " +
                        "CONVERT(VARCHAR, DATEADD(MINUTE, DATEDIFF(MINUTE, '16:30', CLOCK_OUT), 0), 108) " +
                        "ELSE '00:00:00' " +
                        "END AS OVERTIME " +
                        "FROM " +
                        "TBL_ATTENDANCE WHERE ACCOUNT_ID = @ACCOUNT_ID AND DATE = @DATE; ";

                    using (SqlCommand cmd = new SqlCommand(selectData, connect))
                    {
                        cmd.Parameters.AddWithValue("@ACCOUNT_ID", loginView.ID);
                        cmd.Parameters.AddWithValue("@DATE", date);
                        SqlDataReader reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            EmployeeAttendanceData ead = new EmployeeAttendanceData(loginView);
                            ead.Date = (DateTime)reader["DATE"];
                            if (!reader.IsDBNull(reader.GetOrdinal("CLOCK_IN")))
                            {
                                ead.ClockIn = DateTime.Today.Add((TimeSpan)reader["CLOCK_IN"]).ToString("hh:mm tt");
                            }

                            if (!reader.IsDBNull(reader.GetOrdinal("CLOCK_OUT")))
                            {
                                ead.ClockOut = DateTime.Today.Add((TimeSpan)reader["CLOCK_OUT"]).ToString("hh:mm tt");
                            }
                            ead.Status = reader["STATUS"].ToString();
                            ead.Overtime = reader["OVERTIME"].ToString();

                            listdata.Add(ead);
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
        public List<EmployeeAttendanceData> MonthYearAttendanceListData(int fromMonth, int fromYear, int toMonth, int toYear)
        {
            List<EmployeeAttendanceData> listdata = new List<EmployeeAttendanceData>();

            if (connect.State != ConnectionState.Open)
            {
                try
                {
                    connect.Open();

                    string selectData = "SELECT DATE, CLOCK_IN, CLOCK_OUT, " +
                        "CASE " +
                        "WHEN CLOCK_IN > '07:30:00' THEN 'Late' " +
                        "ELSE 'On Time' " +
                        "END AS STATUS, " +
                        "CASE " +
                        "WHEN DATEDIFF(MINUTE, '16:30', CLOCK_OUT) > 0 THEN " +
                        "CONVERT(VARCHAR, DATEADD(MINUTE, DATEDIFF(MINUTE, '16:30', CLOCK_OUT), 0), 108) " +
                        "ELSE '00:00:00' " +
                        "END AS OVERTIME " +
                        "FROM " +
                        "TBL_ATTENDANCE WHERE ACCOUNT_ID = @ACCOUNT_ID AND (YEAR(DATE) >= @StartYear AND MONTH(DATE) >= @StartMonth) " +
                        "AND (YEAR(DATE) <= @EndYear AND MONTH(DATE) <= @EndMonth) " +
                        "ORDER BY DATE ASC";

                    using (SqlCommand cmd = new SqlCommand(selectData, connect))
                    {
                        cmd.Parameters.AddWithValue("@ACCOUNT_ID", loginView.ID);
                        cmd.Parameters.AddWithValue("@StartMonth", fromMonth);
                        cmd.Parameters.AddWithValue("@StartYear", fromYear);
                        cmd.Parameters.AddWithValue("@EndMonth", toMonth);
                        cmd.Parameters.AddWithValue("@EndYear", toYear);
                        SqlDataReader reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            EmployeeAttendanceData ead = new EmployeeAttendanceData(loginView);
                            ead.Date = reader.GetDateTime(reader.GetOrdinal("DATE"));
                            if (!reader.IsDBNull(reader.GetOrdinal("CLOCK_IN")))
                            {
                                ead.ClockIn = DateTime.Today.Add(reader.GetTimeSpan(reader.GetOrdinal("CLOCK_IN"))).ToString("hh:mm tt");
                            }

                            if (!reader.IsDBNull(reader.GetOrdinal("CLOCK_OUT")))
                            {
                                ead.ClockOut = DateTime.Today.Add(reader.GetTimeSpan(reader.GetOrdinal("CLOCK_OUT"))).ToString("hh:mm tt");
                            }
                            ead.Status = reader.GetString(reader.GetOrdinal("STATUS"));
                            ead.Overtime = reader.GetString(reader.GetOrdinal("OVERTIME"));

                            listdata.Add(ead);
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
    }
}
