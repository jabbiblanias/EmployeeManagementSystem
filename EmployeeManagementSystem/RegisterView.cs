using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
namespace EmployeeManagementSystem
{
    public partial class RegisterView : Form
    {
        SqlConnection connect 
            = new SqlConnection(@"Data Source=LAPTOP-0OGAQKFF\SQLEXPRESS;Initial Catalog=DB_Employee;User ID=ja;Password=john");
        public RegisterView()
        {
            InitializeComponent();
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
            if (signup_username.Text == ""
                || signup_password.Text == "" || signup_id.Text == "")
            {
                MessageBox.Show("Please fill all blank fields"
                    , "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (connect.State != ConnectionState.Open)
                {
                    try
                    {
                        connect.Open();
                        string query = @"SELECT 
                            CASE 
                                WHEN NOT EXISTS (SELECT 1 FROM TBL_EMPLOYEE_INFO WHERE ID = @ID) THEN 1 ELSE 0 
                            END AS IDNotExists,
                            CASE 
                                WHEN EXISTS (SELECT 1 FROM TBL_USERS WHERE ID = @ID) THEN 1 ELSE 0 
                            END AS UserIDExists,
                            CASE 
                                WHEN EXISTS (SELECT 1 FROM TBL_USERS WHERE USERNAME = @USERNAME) THEN 1 ELSE 0 
                            END AS UsernameExists;";

                        using (SqlCommand command = new SqlCommand(query, connect))
                        {
                            command.Parameters.AddWithValue("@ID", signup_id.Text.Trim());
                            command.Parameters.AddWithValue("@USERNAME", signup_username.Text.Trim());

                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    bool idNotExists = reader.GetInt32(0) == 1;
                                    bool userIdExists = reader.GetInt32(1) == 1;
                                    bool usernameExists = reader.GetInt32(2) == 1;

                                    if (idNotExists)
                                    {
                                        MessageBox.Show(" The ID is not recorded in the employee record"
                                            , "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    }
                                    else if (userIdExists)
                                    {
                                        MessageBox.Show(" The ID is already exist"
                                            , "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    }else if (usernameExists)
                                    {
                                        MessageBox.Show("The Username is already taken"
                                            , "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    }
                                    else
                                    {
                                        connect.Close();
                                        connect.Open();
                                        DateTime today = DateTime.Today;

                                        string insertData = "INSERT INTO TBL_USERS " +
                                            "(ID, USERNAME, PASSWORD, DATE_REGISTER) " +
                                            "VALUES(@ID, @USERNAME, @PASSWORD, @DATE_REGISTER)";

                                        using (SqlCommand cmd = new SqlCommand(insertData, connect))
                                        {
                                            cmd.Parameters.AddWithValue("@ID", signup_id.Text.Trim());
                                            cmd.Parameters.AddWithValue("@USERNAME", signup_username.Text.Trim());
                                            cmd.Parameters.AddWithValue("@PASSWORD", signup_password.Text.Trim());
                                            cmd.Parameters.AddWithValue("@DATE_REGISTER", today);

                                            cmd.ExecuteNonQuery();

                                            MessageBox.Show("Registered successfully!"
                                                , "Information Message", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                            LoginView loginForm = new LoginView();
                                            loginForm.Show();
                                            this.Hide();
                                        }
                                    }
                                }
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

        private void label1_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void RegisterForm_MouseDown(object sender, MouseEventArgs e)
        {
            
        }
    }
}

