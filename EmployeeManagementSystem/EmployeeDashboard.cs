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

namespace EmployeeManagementSystem
{
    public partial class EmployeeDashboard : UserControl
    {
        LoginView loginView;
        SqlConnection connect =
            new SqlConnection("Data Source=LAPTOP-0OGAQKFF\\SQLEXPRESS;Initial Catalog=DB_Employee;User ID=ja;Password=john");
        public EmployeeDashboard(LoginView loginView)
        {
            InitializeComponent();
            this.loginView = loginView;
            DisplayData();
        }
        private void DisplayData()
        {
            EmployeeDashboardData employeeDashboardData = new EmployeeDashboardData(loginView);
            dashboard_OnTime.Text = employeeDashboardData.OnTimeData().ToString();
            dashboard_Late.Text = employeeDashboardData.LateData().ToString();
            dashboard_Overtime.Text = employeeDashboardData.OvertimeData().ToString();
        }
    }
}
