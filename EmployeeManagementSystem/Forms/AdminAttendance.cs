using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml.Linq;
using EmployeeManagementSystem.Repositories;
using EmployeeManagementSystem.Models;
using EmployeeManagementSystem.Services;

namespace EmployeeManagementSystem
{
    public partial class AdminAttendance : UserControl
    {
        private readonly AttendanceService _attendanceService;
        private bool dispLastAction = false;
        private bool searchLastAction = false;
        private bool filterLastAction = false;
        public AdminAttendance()
        {
            _attendanceService = new AttendanceService();
            InitializeComponent();
            startUp();
        }
        private void startUp()
        {
            LimitDate();
            entries(_attendanceService.GetRecentNumberOfEntries());
            comboBoxProfession.SelectedIndex = 0;
            comboBoxStatus.SelectedIndex = 0;
            displayEmployeesAttendaceListData();
            employeeNameSearchData();
        }
        private void LimitDate()
        {
            DateTime today = DateTime.Today;

            dateTimePickerFrom.Value = today;
            dateTimePickerFrom.MaxDate = today;

            dateTimePickerTo.Value = today;
            dateTimePickerTo.MaxDate = today;
        }
        public void displayEmployeesAttendaceListData()
        {
            List<Attendance> listData = _attendanceService.GetAllEmployeesAttendance(Convert.ToInt32(comboBoxEntries.SelectedItem.ToString()));

            CustomTable(listData);
            dispLastAction = true;
            searchLastAction = false;
            filterLastAction = false;
        }
        private void employeeNameSearchData()
        {
            // Set the AutoCompleteCustomSource property to the AutoCompleteStringCollection
            textBoxSearchName.AutoCompleteCustomSource = _attendanceService.GetEmployeeNames();
        }
        private void buttonSearch_Click(object sender, EventArgs e)
        {
            displaySearchDataEntry();
        }
        private void displaySearchDataEntry()
        {
            EmployeeRepository ad = new EmployeeRepository();
            displaySearchData();
            entries(_attendanceService.GetSearchNumberOfEntries(textBoxSearchName.Text));
        }
        private void displaySearchData()
        {
            List<Attendance> listData = _attendanceService.SearchEmployeeAttendance(Convert.ToInt32(comboBoxEntries.SelectedItem.ToString()), textBoxSearchName.Text.Trim());

            CustomTable(listData);
            searchLastAction = true;
            dispLastAction = false;
            filterLastAction = false;
        }
        private void comboBoxEntries_SelectedValueChanged(object sender, EventArgs e)
        {
            if (searchLastAction)
            {
                displaySearchData();
            }
            else if(dispLastAction)
            {
                displayEmployeesAttendaceListData();
            }
            else if (filterLastAction)
            {
                filterData();
            }
        }
        private void btnRecent_Click(object sender, EventArgs e)
        {
            displayEmployeesAttendaceListData();
            entries(_attendanceService.GetRecentNumberOfEntries());
        }
        private void buttonApply_Click(object sender, EventArgs e)
        {
            if(dateTimePickerFrom.Value <= dateTimePickerTo.Value)
            {
                filterData();
                displayFilterDataEntry();
            }
            else
            {
                MessageBox.Show("The date is not applicable.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void filterData()
        {
            EmployeeRepository ad = new EmployeeRepository();
            List<Attendance> listData = _attendanceService.GetFilterEmployeesAttendance(Convert.ToInt32(comboBoxEntries.SelectedItem.ToString()), comboBoxProfession.SelectedItem.ToString().Trim(), comboBoxStatus.SelectedItem.ToString().Trim(), dateTimePickerFrom.Value, dateTimePickerTo.Value);
            if(listData != null)
            {
                CustomTable(listData);
                filterLastAction = true;
                dispLastAction = false;
                searchLastAction = false;
            }
            else
            {
                MessageBox.Show("There is no data listed", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }
        private void displayFilterDataEntry()
        {
            entries(_attendanceService.GetFilterNumberOfEntries(comboBoxProfession.SelectedItem.ToString(), comboBoxStatus.SelectedItem.ToString(), dateTimePickerFrom.Value, dateTimePickerTo.Value));
        }
        private void entries(int entry)
        {
            if(entry == 0)
            {
                entry = 1;
            }
            comboBoxEntries.Items.Clear();
            for (int i = 1; i <= entry; i++)
            {
                comboBoxEntries.Items.Add(i);
            }
            comboBoxEntries.SelectedIndex = 0;
        }

        private void CustomTable(List<Attendance> listData)
        {
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.Columns.Clear();

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Date",
                HeaderText = "Date"
            });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "ClockIn",
                HeaderText = "Clock In"
            });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "ClockOut",
                HeaderText = "Clock Out"
            });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Status",
                HeaderText = "Status"
            });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Overtime",
                HeaderText = "Overtime"
            });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "EmployeeID",
                HeaderText = "Employee ID"
            });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Name",
                HeaderText = "Name"
            });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Profession",
                HeaderText = "Profession"
            });

            dataGridView1.DataSource = listData;
        }
    }
}
