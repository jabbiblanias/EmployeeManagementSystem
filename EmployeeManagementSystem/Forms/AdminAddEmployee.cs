using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using EmployeeManagementSystem.Services;
using EmployeeManagementSystem.Models;

namespace EmployeeManagementSystem
{
    public partial class AdminAddEmployee : UserControl
    {
        private readonly EmployeeService _employeeService;
        AdministratorView administratorView;
        public AdminAddEmployee(AdministratorView administratorView)
        {
            InitializeComponent();
            this.administratorView = administratorView;
            _employeeService = new EmployeeService();
            displayEmployeeData();
            dataGridView1.ClearSelection();
        }
        public void displayEmployeeData()
        {
            List<Employee> listData = _employeeService.GetEmployees();

            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.Columns.Clear();

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ID",
                HeaderText = "ID",
                DataPropertyName = "ID"
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "EmployeeID",
                HeaderText = "Employee ID",
                DataPropertyName = "EmployeeID"
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Name",
                HeaderText = "Name",
                DataPropertyName = "Name"
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "DateofBirth",
                HeaderText = "Date of Birth",
                DataPropertyName = "DateofBirth"
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Gender",
                HeaderText = "Gender",
                DataPropertyName = "Gender"
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "PrimaryContactNum",
                HeaderText = "Contact",
                DataPropertyName = "Contact"
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Profession",
                HeaderText = "Profession",
                DataPropertyName = "Profession"
            });

            dataGridView1.DataSource = listData;
        }
        public void displayArchiveEmployeeData()
        {
            List<Employee> listData = _employeeService.GetArchivedEmployees();

            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.Columns.Clear();

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ID",
                HeaderText = "ID",
                DataPropertyName = "ID"
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "EmployeeID",
                HeaderText = "Employee ID",
                DataPropertyName = "EmployeeID"
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Name",
                HeaderText = "Name",
                DataPropertyName = "Name"
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "DateofBirth",
                HeaderText = "Date of Birth",
                DataPropertyName = "DateofBirth"
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Gender",
                HeaderText = "Gender",
                DataPropertyName = "Gender"
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "PrimaryContactNum",
                HeaderText = "Contact",
                DataPropertyName = "Contact"
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Profession",
                HeaderText = "Profession",
                DataPropertyName = "Profession"
            });

            dataGridView1.DataSource = listData;
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

            addEmployee_id.Text = row.Cells["EmployeeID"].Value?.ToString();
            addEmployee_fullName.Text = row.Cells["Name"].Value?.ToString();

            if (DateTime.TryParse(row.Cells["DateofBirth"].Value?.ToString(), out DateTime date))
            {
                dateTimePickerBirth.Value = date;
            }

            addEmployee_gender.Text = row.Cells["Gender"].Value?.ToString();
            addEmployee_phoneNum.Text = row.Cells["PrimaryContactNum"].Value?.ToString();
            addEmployee_profession.Text = row.Cells["Profession"].Value?.ToString();

            Employee selectedEmployee = row.DataBoundItem as Employee;

            if (selectedEmployee != null &&
                !string.IsNullOrWhiteSpace(selectedEmployee.Image) &&
                System.IO.File.Exists(selectedEmployee.Image))
            {
                addEmployee_picture.ImageLocation = selectedEmployee.Image;
            }
            else
            {
                addEmployee_picture.ImageLocation = null;
                addEmployee_picture.Image = null;
            }

            removeErrors();
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
                MessageBox.Show(
                    "Please fill in the blanks and check the warnings",
                    "Error Message",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return;
            }

            string employeeId = addEmployee_id.Text.Trim();

            DialogResult check = MessageBox.Show(
                "Are you sure you want to UPDATE Employee ID: " + employeeId + "?",
                "Confirmation Message",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Information);

            if (check != DialogResult.Yes)
            {
                MessageBox.Show(
                    "Cancelled.",
                    "Information Message",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            try
            {
                // Prepare the image destination.
                string directoryPath = Path.Combine(
                    Application.StartupPath,
                    "ImageLocation");

                Directory.CreateDirectory(directoryPath);

                string imagePath = Path.Combine(
                    directoryPath,
                    employeeId + ".jpg");

                string selectedImagePath = addEmployee_picture.ImageLocation;

                // Avoid copying the image onto itself.
                if (!string.IsNullOrWhiteSpace(selectedImagePath))
                {
                    string sourcePath = Path.GetFullPath(selectedImagePath);
                    string destinationPath = Path.GetFullPath(imagePath);

                    if (!sourcePath.Equals(
                        destinationPath,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        File.Copy(sourcePath, destinationPath, true);
                    }
                }

                // Prepare employee data.
                Employee employee = new Employee
                {
                    EmployeeID = employeeId,
                    FullName = addEmployee_fullName.Text.Trim(),
                    DateofBirth = dateTimePickerBirth.Value.Date,
                    Gender = addEmployee_gender.Text.Trim(),
                    PrimaryContactNum = addEmployee_phoneNum.Text.Trim(),
                    Profession = addEmployee_profession.Text.Trim(),
                    Image = imagePath,
                    DateModified = DateTime.Today
                };

                // Call the service.
                _employeeService.UpdateEmployee(employee);

                displayEmployeeData();

                MessageBox.Show(
                    "Updated successfully!",
                    "Information Message",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                clearFields();
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Error Message",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error: " + ex.Message,
                    "Error Message",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
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
                MessageBox.Show(
                    "Please fill in the blanks and check the warnings",
                    "Error Message",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return;
            }

            try
            {
                string employeeId = addEmployee_id.Text.Trim();

                string directoryPath = Path.Combine(
                    Application.StartupPath,
                    "ImageLocation");

                Directory.CreateDirectory(directoryPath);

                string imagePath = Path.Combine(
                    directoryPath,
                    employeeId + ".jpg");

                File.Copy(addEmployee_picture.ImageLocation, imagePath, true);

                Employee employee = new Employee
                {
                    EmployeeID = employeeId,
                    FullName = addEmployee_fullName.Text.Trim(),
                    DateofBirth = dateTimePickerBirth.Value.Date,
                    Gender = addEmployee_gender.SelectedItem.ToString().Trim(),
                    PrimaryContactNum = addEmployee_phoneNum.Text.Trim(),
                    Profession = addEmployee_profession.SelectedItem.ToString().Trim(),
                    Image = imagePath,
                    Salary = 0,
                    DateCreated = DateTime.Today
                };

                _employeeService.AddEmployee(employee);

                displayEmployeeData();

                MessageBox.Show(
                    "Added successfully!",
                    "Information Message",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                clearFields();
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Error Message",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error: " + ex.Message,
                    "Error Message",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
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
            string employeeId = addEmployee_id.Text.Trim();

            // Only the Employee ID is required for archiving/unarchiving.
            if (string.IsNullOrWhiteSpace(employeeId))
            {
                MessageBox.Show(
                    "Please select an employee first.",
                    "Error Message",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return;
            }

            bool isArchiving = btnArchiveList.Text.Equals("Archive List");

            string action = isArchiving ? "ARCHIVE" : "UNARCHIVE";

            DialogResult check = MessageBox.Show(
                $"Are you sure you want to {action} Employee ID: {employeeId}?",
                "Confirmation Message",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Information);

            if (check != DialogResult.Yes)
            {
                MessageBox.Show(
                    "Cancelled.",
                    "Information Message",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            try
            {
                _employeeService.SetEmployeeArchiveStatus(
                    employeeId,
                    isArchiving);

                if (isArchiving)
                {
                    displayEmployeeData();
                }
                else
                {
                    // Return to the active employee list.
                    btnArchiveList_Click(sender, EventArgs.Empty);
                    displayEmployeeData();
                }

                MessageBox.Show(
                    isArchiving
                        ? "Employee has been archived successfully!"
                        : "Employee has been unarchived successfully!",
                    "Information Message",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                clearFields();
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Error Message",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error: " + ex.Message,
                    "Error Message",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
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

