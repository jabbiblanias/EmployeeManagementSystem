using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;

namespace EmployeeManagementSystem
{
    public partial class EventCalendar : UserControl
    {
        LoginView loginView;
        DateTime today = DateTime.Now;
        private int currentMonth, currentYear, realMonth, realYear;
        private string selectedDay;
        private string[] days = { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };
        private string[] months = { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"};

        SqlConnection connect =
new SqlConnection(@"Data Source=LAPTOP-0OGAQKFF\SQLEXPRESS;Initial Catalog=DB_Employee;User ID=ja;Password=john");
        public string WordDate {  get; set; }
        public string Date {  get; set; }
        public EventCalendar(LoginView loginView)
        {
            InitializeComponent();
            this.loginView = loginView;
            for(int i = 1900; i <= 2050; i++)
            {
                comboBoxYear.Items.Add(i);
            }
            
            Calendar();
            DisplayEvents();
        }
        public void Calendar()
        {
            DateTime currentDate = DateTime.Now;

            realYear = currentDate.Year;
            realMonth = currentDate.Month;
            currentMonth = realMonth;
            currentYear = realYear;

            foreach (string header in days)
            {
                this.dataGridView1.Columns.Add(header, header);

            }
            // Disable sorting for all columns.
            foreach (DataGridViewColumn column in this.dataGridView1.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            RefreshCalendar(realYear, realMonth);
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            // Ensure the click is not on the header row.
            if (e.RowIndex >= 0)
            {
                // Get the selected cell's value.
                object selectedValue = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;

                // Ensure the selected value is not null.
                if (selectedValue != null)
                {
                    selectedDay = selectedValue.ToString();
                    WordDate = $"{months[currentMonth - 1]} {selectedDay}, {currentYear}";
                    Date = $"{currentYear}-{currentMonth}-{selectedDay}";
                }
                else
                {
                    // Handle the case where the cell value is null.
                    MessageBox.Show("The selected cell value is empty.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            EventForm eventForm = new EventForm(this, loginView);
            eventForm.Show();
        }

        private void comboBoxYear_SelectedValueChanged(object sender, EventArgs e)
        {
            currentYear = Convert.ToInt32(comboBoxYear.SelectedItem.ToString());
            RefreshCalendar(currentYear, currentMonth);
        }

        public void RefreshCalendar(int year, int month)
        {
            dataGridView1.Rows.Clear();
            dataGridView1.RowCount = 6;
            lblMonth.Text = months[month-1];
            comboBoxYear.SelectedItem = year;
            DateTime firstDayOfMonth = new DateTime(year, month, 1);
            int daysInMonth = DateTime.DaysInMonth(year, month);

            // Get the first day of the week (0 = Sunday, 1 = Monday, ..., 6 = Saturday)
            int firstDayOfWeek = (int)firstDayOfMonth.DayOfWeek;

            int currentRow = 0;
            int currentColumn = firstDayOfWeek;


            for (int day = 1; day <= daysInMonth; day++)
            {
                dataGridView1[currentColumn, currentRow].Value = day;
                currentColumn++;
                if (currentColumn > 6)
                {
                    currentColumn = 0;
                    currentRow++;
                }
            }
            dataGridView1.ClearSelection();
            CalendarHighlights(year,month);
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                // Get the selected cell's value.
                object selectedValue = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;

                // Ensure the selected value is not null.
                if (selectedValue != null)
                {
                    selectedDay = selectedValue.ToString();
                    WordDate = $"{months[currentMonth - 1]} {selectedDay}, {currentYear}";
                    Date = $"{currentYear}-{currentMonth}-{selectedDay}";
                }
                DisplayEvents();
            }
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            if (currentMonth == 1)
            { //Back one year
                currentMonth = 12;
                currentYear -= 1;
            }
            else
            { //Back one month
                currentMonth -= 1;
            }
            RefreshCalendar(currentYear, currentMonth);
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (currentMonth == 12)
            { //Foward one year
                currentMonth = 1;
                currentYear += 1;
            }
            else
            { //Foward one month
                currentMonth += 1;
            }
            RefreshCalendar(currentYear, currentMonth);
        }
        public void DisplayEvents()
        {
            try
            {
                listBox1.Items.Clear();
                connect.Open();
                string selectData = "SELECT * FROM TBL_EVENTS INNER JOIN TBL_USERS ON TBL_EVENTS.ACCOUNT_ID = TBL_USERS.ACCOUNT_ID WHERE TBL_EVENTS.EVENT_DATE = @EVENT_DATE AND (TBL_EVENTS.ACCOUNT_ID = @ACCOUNT_ID OR TBL_USERS.ROLE = 'ADMINISTRATOR')";
                using (SqlCommand cmd = new SqlCommand(selectData, connect))
                {
                    cmd.Parameters.AddWithValue("@ACCOUNT_ID", loginView.ID);
                    cmd.Parameters.AddWithValue("@EVENT_DATE", Date);
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {

                        while (reader.Read())
                        {
                            listBox1.Items.Add("Title: " + reader["TITLE"].ToString());
                            listBox1.Items.Add($"Time: {DateTime.Today.Add(reader.GetTimeSpan(reader.GetOrdinal("START_TIME"))).ToString("hh:mm tt")} - {DateTime.Today.Add(reader.GetTimeSpan(reader.GetOrdinal("END_TIME"))).ToString("hh:mm tt")}");
                            listBox1.Items.Add("Description: " + reader["DESCRIPTION"].ToString());
                            listBox1.Items.Add("Category: " + reader["CATEGORY"].ToString());
                            listBox1.Items.Add("Location: " + reader["LOCATION"].ToString());
                            listBox1.Items.Add("");
                        }
                        listBox1.Items.RemoveAt(listBox1.Items.Count - 1);
                    }
                    else
                    {
                        listBox1.Items.Add("*No events on this day*");
                    }
                    
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex
                    , "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally { 
                connect.Close();
            }
        }
        private void CalendarHighlights(int year, int month)
        {
            //Events
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    if(cell.Value != null)
                    {
                        try
                        {
                            connect.Open();
                            string selectData = "SELECT DISTINCT CATEGORY FROM TBL_EVENTS INNER JOIN TBL_USERS ON TBL_EVENTS.ACCOUNT_ID = TBL_USERS.ACCOUNT_ID WHERE TBL_EVENTS.EVENT_DATE = @EVENT_DATE AND (TBL_EVENTS.ACCOUNT_ID = @ACCOUNT_ID OR TBL_USERS.ROLE = 'ADMINISTRATOR')";
                            using (SqlCommand cmd = new SqlCommand(selectData, connect))
                            {
                                cmd.Parameters.AddWithValue("@ACCOUNT_ID", loginView.ID);
                                cmd.Parameters.AddWithValue("@EVENT_DATE", $"{year}-{month}-{cell.Value.ToString()}");
                                using (SqlDataReader reader = cmd.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        switch (reader.GetString(0).Trim())
                                        {
                                            case "Personal":
                                                cell.Style.BackColor = Color.MediumOrchid;
                                                cell.Style.ForeColor = Color.White;
                                                break;
                                            case "Company Event":
                                                cell.Style.BackColor = Color.Green;
                                                cell.Style.ForeColor = Color.White;
                                                break;
                                            case "Working Holiday":
                                                cell.Style.BackColor = Color.Orange;
                                                cell.Style.ForeColor = Color.White;
                                                break;
                                            case "Non-Working Holiday":
                                                cell.Style.BackColor = Color.Purple;
                                                cell.Style.ForeColor = Color.White;
                                                break;
                                            case "Company No Working Day":
                                                cell.Style.BackColor = Color.Red;
                                                cell.Style.ForeColor = Color.White;
                                                break;
                                        }
                                    }

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
                }
            }

            //Today
            if (currentMonth == realMonth && currentYear == realYear)
            {

                int currentDay = today.Day;

                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    foreach (DataGridViewCell cell in row.Cells)
                    {
                        if (cell.Value != null && (int)cell.Value == currentDay)
                        {
                            selectedDay = currentDay.ToString();
                            Date = $"{currentYear}-{currentMonth}-{selectedDay}";
                            // Set the background color of the cell to highlight it
                            cell.Style.BackColor = Color.LightBlue;
                        }
                    }
                }
            }

        }
    }
}
