using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EmployeeManagementSystem
{
    public partial class UserProfileEdit : UserControl
    {
        UserProfile userProfile;
        LoginView loginView;
        SqlConnection connect =
            new SqlConnection(@"Data Source=LAPTOP-0OGAQKFF\SQLEXPRESS;Initial Catalog=DB_Employee;User ID=ja;Password=john");
        public UserProfileEdit(UserProfile user, LoginView loginView)
        {
            InitializeComponent();
            this.loginView = loginView;
            this.userProfile = user;
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                connect.Open();

                string updateData = "UPDATE TBL_USERS SET USERNAME = @USERNAME " +
                    ", PASSWORD = @PASSWORD " +
                "WHERE ACCOUNT_ID = @ACCOUNT_ID";

                using (SqlCommand cmd = new SqlCommand(updateData, connect))
                {
                    cmd.Parameters.AddWithValue("@USERNAME", textBox1.Text.Trim());
                    cmd.Parameters.AddWithValue("@PASSWORD", textBox3.Text.Trim());
                    cmd.Parameters.AddWithValue("@ACCOUNT_ID", loginView.ID);

                    cmd.ExecuteNonQuery();

                    if (loginView.Password == textBox2.Text.Trim())
                    {
                        loginView.Username = textBox1.Text.Trim();
                        loginView.Password = textBox2.Text.Trim();
                        MessageBox.Show("Update successfully!"
                        , "Information Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        userProfile.changeCurrentPanel(new UserProfileDisplay(userProfile, loginView));
                    }
                    else
                    {
                        MessageBox.Show("Password incorrect"
                        , "Information Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            userProfile.changeCurrentPanel(new UserProfileDisplay(userProfile, loginView));
        }
    }
}
