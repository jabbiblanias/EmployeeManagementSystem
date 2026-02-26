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
    public partial class LoginView : Form
    {
        public int ID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        SqlConnection connect
            = new SqlConnection(@"Data Source=LAPTOP-0OGAQKFF\SQLEXPRESS;Initial Catalog=DB_Employee;User ID=ja;Password=john");
        public LoginView()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 1;
        }

        private void exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void login_btn_Click(object sender, EventArgs e)
        {
            if (login_username.Text == ""
               || login_password.Text == "" || comboBox1.SelectedIndex == -1)
            {
                MessageBox.Show("Please fill all blank fields"
                    , "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (connect.State == ConnectionState.Closed)
                {
                    try
                    {
                        connect.Open();

                        string selectData = "SELECT TOP 1 ACCOUNT_ID FROM TBL_USERS " +
                            "WHERE USERNAME = @USERNAME AND PASSWORD = @PASSWORD AND ROLE = @ROLE";

                        using (SqlCommand cmd = new SqlCommand(selectData, connect))
                        {
                            cmd.Parameters.AddWithValue("@USERNAME", login_username.Text.Trim());
                            cmd.Parameters.AddWithValue("@PASSWORD", login_password.Text.Trim());
                            cmd.Parameters.AddWithValue("@ROLE", comboBox1.SelectedItem.ToString());

                            SqlDataReader reader = cmd.ExecuteReader();
                            if (reader.Read())
                            {
                                Username = login_username.Text.Trim();
                                Password = login_password.Text.Trim();
                                Role = comboBox1.SelectedItem.ToString();
                                ID = Convert.ToInt32(reader["ACCOUNT_ID"]);
                                
                                MessageBox.Show("Login successfully!"
                                    , "Information Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                
                                if (comboBox1.Items[0].ToString().Equals(comboBox1.SelectedItem.ToString()))
                                {
                                    AdministratorView adminView = new AdministratorView(this);
                                    adminView.Show();
                                    this.Hide();
                                }
                                else
                                {
                                    EmployeeView empView = new EmployeeView(this);
                                    empView.Show();
                                    this.Hide();
                                }  
                            }
                            else
                            {
                                MessageBox.Show("Incorrect Username/Password/Role"
                                    , "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void login_username_Leave(object sender, EventArgs e)
        {

        }

        private void LoginView_Load(object sender, EventArgs e)
        {

        }
    }
}
