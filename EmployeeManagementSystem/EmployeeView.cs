using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EmployeeManagementSystem
{
    public partial class EmployeeView : Form
    {
        UserControl currentControl;
        LoginView loginView;
        public EmployeeView(LoginView loginView)
        {
            InitializeComponent();
            this.loginView = loginView;
            greet_user.Text += loginView.Username;
            changeCurrentPanel(new EmployeeAttendance(loginView));
        }
        private void changeCurrentPanel(UserControl newUserControl)
        {
            panel4.Controls.Remove(currentControl);
            panel4.Controls.Add(newUserControl);
            currentControl = newUserControl;
        }

        private void btnAttendance_Click(object sender, EventArgs e)
        {
            changeCurrentPanel(new EmployeeAttendance(loginView));
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
            changeCurrentPanel(new EventCalendar(loginView));
        }

        private void btnProfile_Click(object sender, EventArgs e)
        {
            changeCurrentPanel(new UserProfile(loginView));
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
    }
}
