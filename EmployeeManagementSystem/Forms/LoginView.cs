using EmployeeManagementSystem.Services;
using System;
using System.Data.SqlClient;
using System.Windows.Forms;
using EmployeeManagementSystem.Models;

namespace EmployeeManagementSystem
{
    public partial class LoginView : Form
    {
        private readonly AuthService _authService;
        public LoginView()
        {
            InitializeComponent();
            _authService = new AuthService();
        }

        private void exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void login_btn_Click(object sender, EventArgs e)
        {
            try
            {
                User user = _authService.Login(
                    login_username.Text,
                    login_password.Text);

                if (user == null)
                {
                    MessageBox.Show(
                        "Invalid username or password, or account inactive.",
                        "Login Failed",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return;
                }

                MessageBox.Show("Login successfully!"
                        , "Information Message", MessageBoxButtons.OK, MessageBoxIcon.Information);

                if (user.Role == "Administrator")
                {
                    AdministratorView adminView = new AdministratorView(user);
                    adminView.Show();
                    this.Hide();
                }
                else
                {
                    EmployeeView empView = new EmployeeView(user);
                    empView.Show();
                    this.Hide();
                }
            }
            catch (SqlException)
            {
                MessageBox.Show(
                    "Unable to connect to the database. Please try again.",
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            catch(Exception ex)
            {
                MessageBox.Show(
                    $"An unexpected error occurred: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
        private void login_signupBtn_Click(object sender, EventArgs e)
        {
            RegisterView regForm = new RegisterView();
            regForm.Show();
            this.Hide();
        }

        private void login_showPass_CheckedChanged(object sender, EventArgs e)
        {
            login_password.PasswordChar = login_showPass.Checked ? '\0' : '*';
        }

        private void label1_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
    }
}
