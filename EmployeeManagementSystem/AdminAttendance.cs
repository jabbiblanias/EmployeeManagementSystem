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

namespace EmployeeManagementSystem
{
    public partial class AdminAttendance : UserControl
    {
        private bool lastAction = false;
        public AdminAttendance()
        {
            InitializeComponent();
            AdminAttendanceData ad = new AdminAttendanceData();
            for (int i = 1; i <= ad.recentNumberOfEntries(); i++)
            {
                comboBoxEntries.Items.Add(i);
            }
            comboBoxEntries.SelectedIndex = 0;
            displayEmployeesAttendaceListData();
            employeeNameSearchData();
        }
        public void displayEmployeesAttendaceListData()
        {
            AdminAttendanceData ad = new AdminAttendanceData();
            List<AdminAttendanceData> listData = ad.employeesAttendanceListData(Convert.ToInt32(comboBoxEntries.SelectedItem.ToString()));

            dataGridView1.DataSource = listData;

            lastAction = false;
        }
        private void displaySearchData()
        {
            AdminAttendanceData ad = new AdminAttendanceData();
            List<AdminAttendanceData> listData = ad.SearchEmployeeAttendanceListData(Convert.ToInt32(comboBoxEntries.SelectedItem.ToString()), textBoxSearchName.Text.Trim());

            dataGridView1.DataSource = listData;
            lastAction = true;

        }
        private void buttonSearch_Click(object sender, EventArgs e)
        {
            displaySearchDataEntry();
        }

        private void comboBoxEntries_SelectedValueChanged(object sender, EventArgs e)
        {
            if (lastAction)
            {
                displaySearchData();
            }
            else
            {
                displayEmployeesAttendaceListData();
            }
        }
        private void employeeNameSearchData()
        {
            AdminAttendanceData ad = new AdminAttendanceData();

            // Set the AutoCompleteCustomSource property to the AutoCompleteStringCollection
            textBoxSearchName.AutoCompleteCustomSource = ad.employeeNamesData();
        }
        private void btnRecent_Click(object sender, EventArgs e)
        {
            displayEmployeesAttendaceListData();
            AdminAttendanceData ad = new AdminAttendanceData();
            comboBoxEntries.Items.Clear();
            for (int i = 1; i <= ad.recentNumberOfEntries(); i++)
            {
                comboBoxEntries.Items.Add(i);
            }
            comboBoxEntries.SelectedIndex = 0;
            
        }
        private void displaySearchDataEntry()
        {
            AdminAttendanceData ad = new AdminAttendanceData();
            displaySearchData();
            comboBoxEntries.Items.Clear();
            for (int i = 1; i <= ad.searchNumberOfEntries(textBoxSearchName.Text); i++)
            {
                comboBoxEntries.Items.Add(i);
            }
            comboBoxEntries.SelectedIndex = 0;
        }
    }
}
