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
using EmployeeManagementSystem.Models;
using EmployeeManagementSystem.Services;

namespace EmployeeManagementSystem
{
    public partial class AdminDashboard : UserControl
    {
        private readonly User _currentUser;
        private readonly DashboardService _dashboardService;
        public AdminDashboard(User user)
        {
            InitializeComponent();
            this._currentUser = user;
            _dashboardService = new DashboardService();
            DisplayCount();
            WeeklyAttendanceChart();
            DisplayEvents();
            DisplayTopFive();
        }

        public void DisplayCount()
        {
            dashboard_TE.Text = _dashboardService.GetTotalEmployees().ToString();
            dashboard_AE.Text = _dashboardService.GetActiveEmployees().ToString();
            dashboard_IE.Text = _dashboardService.GetInactiveEmployees().ToString();
        }
        private void WeeklyAttendanceChart()
        {
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
                        series1.Points.AddXY(newDate.Date, _dashboardService.GetOnTimeCount(newDate.Date));
                        series2.Points.AddXY(newDate.Date, _dashboardService.GetLateCount(newDate.Date));
                    }
                    int daysInMonth = DateTime.DaysInMonth(today.Year, today.Month - 1);
                    for (int i = daysInMonth; datediff >= 1; i--, datediff--)
                    {
                        newDate = new DateTime(today.Year, today.Month - 1, i);
                        series1.Points.AddXY(newDate.Date, _dashboardService.GetOnTimeCount(newDate.Date));
                        series2.Points.AddXY(newDate.Date, _dashboardService.GetLateCount(newDate.Date));
                    }
                }
                else
                {
                    DateTime date = new DateTime(today.Year, today.Month, today.Day - 6);
                    int currentDay = date.Day;
                    for (int i = 1; i <= 6; i++, currentDay++)
                    {
                        date = new DateTime(today.Year, today.Month, currentDay);
                        series1.Points.AddXY(date.Date, _dashboardService.GetOnTimeCount(date.Date));
                        series2.Points.AddXY(date.Date, _dashboardService.GetLateCount(date.Date));
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
                    series1.Points.AddXY(date.Date, _dashboardService.GetOnTimeCount(date.Date));
                    series2.Points.AddXY(date.Date, _dashboardService.GetLateCount(date.Date));
                }
            }
        }
        public void DisplayEvents()
        {
            try
            {
                listBox1.Items.Clear();
                List<string> events = _dashboardService.GetEvents(_currentUser.ID);
                int count = events.Count();
                if (count > 0)
                {
                    foreach(var ev in events)
                    {
                        listBox1.Items.Add(ev);
                    }
                }
                else
                {
                    listBox1.Items.Add("*No events for today*");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex
                    , "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void DisplayTopFive()
        {
            try
            {
                listBox2.Items.Clear();
                List<string> topFive = _dashboardService.GetTopFiveEmployees();
                int count = topFive.Count();
                if (count > 0)
                {
                    foreach (string top in topFive)
                    {
                        listBox2.Items.Add(top);
                    }
                }
                else
                {
                    listBox2.Items.Add("*No one has met the attendance time*");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex
                    , "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }    
}
