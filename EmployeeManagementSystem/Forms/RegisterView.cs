using EmployeeManagementSystem.Services;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EmployeeManagementSystem
{
    public partial class RegisterView : Form
    {
        AuthService authService;
        public RegisterView()
        {
            InitializeComponent();
            authService = new AuthService();
        }

        private void exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void signup_loginBtn_Click(object sender, EventArgs e)
        {
            LoginView loginForm = new LoginView();
            loginForm.Show();
            this.Hide();
        }

        private void signup_showPass_CheckedChanged(object sender, EventArgs e)
        {
            signup_password.PasswordChar = signup_showPass.Checked ? '\0' : '*';
        }

        private void signup_btn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(signup_username.Text) ||
                string.IsNullOrWhiteSpace(signup_password.Text) ||
                string.IsNullOrWhiteSpace(signup_id.Text))
            {
                MessageBox.Show(
                    "Please fill all blank fields.",
                    "Error Message",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return;
            }

            if (!int.TryParse(signup_id.Text.Trim(), out int employeeId))
            {
                MessageBox.Show(
                    "Please enter a valid employee ID.",
                    "Invalid ID",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            try
            {
                authService.Register(
                    employeeId,
                    signup_username.Text.Trim(),
                    signup_password.Text
                );

                MessageBox.Show(
                    "Registered successfully!",
                    "Information Message",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                LoginView loginForm = new LoginView();
                loginForm.Show();

                this.Hide();
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Registration Failed",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "An unexpected error occurred: " + ex.Message,
                    "Error Message",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
    }
}

