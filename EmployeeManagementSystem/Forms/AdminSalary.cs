using EmployeeManagementSystem.Services;
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

namespace EmployeeManagementSystem
{
    public partial class AdminSalary : UserControl
    {
        private readonly EmployeeService _employeeService;
        public AdminSalary()
        {
            InitializeComponent();

            _employeeService = new EmployeeService();
            displayEmployees();
            disableFields();
        }

        public void RefreshData()
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)RefreshData);
                return;
            }

            displayEmployees();
            disableFields();
        }

        public void disableFields()
        {
            salary_employeeID.Enabled = false;
            salary_name.Enabled = false;
            salary_profession.Enabled = false;
        }

        public void displayEmployees()
        {
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.Columns.Clear();
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "EmployeeID", HeaderText = "Employee ID" });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Name", HeaderText = "Name" });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Profession", HeaderText = "Profession" });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Salary", HeaderText = "Salary" });
            dataGridView1.DataSource = _employeeService.GetEmployeesForSalary();
        }

        private void salary_updateBtn_Click(object sender, EventArgs e)
        {
            if (salary_employeeID.Text == ""
                || salary_name.Text == ""
                || salary_profession.Text == ""
                || salary_salary.Text == "")
            {
                MessageBox.Show("Please fill all blank fields", "Error Message"
                    , MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                DialogResult check = MessageBox.Show("Are you sure you want to UPDATE Employee ID: "
                    + salary_employeeID.Text.Trim() + "?", "Confirmation Message"
                    , MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (check == DialogResult.Yes)
                {
                    _employeeService.UpdateSalary(salary_employeeID.Text.Trim(), Convert.ToInt32(salary_salary.Text.Trim()));
                    displayEmployees();

                    MessageBox.Show("Updated successfully!"
                        , "Information Message", MessageBoxButtons.OK
                        , MessageBoxIcon.Information);

                    clearFields();
                }
                else
                {
                    MessageBox.Show("Cancelled", "Information Message"
                    , MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        public void clearFields()
        {
            salary_employeeID.Text = "";
            salary_name.Text = "";
            salary_profession.Text = "";
            salary_salary.Text = "";
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                salary_employeeID.Text = row.Cells[0].Value.ToString();
                salary_name.Text = row.Cells[1].Value.ToString();
                salary_profession.Text = row.Cells[2].Value.ToString();
                salary_salary.Text = row.Cells[3].Value.ToString();
            }
        }

        private void salary_clearBtn_Click(object sender, EventArgs e)
        {
            clearFields();
        }
    }
}
