using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Forms.DataVisualization;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;

namespace EmployeeManagementSystem
{
    public partial class AdminDashboard : UserControl
    {
        LoginView loginView;
        SqlConnection connect =
            new SqlConnection("Data Source=LAPTOP-0OGAQKFF\\SQLEXPRESS;Initial Catalog=DB_Employee;User ID=ja;Password=john");
        public AdminDashboard(LoginView loginView)
        {
            this.loginView = loginView;
            InitializeComponent();
            DisplayCount();
            WeeklyAttendanceChart();
            DisplayEvents();
            DisplayTop5();
        }

        public void DisplayCount()
        {
            AdminDashboardData adminDashboardData = new AdminDashboardData();
            dashboard_TE.Text = adminDashboardData.totalOfEmployees().ToString();
            dashboard_AE.Text = adminDashboardData.numberOfActiveEmployees().ToString();
            dashboard_IE.Text = adminDashboardData.numberOfInactiveEmployees().ToString();
        }
        private void WeeklyAttendanceChart()
        {
            AdminDashboardData adminDashboardData = new AdminDashboardData();

            Series series1 = weeklyAttendance.Series["On Time"];
            Series series2 = weeklyAttendance.Series["Late"];

            // Clear any existing data points (if necessary)
            series1.Points.Clear();
            series2.Points.Clear();

            DateTime today = DateTime.Today;
            int day = today.Day;

            if (today.DayOfWeek == DayOfWeek.Sunday)
            {
                int datediff;
                if (day <= 6)
                {
                    datediff = Math.Abs(day - 6);
                    DateTime newDate;
                    for (int i = day; i >= 1; i--)
                    {
                        newDate = new DateTime(today.Year, today.Month, i);
                        series1.Points.AddXY(newDate.Date, adminDashboardData.OnTimeData(newDate.Date));
                        series2.Points.AddXY(newDate.Date, adminDashboardData.LateData(newDate.Date));
                    }
                    int daysInMonth = DateTime.DaysInMonth(today.Year, today.Month - 1);
                    for (int i = daysInMonth; datediff >= 1; i--, datediff--)
                    {
                        newDate = new DateTime(today.Year, today.Month - 1, i);
                        series1.Points.AddXY(newDate.Date, adminDashboardData.OnTimeData(newDate.Date));
                        series2.Points.AddXY(newDate.Date, adminDashboardData.LateData(newDate.Date));
                    }
                }
                else
                {
                    DateTime date = new DateTime(today.Year, today.Month, today.Day - 6);
                    int currentDay = date.Day;
                    for (int i = 1; i <= 6; i++, currentDay++)
                    {
                        date = new DateTime(today.Year, today.Month, currentDay);
                        series1.Points.AddXY(date.Date, adminDashboardData.OnTimeData(date.Date));
                        series2.Points.AddXY(date.Date, adminDashboardData.LateData(date.Date));
                    }
                }
            }
            else
            {
                int dayOfWeek = (int)today.DayOfWeek;
                int currentDay = today.Day;
                for (int i = dayOfWeek; i >= 1; i--, currentDay--)
                {
                    DateTime date = new DateTime(today.Year, today.Month, currentDay);
                    series1.Points.AddXY(date.Date, adminDashboardData.OnTimeData(date.Date));
                    series2.Points.AddXY(date.Date, adminDashboardData.LateData(date.Date));
                }
            }
        }
        public void DisplayEvents()
        {
            try
            {
                listBox1.Items.Clear();
                connect.Open();
                string selectData = "SELECT * FROM TBL_EVENTS INNER JOIN TBL_USERS ON TBL_EVENTS.ACCOUNT_ID = TBL_USERS.ACCOUNT_ID WHERE TBL_EVENTS.EVENT_DATE = @EVENT_DATE AND TBL_USERS.ROLE = 'ADMINISTRATOR'";
                using (SqlCommand cmd = new SqlCommand(selectData, connect))
                {
                    cmd.Parameters.AddWithValue("@ACCOUNT_ID", loginView.ID);
                    cmd.Parameters.AddWithValue("@EVENT_DATE", DateTime.Today);
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            listBox1.Items.Add("Title: " + reader["TITLE"].ToString());
                            listBox1.Items.Add($"Time: {DateTime.Today.Add(reader.GetTimeSpan(reader.GetOrdinal("START_TIME"))).ToString("hh:mm tt")} - {DateTime.Today.Add(reader.GetTimeSpan(reader.GetOrdinal("END_TIME"))).ToString("hh:mm tt")}");
                            listBox1.Items.Add("");
                        }
                    }
                    else
                    {
                        listBox1.Items.Add("*No events for today*");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex
                    , "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                connect.Close();
            }
        }
        public void DisplayTop5()
        {
            try
            {
                listBox2.Items.Clear();
                connect.Open();
                string selectData = "SELECT DISTINCT DATE, FULL_NAME FROM (" +
                    "SELECT TBL_ATTENDANCE.DATE,TBL_EMPLOYEE_INFO.FULL_NAME,ROW_NUMBER() OVER (ORDER BY TBL_ATTENDANCE.DATE) AS RowNum FROM TBL_EMPLOYEE_INFO INNER JOIN TBL_USERS ON TBL_EMPLOYEE_INFO.ID = TBL_USERS.ID INNER JOIN TBL_ATTENDANCE ON TBL_USERS.ACCOUNT_ID = TBL_ATTENDANCE.ACCOUNT_ID WHERE TBL_ATTENDANCE.DATE = @DATE AND TBL_ATTENDANCE.STATUS = 'On Time'" +
                    ")AS RowConstrainedResult " +
                    "WHERE RowNum >= 1 AND RowNum <= 5 " +
                    "ORDER BY DATE";
                using (SqlCommand cmd = new SqlCommand(selectData, connect))
                {
                    cmd.Parameters.AddWithValue("@DATE", DateTime.Today);
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        int i = 1;
                        while (reader.Read())
                        {
                            listBox2.Items.Add($"{i}: " + reader["FULL_NAME"].ToString());
                            i++;
                        }
                    }
                    else
                    {
                        listBox2.Items.Add("*No one has met the attendance time*");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex
                    , "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                connect.Close();
            }
        }
    }    
}
