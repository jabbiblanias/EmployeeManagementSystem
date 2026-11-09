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
using EmployeeManagementSystem.Models;
using EmployeeManagementSystem.Services;

namespace EmployeeManagementSystem
{
    public partial class EmployeeDashboard : UserControl
    {
        private readonly User _currentUser;
        private readonly DashboardService _dashboardService;
        public EmployeeDashboard(User currentUser)
        {
            InitializeComponent();
            this._currentUser = currentUser;
            this._dashboardService = new DashboardService();
            DisplayData();
        }
        private void DisplayData()
        {
            try
            {
                int accountId = _currentUser.ID;

                int onTime = _dashboardService.GetOnTimeCount(accountId);
                dashboard_OnTime.Text = onTime.ToString();

                int late = _dashboardService.GetLateCount(accountId);
                dashboard_Late.Text = late.ToString();

                TimeSpan overtime = _dashboardService.GetOvertime(accountId);
                dashboard_Overtime.Text = overtime.ToString(@"hh\:mm");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Dashboard Error");
            }
        }
    }
}
