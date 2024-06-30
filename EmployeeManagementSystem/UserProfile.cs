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
    public partial class UserProfile : UserControl
    {
        UserControl currentControl;
        LoginView loginView;
        public UserProfile(LoginView loginView)
        {
            InitializeComponent();
            this.loginView = loginView;
            changeCurrentPanel(new UserProfileDisplay(this, loginView));
        }
        public void changeCurrentPanel(UserControl newUserControl)
        {
            panel2.Controls.Remove(currentControl);
            panel2.Controls.Add(newUserControl);
            currentControl = newUserControl;
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            
            
        }
    }
}
