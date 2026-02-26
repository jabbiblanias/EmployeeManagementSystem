using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;

namespace EmployeeManagementSystem
{
    public partial class AdminAttendance : UserControl
    {
        private bool dispLastAction = false;
        private bool searchLastAction = false;
        private bool filterLastAction = false;
        public AdminAttendance()
        {
            InitializeComponent();
            startUp();
        }
        private void startUp()
        {
            LimitDate();
            AdminAttendanceData ad = new AdminAttendanceData();
            entries(ad.recentNumberOfEntries());
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
            AdminAttendanceData ad = new AdminAttendanceData();
            List<AdminAttendanceData> listData = ad.employeesAttendanceListData(Convert.ToInt32(comboBoxEntries.SelectedItem.ToString()));

            dataGridView1.DataSource = listData;
            dispLastAction = true;
            searchLastAction = false;
            filterLastAction = false;
        }
        private void employeeNameSearchData()
        {
            AdminAttendanceData ad = new AdminAttendanceData();

            // Set the AutoCompleteCustomSource property to the AutoCompleteStringCollection
            textBoxSearchName.AutoCompleteCustomSource = ad.employeeNamesData();
        }
        private void buttonSearch_Click(object sender, EventArgs e)
        {
            displaySearchDataEntry();
        }
        private void displaySearchDataEntry()
        {
            AdminAttendanceData ad = new AdminAttendanceData();
            displaySearchData();
            entries(ad.searchNumberOfEntries(textBoxSearchName.Text));
        }
        private void displaySearchData()
        {
            AdminAttendanceData ad = new AdminAttendanceData();
            List<AdminAttendanceData> listData = ad.SearchEmployeeAttendanceListData(Convert.ToInt32(comboBoxEntries.SelectedItem.ToString()), textBoxSearchName.Text.Trim());

            dataGridView1.DataSource = listData;
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
            AdminAttendanceData ad = new AdminAttendanceData();
            entries(ad.recentNumberOfEntries());
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
            AdminAttendanceData ad = new AdminAttendanceData();
            List<AdminAttendanceData> listData = ad.filterEmployeesAttendanceListData(Convert.ToInt32(comboBoxEntries.SelectedItem.ToString()), comboBoxProfession.SelectedItem.ToString().Trim(), comboBoxStatus.SelectedItem.ToString().Trim(), dateTimePickerFrom.Value, dateTimePickerTo.Value);
            if(listData != null)
            {
                dataGridView1.DataSource = listData;
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
            AdminAttendanceData ad = new AdminAttendanceData();
            entries(ad.filterNumberOfEntries(comboBoxProfession.SelectedItem.ToString(), comboBoxStatus.SelectedItem.ToString(), dateTimePickerFrom.Value, dateTimePickerTo.Value));
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
    }
}
