using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EmployeeManagementSystem.Models;

namespace EmployeeManagementSystem
{
    public partial class EmployeeView : Form
    {
        UserControl currentControl;
        private readonly User _currentUser;
        LoginView loginView;
        public EmployeeView(User user)
        {
            InitializeComponent();
            loginView = new LoginView();
            this._currentUser = user;
            greet_user.Text += _currentUser.Username;
            changeCurrentPanel(new EmployeeDashboard(_currentUser));
        }
        private void changeCurrentPanel(UserControl newUserControl)
        {
            panel4.Controls.Remove(currentControl);
            panel4.Controls.Add(newUserControl);
            currentControl = newUserControl;
        }

        private void btnAttendance_Click(object sender, EventArgs e)
        {
            changeCurrentPanel(new EmployeeAttendance(_currentUser));
        }

        private void exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void minimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void btnCalendar_Click(object sender, EventArgs e)
        {
            changeCurrentPanel(new EventCalendar(_currentUser));
        }

        private void btnProfile_Click(object sender, EventArgs e)
        {
            changeCurrentPanel(new EmployeeProfile(_currentUser));
        }

        private void logout_btn_Click(object sender, EventArgs e)
        {
            DialogResult check = MessageBox.Show("Are you sure you want to logout?"
               , "Confirmation Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (check == DialogResult.Yes)
            {
                loginView.Show();
                this.Hide();
            }
        }

        private void dashboard_btn_Click(object sender, EventArgs e)
        {
            changeCurrentPanel(new EmployeeDashboard(_currentUser));
        }
    }
}
