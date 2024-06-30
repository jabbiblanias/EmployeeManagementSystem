using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace EmployeeManagementSystem
{
    public partial class UserProfileDisplay : UserControl
    {
        UserProfile user;
        LoginView loginView;
        SqlConnection connect =
            new SqlConnection(@"Data Source=LAPTOP-0OGAQKFF\SQLEXPRESS;Initial Catalog=DB_Employee;User ID=ja;Password=john");
        public UserProfileDisplay(UserProfile userProfile, LoginView loginView)
        {
            InitializeComponent();
            this.loginView = loginView;
            this.user = userProfile;
            ProfileDislay();
        }
        private void ProfileDislay()
        {
            lblFName.Text = loginView.Username;
            lblPassword.Text = new string('*', loginView.Password.Length);
        }

        private void gpBasicInfo_Enter(object sender, EventArgs e)
        {

        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            user.changeCurrentPanel(new UserProfileEdit(user, loginView));
        }
    }
}
