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
    public partial class AdminEmployeeProfile : UserControl
    {
        AdministratorView administratorView;
        private string empId;
        AdminAddEmployee AdminAddEmployee;
        public AdminEmployeeProfile(AdministratorView administratorView, string empId)
        {
            InitializeComponent();
            this.administratorView = administratorView;
            this.empId = empId;
            Display();
        }

        private void pictureBoxBack_Click(object sender, EventArgs e)
        {
            AdminAddEmployee = new AdminAddEmployee(administratorView);
            if(lblStatus.Text == "active")
            {
                AdminAddEmployee.btnArchiveList.Text = "Active List";
            }
            else
            {
                AdminAddEmployee.btnArchiveList.Text = "Archive List";
            }
            AdminAddEmployee.checkArchive();
            administratorView.changeCurrentPanel(AdminAddEmployee);
        }
        private void Display()
        {
            ProfileData profileData = new ProfileData();
            profileData.EmployeeID = empId;
            profileData.EmployeeInfo();
            lblEmpId.Text = empId;
            lblProfession.Text = profileData.Profession;
            lblFullName.Text = profileData.FullName;
            lblGender.Text = profileData.Gender;
            lblDateofBirth.Text = profileData.DateOfBirth.ToString("d");
            lblEmailAdd.Text = profileData.EmailAdd;
            lblCurrentAdd.Text = profileData.CurrentAdd;
            lblPriContactNum.Text = profileData.PrimaryContactNum;
            lblSecContactNum.Text = profileData.SecContactNum;
            lblEmergencyContactNum.Text= profileData.EmergencyContactNum;
            lblStatus.Text = profileData.Status;
            lblDateCreated.Text = profileData.DateCreated.ToString("d");
            lblDateModified.Text = profileData.DateModified.ToString("d");
            pictureBox1.ImageLocation = profileData.Image;
        }
    }
}
