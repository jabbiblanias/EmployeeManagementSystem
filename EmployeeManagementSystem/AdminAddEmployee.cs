using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace EmployeeManagementSystem
{
    public partial class AdminAddEmployee : UserControl
    {
        AdministratorView administratorView;
        SqlConnection connect =
            new SqlConnection(@"Data Source=LAPTOP-0OGAQKFF\SQLEXPRESS;Initial Catalog=DB_Employee;User ID=ja;Password=john");
        public AdminAddEmployee(AdministratorView administratorView)
        {
            InitializeComponent();
            this.administratorView = administratorView;
            // TO DISPLAY THE DATA FROM DATABASE TO YOUR DATA GRID VIEW
            displayEmployeeData();
            dataGridView1.ClearSelection();
        }
        public void displayEmployeeData()
        {
            AdminEmployeesData ed = new AdminEmployeesData();
            List<AdminEmployeesData> listData = ed.employeeListData();

            dataGridView1.DataSource = listData;
            dataGridView1.Columns["Image"].Visible = false;
        }
        public void displayArchiveEmployeeData()
        {
            AdminEmployeesData ed = new AdminEmployeesData();
            List<AdminEmployeesData> listData = ed.ArchiveEmployeeListData();

            dataGridView1.DataSource = listData;
            dataGridView1.Columns["Image"].Visible = false;
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {

            if (e.RowIndex != -1)
            {

                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                addEmployee_id.Text = row.Cells[1].Value.ToString();
                addEmployee_fullName.Text = row.Cells[2].Value.ToString();
                DateTime date;
                if (DateTime.TryParse(row.Cells[3].Value.ToString(), out date))
                {
                    dateTimePickerBirth.Value = date;
                }
                addEmployee_gender.Text = row.Cells[4].Value.ToString();
                addEmployee_phoneNum.Text = row.Cells[5].Value.ToString();
                addEmployee_profession.Text = row.Cells[6].Value.ToString();
                addEmployee_picture.ImageLocation = row.Cells[7].Value.ToString();
                removeErrors();
            }
        }

        public void clearFields()
        {
            addEmployee_id.Text = "";
            addEmployee_fullName.Text = "";
            addEmployee_gender.SelectedIndex = -1;
            addEmployee_phoneNum.Text = "";
            addEmployee_profession.SelectedIndex = -1;
            addEmployee_picture.Image = null;
        }

        private void addEmployee_updateBtn_Click(object sender, EventArgs e)
        {
            if (CheckError())
            {
                MessageBox.Show("Please fill in the blanks and check the warnings"
                    , "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                DialogResult check = MessageBox.Show("Are you sure you want to UPDATE " +
                    "Employee ID: " + addEmployee_id.Text.Trim() + "?", "Confirmation Message"
                    , MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                if (check == DialogResult.Yes)
                {
                    try
                    {
                        connect.Close();
                        connect.Open();
                        DateTime today = DateTime.Today;
                        string updateData = "UPDATE TBL_EMPLOYEE_INFO SET FULL_NAME = @FULL_NAME, DATEOFBIRTH = @DATEOFBIRTH" +
                            ", GENDER = @GENDER, PRIMARY_CONTACT_NUM = @PRIMARY_CONTACT_NUM" +
                            ", PROFESSION = @PROFESSION, DATE_MODIFIED = @DATE_MODIFIED " +
                            "WHERE EMPLOYEE_ID = @EMPLOYEE_ID";

                        string path = Path.Combine(@"C:\Users\John April\Downloads\EmployeeManagementSystem\EmployeeManagementSystem\EmployeeManagementSystem\ImageLocation\"
                                    + addEmployee_id.Text.Trim() + ".jpg");

                        string directoryPath = Path.GetDirectoryName(path);

                        if (!addEmployee_picture.ImageLocation.Equals(path))
                        {
                            if (!Directory.Exists(directoryPath))
                            {
                                Directory.CreateDirectory(directoryPath);
                            }
                            else if (File.Exists(path))
                            {
                                File.Delete(path);
                            }

                            File.Copy(addEmployee_picture.ImageLocation, path, true);
                        }
                        using (SqlCommand cmd = new SqlCommand(updateData, connect))
                        {
                            cmd.Parameters.AddWithValue("@FULL_NAME", addEmployee_fullName.Text.Trim());
                            cmd.Parameters.AddWithValue("@DATEOFBIRTH", dateTimePickerBirth.Value.ToString("yyyy-MM-dd"));
                            cmd.Parameters.AddWithValue("@GENDER", addEmployee_gender.Text.Trim());
                            cmd.Parameters.AddWithValue("@PRIMARY_CONTACT_NUM", addEmployee_phoneNum.Text.Trim());
                            cmd.Parameters.AddWithValue("@PROFESSION", addEmployee_profession.Text.Trim());
                            cmd.Parameters.AddWithValue("@DATE_MODIFIED", today);
                            cmd.Parameters.AddWithValue("@EMPLOYEE_ID", addEmployee_id.Text.Trim());

                            cmd.ExecuteNonQuery();
                            connect.Close();
                            displayEmployeeData();

                            MessageBox.Show("Update successfully!"
                                , "Information Message", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            clearFields();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex
                        , "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Cancelled."
                        , "Information Message", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

            }
        }

        private void addEmployee_clearBtn_Click(object sender, EventArgs e)
        {
            clearFields();
        }

        private void addEmployee_addBtn_Click_1(object sender, EventArgs e)
        {
            if (CheckError())
            {
                MessageBox.Show("Please fill in the blanks and check the warnings"
                    , "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (connect.State == ConnectionState.Closed)
                {
                    try
                    {
                        connect.Open();
                        string checkEmID = "SELECT COUNT(*) FROM TBL_EMPLOYEE_INFO WHERE EMPLOYEE_ID = @EMPLOYEE_ID AND ARCHIVE_DATE IS NULL";

                        using (SqlCommand checkEm = new SqlCommand(checkEmID, connect))
                        {
                            checkEm.Parameters.AddWithValue("@EMPLOYEE_ID", addEmployee_id.Text.Trim());
                            int count = (int)checkEm.ExecuteScalar();

                            if (count >= 1)
                            {
                                MessageBox.Show(addEmployee_id.Text.Trim() + " is already taken"
                                    , "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {
                                DateTime today = DateTime.Today;
                                string insertData = "INSERT INTO TBL_EMPLOYEE_INFO " +
                                    "(EMPLOYEE_ID, FULL_NAME, DATEOFBIRTH, GENDER, PRIMARY_CONTACT_NUM" +
                                    ", PROFESSION, IMAGE, SALARY, DATE_CREATED) " +
                                    "VALUES(@EMPLOYEE_ID, @FULL_NAME, @DATEOFBIRTH, @GENDER, @PRIMARY_CONTACT_NUM" +
                                    ", @PROFESSION, @IMAGE, @SALARY, @DATE_CREATED)";

                                string path = Path.Combine(@"C:\Users\John April\Downloads\EmployeeManagementSystem\EmployeeManagementSystem\EmployeeManagementSystem\ImageLocation\"
                                    + addEmployee_id.Text.Trim() + ".jpg");



                                string directoryPath = Path.GetDirectoryName(path);

                                if (!Directory.Exists(directoryPath))
                                {
                                    Directory.CreateDirectory(directoryPath);
                                }

                                File.Copy(addEmployee_picture.ImageLocation, path, true);

                                using (SqlCommand cmd = new SqlCommand(insertData, connect))
                                {
                                    cmd.Parameters.AddWithValue("@EMPLOYEE_ID", addEmployee_id.Text.Trim());
                                    cmd.Parameters.AddWithValue("@FULL_NAME", addEmployee_fullName.Text.Trim());
                                    cmd.Parameters.AddWithValue("@DATEOFBIRTH", dateTimePickerBirth.Value.ToString("yyyy-MM-dd"));
                                    cmd.Parameters.AddWithValue("@GENDER", addEmployee_gender.SelectedItem.ToString().Trim());
                                    cmd.Parameters.AddWithValue("@PRIMARY_CONTACT_NUM", addEmployee_phoneNum.Text.Trim());
                                    cmd.Parameters.AddWithValue("@PROFESSION", addEmployee_profession.SelectedItem.ToString().Trim());
                                    cmd.Parameters.AddWithValue("@IMAGE", path);
                                    cmd.Parameters.AddWithValue("@SALARY", 0);
                                    cmd.Parameters.AddWithValue("@DATE_CREATED", today);

                                    cmd.ExecuteNonQuery();
                                    connect.Close();
                                    displayEmployeeData();

                                    MessageBox.Show("Added successfully!"
                                        , "Information Message", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                    clearFields();
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex
                    , "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void addEmployee_importBtn_Click_1(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = "Image Files |*.gif;*.jpg;*.jpeg;*.bmp;*.wmf;*.png";
                string imagePath = "";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    imagePath = dialog.FileName;
                    addEmployee_picture.ImageLocation = imagePath;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex, "Error Message"
                    , MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void removeErrors()
        {
            errorProvider.SetError(addEmployee_fullName, null);
            errorProvider.SetError(addEmployee_id, null);
            errorProvider.SetError(addEmployee_gender, null);
            errorProvider.SetError(addEmployee_phoneNum, null);
            errorProvider.SetError(addEmployee_profession, null);
            errorProvider.SetError(addEmployee_picture, null);
        }
        private void enableTextBoxAndButton()
        {
            addEmployee_addBtn.Enabled = true;
            addEmployee_updateBtn.Enabled = true;
            addEmployee_clearBtn.Enabled = true;
            addEmployee_importBtn.Enabled = true;
            addEmployee_id.Enabled = true;
            addEmployee_fullName.Enabled = true;
            addEmployee_gender.Enabled = true;
            addEmployee_phoneNum.Enabled = true;
            addEmployee_profession.Enabled = true;
            dateTimePickerBirth.Enabled = true;
        }
        private void disableTextBoxAndButton()
        {
            addEmployee_addBtn.Enabled = false;
            addEmployee_updateBtn.Enabled = false;
            addEmployee_clearBtn.Enabled = false;
            addEmployee_importBtn.Enabled = false;
            addEmployee_id.Enabled = false;
            addEmployee_fullName.Enabled = false;
            addEmployee_gender.Enabled = false;
            addEmployee_phoneNum.Enabled = false;
            addEmployee_profession.Enabled = false;
            dateTimePickerBirth.Enabled = false;
        }

        private void btnArchiveList_Click(object sender, EventArgs e)
        {
            checkArchive();
        }

        private void addEmployee_archiveBtn_Click(object sender, EventArgs e)
        {
            if (addEmployee_id.Text == ""
                || addEmployee_fullName.Text == ""
                || addEmployee_gender.Text == ""
                || addEmployee_phoneNum.Text == ""
                || addEmployee_profession.Text == ""
                || addEmployee_picture.Image == null)
            {
                MessageBox.Show("Please fill all blank fields"
                    , "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                DialogResult check;
                if (btnArchiveList.Text.Equals("Archive List"))
                {
                    check = MessageBox.Show("Are you sure you want to ARCHIVE " +
                        "Employee ID: " + addEmployee_id.Text.Trim() + "?", "Confirmation Message",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                    if (check == DialogResult.Yes)
                    {
                        try
                        {
                            connect.Open();
                            DateTime today = DateTime.Today;

                            string updateData = "UPDATE TBL_EMPLOYEE_INFO SET ARCHIVE_DATE = @ARCHIVE_DATE, STATUS = @STATUS " +
                                "WHERE EMPLOYEE_ID = @EMPLOYEE_ID";

                            using (SqlCommand cmd = new SqlCommand(updateData, connect))
                            {
                                cmd.Parameters.AddWithValue("@ARCHIVE_DATE", today);
                                cmd.Parameters.AddWithValue("@STATUS", "inactive");
                                cmd.Parameters.AddWithValue("@EMPLOYEE_ID", addEmployee_id.Text.Trim());

                                cmd.ExecuteNonQuery();
                                connect.Close();
                                displayEmployeeData();

                                MessageBox.Show("Employee has been archive successfully!"
                                    , "Information Message", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                clearFields();
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error: " + ex
                            , "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Cancelled."
                            , "Information Message", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    check = MessageBox.Show("Are you sure you want to UNARCHIVE " +
                        "Employee ID: " + addEmployee_id.Text.Trim() + "?", "Confirmation Message",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Information); if (check == DialogResult.Yes)
                    {
                        try
                        {
                            connect.Open();
                            DateTime today = DateTime.Today;

                            string updateData = "UPDATE TBL_EMPLOYEE_INFO SET STATUS = @STATUS " +
                                "WHERE EMPLOYEE_ID = @EMPLOYEE_ID";

                            using (SqlCommand cmd = new SqlCommand(updateData, connect))
                            {
                                cmd.Parameters.AddWithValue("@STATUS", "active");
                                cmd.Parameters.AddWithValue("@EMPLOYEE_ID", addEmployee_id.Text.Trim());

                                cmd.ExecuteNonQuery();
                                connect.Close();
                                this.btnArchiveList_Click(sender, new EventArgs());
                                displayEmployeeData();

                                MessageBox.Show("Employee has been unarchive successfully!"
                                    , "Information Message", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                clearFields();
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error: " + ex
                            , "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Cancelled."
                            , "Information Message", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
        }
        private void addEmployee_phoneNum_Validating(object sender, CancelEventArgs e)
        { 
            string pat1 = @"^(09|\+639)\d{10,}$";
            string pat2 = @"^(09|\+639)\d{0,8}$";
            string pat3 = @"[^0-9+]";
            string pat4 = @"^(09|\+639)\d{9}$";
            Regex regex = new Regex(pat1);
            Regex regex2 = new Regex(pat2);
            Regex regex3 = new Regex(pat3);
            Regex regex4 = new Regex(pat4);
            if (addEmployee_phoneNum.Text.Equals(string.Empty))
            {
                errorProvider.SetError(addEmployee_phoneNum, "Phone Number field is required");
            }
            else if (regex.IsMatch(addEmployee_phoneNum.Text))
            {
                errorProvider.SetError(addEmployee_phoneNum, "Phone Number length has exceeded");
            }
            else if (regex2.IsMatch(addEmployee_phoneNum.Text) || !regex4.IsMatch(addEmployee_phoneNum.Text))
            {
                errorProvider.SetError(addEmployee_phoneNum, "Phone Number has missing number/s");
            }
            else if (regex3.IsMatch(addEmployee_phoneNum.Text))
            {
                errorProvider.SetError(addEmployee_phoneNum, "Phone Number cannot contain alphabet and special characters(except '+')");
            }
            else
            {
                errorProvider.SetError(addEmployee_phoneNum, null); // Clear the error if validation passes
            }
        }
        private void addEmployee_profession_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(addEmployee_profession.Text))
            {
                errorProvider.SetError(addEmployee_profession, "Profession field is required");
            }
            else
            {
                errorProvider.SetError(addEmployee_phoneNum, null); // Clear the error if validation passes
            }
        }

        private void addEmployee_gender_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(addEmployee_gender.Text))
            {
                errorProvider.SetError(addEmployee_gender, "Gender field is required");
            }
            else
            {
                errorProvider.SetError(addEmployee_gender, null); // Clear the error if validation passes
            }
        }

        private void addEmployee_importBtn_Validating(object sender, CancelEventArgs e)
        {
            if (addEmployee_picture.Image == null)
            {
                errorProvider.SetError(addEmployee_picture, "Please insert a photo");
            }
            else
            {
                errorProvider.SetError(addEmployee_picture, null); // Clear the error if validation passes
            }
        }

        private void addEmployee_fullName_Validating(object sender, CancelEventArgs e)
        {
            string pattern = @"[^A-Za-z'-.\s]";
            Regex regex = new Regex(pattern);
            if (addEmployee_fullName.Text.Equals(string.Empty))
            {
                errorProvider.SetError(addEmployee_fullName, "Full Name field is required");
            }
            else if (regex.IsMatch(addEmployee_fullName.Text) || addEmployee_fullName.Text.Equals(string.Empty))
            {
                errorProvider.SetError(addEmployee_fullName, "Invalid Full Name");
            }
            else
            {
                errorProvider.SetError(addEmployee_fullName, null); // Clear the error if validation passes
            }
        }

        private void addEmployee_id_Validating(object sender, CancelEventArgs e)
        {
            string pat1 = @"\D";
            string pat2 = @"^\d{0,10}$";
            Regex regex1 = new Regex(pat1);
            Regex regex2 = new Regex(pat2);
            if (regex1.IsMatch(addEmployee_id.Text))
            {
                errorProvider.SetError(addEmployee_id, "Employee ID cannot contain alphabet and special characters");
            }
            else if (addEmployee_id.Text.Equals(string.Empty))
            {
                errorProvider.SetError(addEmployee_id, "Employee ID field is required");
            }
            else if (regex2.IsMatch(addEmployee_id.Text))
            {
                errorProvider.SetError(addEmployee_id, "Employee ID has missing number/s");
            }
            else
            {
                errorProvider.SetError(addEmployee_id, null); // Clear the error if validation passes
            }
        }
        private bool CheckError()
        {
            this.ValidateChildren();

            bool empIdError = !string.IsNullOrWhiteSpace(errorProvider.GetError(addEmployee_id));
            bool fullNameError = !string.IsNullOrWhiteSpace(errorProvider.GetError(addEmployee_fullName));
            bool genderError = !string.IsNullOrWhiteSpace(errorProvider.GetError(addEmployee_gender));
            bool phoneNumError = !string.IsNullOrWhiteSpace(errorProvider.GetError(addEmployee_phoneNum));
            bool professionError = !string.IsNullOrWhiteSpace(errorProvider.GetError(addEmployee_profession));
            bool pictureError = !string.IsNullOrWhiteSpace(errorProvider.GetError(addEmployee_picture));
            if (empIdError || fullNameError || genderError || phoneNumError || professionError || pictureError)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void dataGridView1_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                administratorView.changeCurrentPanel(new AdminEmployeeProfile(administratorView, row.Cells[1].Value.ToString()));
            }
        }
        public void checkArchive()
        {
            if (btnArchiveList.Text.Equals("Active List"))
            {
                btnArchiveList.Text = "Archive List";
                addEmployee_archiveBtn.Text = "Archive";
                displayEmployeeData();
                enableTextBoxAndButton();
            }
            else
            {
                btnArchiveList.Text = "Active List";
                addEmployee_archiveBtn.Text = "Unarchive";
                displayArchiveEmployeeData();
                disableTextBoxAndButton();
                clearFields();
                removeErrors();
            }
        }
    }
}

