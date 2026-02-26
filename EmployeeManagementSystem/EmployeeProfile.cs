using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace EmployeeManagementSystem
{
    public partial class EmployeeProfile : UserControl
    {
        UserControl currentControl;
        LoginView loginView;
        DisplayUserProfileData data;
        SqlConnection connect =
            new SqlConnection(@"Data Source=LAPTOP-0OGAQKFF\SQLEXPRESS;Initial Catalog=DB_Employee;User ID=ja;Password=john");
        public EmployeeProfile(LoginView loginView)
        {
            InitializeComponent();
            this.loginView = loginView;
            groupBoxContactInfoEdit.Visible = false;
            groupBoxUserAccEdit.Visible = false;
            ProfileDislay();
        }
        public void changeCurrentPanel(UserControl newUserControl)
        {
            panel2.Controls.Remove(currentControl);
            panel2.Controls.Add(newUserControl);
            currentControl = newUserControl;
        }
        private void ProfileDislay()
        {
            lblFName.Text = loginView.Username;
            lblPassword.Text = new string('*', loginView.Password.Length);
            data = new DisplayUserProfileData(loginView);
            data.displayData();
            lblFullName.Text = data.FullName;
            lblPriContactNum.Text = data.PrimaryContactNum;
            lblSecContactNum.Text = data.SecContactNum;
            lblEmailAdd.Text = data.EmailAdd;
            lblEmergencyContactNum.Text = data.EmergencyContactNum;
            lblCurrentAdd.Text = data.CurrentAdd;
            lblEmpId.Text = data.EmployeeID;
            lblGender.Text = data.Gender;
            lblDateofBirth.Text = data.DateOfBirth.ToString("d");
            lblProfession.Text = data.Profession;
            pictureBox1.ImageLocation = data.Image;
        }

        private void btnUserAccCancel_Click(object sender, EventArgs e)
        {
            groupBoxUserAccDisp.Visible = true;
            groupBoxUserAccEdit.Visible = false;
        }

        private void btnContactInfoCancel_Click(object sender, EventArgs e)
        {
            groupBoxContactInfoDisp.Visible = true;
            groupBoxContactInfoEdit.Visible = false;
            errorProvider.SetError(textBoxEmailAdd, null);
            errorProvider.SetError(textBoxPrimaryContactNum, null);
            errorProvider.SetError(textBoxSecondaryContactNum, null);
            errorProvider.SetError(textBoxEmergencyContactNum, null);
            errorProvider.SetError(textBoxCurrentAdd, null);
        }

        private void btnUserAccSave_Click(object sender, EventArgs e)
        {
            
            UserAccSaveChanges();
        }

        private void btnContactInfoSave_Click(object sender, EventArgs e)
        {
            if (CheckError())
            {
                MessageBox.Show("Please check the error messages before saving it", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                ContactInfoSaveChanges();
            }
        }

        private void btnUserAccEdit_Click(object sender, EventArgs e)
        {
            groupBoxUserAccDisp.Visible = false;
            groupBoxUserAccEdit.Visible = true;
        }
        private void btnContactInfoEdit_Click(object sender, EventArgs e)
        {
            groupBoxContactInfoDisp.Visible = false;
            groupBoxContactInfoEdit.Visible = true;
            textBoxEmailAdd.Text = data.EmailAdd;
            textBoxPrimaryContactNum.Text = data.PrimaryContactNum;
            textBoxSecondaryContactNum.Text = data.SecContactNum;
            textBoxEmergencyContactNum.Text = data.EmergencyContactNum;
            textBoxCurrentAdd.Text = data.CurrentAdd;
        }
        public void UserAccSaveChanges()
        {
            try
            {
                connect.Open();

                string updateData = "UPDATE TBL_USERS SET USERNAME = @USERNAME " +
                    ", PASSWORD = @PASSWORD " +
                "WHERE ACCOUNT_ID = @ACCOUNT_ID";

                using (SqlCommand cmd = new SqlCommand(updateData, connect))
                {
                    if (loginView.Password == textBox2.Text.Trim())
                    {
                        loginView.Username = textBox1.Text.Trim();
                        loginView.Password = textBox2.Text.Trim();

                        cmd.Parameters.AddWithValue("@USERNAME", textBox1.Text.Trim());
                        cmd.Parameters.AddWithValue("@PASSWORD", textBox3.Text.Trim());
                        cmd.Parameters.AddWithValue("@ACCOUNT_ID", loginView.ID);

                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Update successfully!"
                        , "Information Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ProfileDislay();
                        UserAccUpdateChanges();
                    }
                    else
                    {
                        MessageBox.Show("Current password does not match"
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
        private void UserAccUpdateChanges()
        {
            groupBoxUserAccDisp.Visible = true;
            groupBoxUserAccEdit.Visible = false;
            textBox1.Text = string.Empty;
            textBox2.Text = string.Empty;
            textBox3.Text = string.Empty;
        }
        public void ContactInfoSaveChanges()
        {
            try
            {
                connect.Open();

                string updateData = @"UPDATE TBL_EMPLOYEE_INFO
                    SET 
                        EMAIL_ADD = @EMAIL_ADD, 
                        PRIMARY_CONTACT_NUM = @PRIMARY_CONTACT_NUM, 
                        SECONDARY_CONTACT_NUM = @SECONDARY_CONTACT_NUM, 
                        EMERGENCY_CONTACT_NUM = @EMERGENCY_CONTACT_NUM, 
                        CURRENT_ADD = @CURRENT_ADD
                    FROM 
                        TBL_EMPLOYEE_INFO
                        INNER JOIN 
                        TBL_USERS ON TBL_EMPLOYEE_INFO.ID = TBL_USERS.ID
                    WHERE 
                        TBL_USERS.ACCOUNT_ID = @ACCOUNT_ID";

                using (SqlCommand cmd = new SqlCommand(updateData, connect))
                {
                    cmd.Parameters.AddWithValue("@EMAIL_ADD", textBoxEmailAdd.Text.Trim());
                    cmd.Parameters.AddWithValue("@PRIMARY_CONTACT_NUM", textBoxPrimaryContactNum.Text.Trim());
                    cmd.Parameters.AddWithValue("@SECONDARY_CONTACT_NUM", textBoxSecondaryContactNum.Text.Trim());
                    cmd.Parameters.AddWithValue("@EMERGENCY_CONTACT_NUM", textBoxEmergencyContactNum.Text.Trim());
                    cmd.Parameters.AddWithValue("@CURRENT_ADD", textBoxCurrentAdd.Text.Trim());
                    cmd.Parameters.AddWithValue("@ACCOUNT_ID", loginView.ID);

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Update successfully!"
                    , "Information Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ProfileDislay();
                    ContactInfoUpdateChanges();
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
        private void ContactInfoUpdateChanges()
        {
            groupBoxContactInfoDisp.Visible = true;
            groupBoxContactInfoEdit.Visible = false;
            textBoxEmailAdd.Text = string.Empty;
            textBoxPrimaryContactNum.Text = string.Empty;
            textBoxSecondaryContactNum.Text = string.Empty;
            textBoxEmergencyContactNum.Text = string.Empty;
            textBoxCurrentAdd.Text = string.Empty;
        }
        

        private void textBoxPrimaryContactNum_Validating(object sender, CancelEventArgs e)
        {
            string pat1 = @"^(09|\+639)\d{10,}$";
            string pat2 = @"^(09|\+639)\d{0,8}$";
            string pat3 = @"[^0-9+]";
            string pat4 = @"^(09|\+639)\d{9}$";
            Regex regex = new Regex(pat1);
            Regex regex2 = new Regex(pat2);
            Regex regex3 = new Regex(pat3);
            Regex regex4 = new Regex(pat4);
            if (textBoxPrimaryContactNum.Text.Equals(string.Empty))
            {
                errorProvider.SetError(textBoxPrimaryContactNum, "Primary Contact Number field is required");
            }
            else if (regex.IsMatch(textBoxPrimaryContactNum.Text))
            {
                errorProvider.SetError(textBoxPrimaryContactNum, "Primary Contact Number length has exceeded");
            }
            else if (regex2.IsMatch(textBoxPrimaryContactNum.Text) || !regex4.IsMatch(textBoxPrimaryContactNum.Text))
            {
                errorProvider.SetError(textBoxPrimaryContactNum, "Primary Contact Number has missing number/s");
            }
            else if (regex3.IsMatch(textBoxPrimaryContactNum.Text))
            {
                errorProvider.SetError(textBoxPrimaryContactNum, "Primary Contact Number cannot contain alphabet and special characters(except '+')");
            }
            else
            {
                errorProvider.SetError(textBoxPrimaryContactNum, null); // Clear the error if validation passes
            }
        }

        private void textBoxSecondaryContactNum_Validating(object sender, CancelEventArgs e)
        {
            string pat1 = @"^(09|\+639)\d{10,}$";
            string pat2 = @"^(09|\+639)\d{0,8}$";
            string pat3 = @"[^0-9+]";
            string pat4 = @"^(09|\+639)\d{9}$";
            Regex regex = new Regex(pat1);
            Regex regex2 = new Regex(pat2);
            Regex regex3 = new Regex(pat3);
            Regex regex4 = new Regex(pat4);

            if (textBoxPrimaryContactNum.Text.Equals(string.Empty))
            {
                errorProvider.SetError(textBoxPrimaryContactNum, null);
            }
            else if (regex.IsMatch(textBoxSecondaryContactNum.Text))
            {
                errorProvider.SetError(textBoxSecondaryContactNum, "Phone Number length has exceeded");
            }
            else if (regex2.IsMatch(textBoxSecondaryContactNum.Text) || !regex4.IsMatch(textBoxSecondaryContactNum.Text))
            {
                errorProvider.SetError(textBoxSecondaryContactNum, "Phone Number has missing number/s");
            }
            else if (regex3.IsMatch(textBoxSecondaryContactNum.Text))
            {
                errorProvider.SetError(textBoxSecondaryContactNum, "Phone Number cannot contain alphabet and special characters(except '+')");
            }
            else
            {
                errorProvider.SetError(textBoxSecondaryContactNum, null); // Clear the error if validation passes
            }
        }

        private void textBoxEmergencyContactNum_Validating(object sender, CancelEventArgs e)
        {
            string pat1 = @"^(09|\+639)\d{10,}$";
            string pat2 = @"^(09|\+639)\d{0,8}$";
            string pat3 = @"[^0-9+]";
            string pat4 = @"^(09|\+639)\d{9}$";
            Regex regex = new Regex(pat1);
            Regex regex2 = new Regex(pat2);
            Regex regex3 = new Regex(pat3);
            Regex regex4 = new Regex(pat4);
            if (textBoxEmergencyContactNum.Text.Equals(string.Empty))
            {
                errorProvider.SetError(textBoxEmergencyContactNum, "Emergency Contact Number field is required");
            }
            else if (regex.IsMatch(textBoxEmergencyContactNum.Text))
            {
                errorProvider.SetError(textBoxEmergencyContactNum, "Emergency Contact Number length has exceeded");
            }
            else if (regex2.IsMatch(textBoxEmergencyContactNum.Text) || !regex4.IsMatch(textBoxEmergencyContactNum.Text))
            {
                errorProvider.SetError(textBoxEmergencyContactNum, "Emergency Contact Number has missing number/s");
            }
            else if (regex3.IsMatch(textBoxEmergencyContactNum.Text))
            {
                errorProvider.SetError(textBoxEmergencyContactNum, "Emergency Contact Number cannot contain alphabet and special characters(except '+')");
            }
            else
            {
                errorProvider.SetError(textBoxEmergencyContactNum, null); // Clear the error if validation passes
            }
        }

        private void textBoxEmailAdd_Validating(object sender, CancelEventArgs e)
        {
            string pat1 = "^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9-]+\\.[a-zA-Z]{2,}$";
            Regex regex1 = new Regex(pat1);

            if (textBoxEmailAdd.Text.Equals(string.Empty))
            {
                errorProvider.SetError(textBoxEmailAdd, "Email Address field is required");
            }
            else if (!regex1.IsMatch(textBoxEmailAdd.Text.Trim()))
            {
                errorProvider.SetError(textBoxEmailAdd, "The Email Address is invalid");
            }
            else
            {
                errorProvider.SetError(textBoxEmailAdd, null);
            }
        }

        private void textBoxCurrentAdd_Validating(object sender, CancelEventArgs e)
        {
            string pat1 = @"^[0-9]*, .*, .*, \D*, \D*$";
            Regex regex1 = new Regex(pat1);

            if (textBoxCurrentAdd.Text.Equals(string.Empty))
            {
                errorProvider.SetError(textBoxCurrentAdd, "Current Address field is required");
            }
            else if (!regex1.IsMatch(textBoxCurrentAdd.Text.Trim()))
            {
                errorProvider.SetError(textBoxCurrentAdd, "Make sure all necessary details are provided and the format is correct");
            }
            else
            {
                errorProvider.SetError(textBoxCurrentAdd, null);
            }
        }
        private bool CheckError()
        {
            this.ValidateChildren();

            bool emailAddError = !string.IsNullOrWhiteSpace(errorProvider.GetError(textBoxEmailAdd));
            bool primaryContactNumError = !string.IsNullOrWhiteSpace(errorProvider.GetError(textBoxPrimaryContactNum));
            bool secondaryContactNumError = !string.IsNullOrWhiteSpace(errorProvider.GetError(textBoxSecondaryContactNum));
            bool emergencyContactNumError = !string.IsNullOrWhiteSpace(errorProvider.GetError(textBoxEmergencyContactNum));
            bool currentAddError = !string.IsNullOrWhiteSpace(errorProvider.GetError(textBoxCurrentAdd));
            if (emailAddError || primaryContactNumError || emergencyContactNumError || currentAddError)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void addEmployee_importBtn_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = "Image Files |*.gif;*.jpg;*.jpeg;*.bmp;*.wmf;*.png";
                string imagePath = "";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    imagePath = dialog.FileName;
                    pictureBox1.ImageLocation = imagePath;
                }

                string path = Path.Combine(@"C:\Users\John April\Downloads\EmployeeManagementSystem\EmployeeManagementSystem\EmployeeManagementSystem\ImageLocation\"
                                    + lblEmpId.Text.Trim() + ".jpg");

                string directoryPath = Path.GetDirectoryName(path);

                if (!pictureBox1.ImageLocation.Equals(path))
                {
                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }
                    else if (File.Exists(path))
                    {
                        File.Delete(path);
                    }

                    File.Copy(pictureBox1.ImageLocation, path, true);
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
