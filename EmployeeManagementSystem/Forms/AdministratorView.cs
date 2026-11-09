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
    public partial class AdministratorView : Form
    {
        UserControl currentControl;
        User _currentUser;
        LoginView loginView;
        public AdministratorView(User user)
        {
            InitializeComponent();
            loginView = new LoginView();
            _currentUser = user;
            currentControl = new AdminDashboard(_currentUser);
            panel4.Controls.Add(currentControl);
            greet_user.Text += _currentUser.Username;
        }
        private void exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void logout_btn_Click_1(object sender, EventArgs e)
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
            changeCurrentPanel(new AdminDashboard(_currentUser));
        }

        private void addEmployee_btn_Click(object sender, EventArgs e)
        {
            changeCurrentPanel(new AdminAddEmployee(this));
        }

        private void salary_btn_Click(object sender, EventArgs e)
        {
            changeCurrentPanel(new AdminSalary());
        }
        public void changeCurrentPanel(UserControl newUserControl)
        {
            panel4.Controls.Remove(currentControl);
            panel4.Controls.Add(newUserControl);
            currentControl = newUserControl;
        }

        private void label1_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            changeCurrentPanel(new EventCalendar(_currentUser));
        }

        private void btnAttendance_Click(object sender, EventArgs e)
        {
            changeCurrentPanel(new AdminAttendance());
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}