using EmployeeManagementSystem.Models;
using EmployeeManagementSystem.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace EmployeeManagementSystem
{
    public partial class EventCalendar : UserControl
    {
        private readonly User _currentUser;
        private readonly EventService _eventService;

        private DateTime _selectedDate = DateTime.Today;

        private int currentMonth;
        private int currentYear;

        private readonly string[] days =
        {
            "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"
        };

        private readonly string[] months =
        {
            "January", "February", "March", "April",
            "May", "June", "July", "August",
            "September", "October", "November", "December"
        };

        public string WordDate { get; set; }
        public string Date { get; set; }

        public EventCalendar(User user)
        {
            InitializeComponent();

            _currentUser = user;
            _eventService = new EventService();

            currentMonth = DateTime.Today.Month;
            currentYear = DateTime.Today.Year;

            _selectedDate = DateTime.Today;
            UpdateSelectedDate();

            for (int year = 1900; year <= 2050; year++)
            {
                comboBoxYear.Items.Add(year);
            }

            Calendar();
            DisplayEvents();
        }

        private void UpdateSelectedDate()
        {
            WordDate = _selectedDate.ToString("MMMM d, yyyy");
            Date = _selectedDate.ToString("yyyy-MM-dd");
        }

        public void Calendar()
        {
            dataGridView1.Columns.Clear();

            foreach (string header in days)
            {
                dataGridView1.Columns.Add(header, header);
            }

            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }

            RefreshCalendar(currentYear, currentMonth);
        }

        public void RefreshCalendar(int year, int month)
        {
            currentYear = year;
            currentMonth = month;

            dataGridView1.Rows.Clear();
            dataGridView1.RowCount = 6;

            lblMonth.Text = months[month - 1];

            if (comboBoxYear.SelectedItem == null ||
                Convert.ToInt32(comboBoxYear.SelectedItem) != year)
            {
                comboBoxYear.SelectedItem = year;
            }

            DateTime firstDayOfMonth = new DateTime(year, month, 1);

            int daysInMonth = DateTime.DaysInMonth(year, month);
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

            CalendarHighlights(year, month);
        }

        private void SelectDateFromCell(DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            object selectedValue =
                dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;

            if (selectedValue == null)
                return;

            int day = Convert.ToInt32(selectedValue);

            _selectedDate = new DateTime(currentYear, currentMonth, day);

            UpdateSelectedDate();
        }

        private void dataGridView1_CellClick(
            object sender,
            DataGridViewCellEventArgs e)
        {
            SelectDateFromCell(e);

            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                DisplayEvents();
            }
        }

        private void dataGridView1_CellDoubleClick(
            object sender,
            DataGridViewCellEventArgs e)
        {
            SelectDateFromCell(e);

            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            object selectedValue =
                dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;

            if (selectedValue == null)
                return;

            EventForm eventForm = new EventForm(this, _currentUser);
            eventForm.Show();
        }

        private void comboBoxYear_SelectedValueChanged(
            object sender,
            EventArgs e)
        {
            if (comboBoxYear.SelectedItem == null)
                return;

            currentYear = Convert.ToInt32(comboBoxYear.SelectedItem);

            RefreshCalendar(currentYear, currentMonth);
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            if (currentMonth == 1)
            {
                currentMonth = 12;
                currentYear--;
            }
            else
            {
                currentMonth--;
            }

            RefreshCalendar(currentYear, currentMonth);
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (currentMonth == 12)
            {
                currentMonth = 1;
                currentYear++;
            }
            else
            {
                currentMonth++;
            }

            RefreshCalendar(currentYear, currentMonth);
        }

        public void DisplayEvents()
        {
            try
            {
                listBox1.Items.Clear();

                List<Event> events =
                    _eventService.GetEventsForDate(
                        _selectedDate,
                        _currentUser.ID
                    );

                if (events.Count == 0)
                {
                    listBox1.Items.Add("*No events on this day*");
                    return;
                }

                foreach (Event item in events)
                {
                    string startTime = DateTime.Today
                        .Add(item.StartTime)
                        .ToString("hh:mm tt");

                    string endTime = DateTime.Today
                        .Add(item.EndTime)
                        .ToString("hh:mm tt");

                    listBox1.Items.Add("Title: " + item.Title);
                    listBox1.Items.Add(
                        $"Time: {startTime} - {endTime}"
                    );
                    listBox1.Items.Add(
                        "Description: " + item.Description
                    );
                    listBox1.Items.Add(
                        "Category: " + item.Category
                    );
                    listBox1.Items.Add(
                        "Location: " + item.Location
                    );
                    listBox1.Items.Add("");
                }

                // Remove the last empty line.
                if (listBox1.Items.Count > 0)
                {
                    listBox1.Items.RemoveAt(listBox1.Items.Count - 1);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to display events: " + ex.Message,
                    "Error Message",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void CalendarHighlights(int year, int month)
        {
            try
            {
                DateTime startDate = new DateTime(year, month, 1);
                DateTime endDate = startDate.AddMonths(1);

                List<Event> events =
                    _eventService.GetEventsForMonth(
                        startDate,
                        endDate,
                        _currentUser.ID
                    );

                // Apply category colors to calendar dates.
                foreach (Event item in events)
                {
                    int day = item.EventDate.Day;

                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        foreach (DataGridViewCell cell in row.Cells)
                        {
                            if (cell.Value != null &&
                                Convert.ToInt32(cell.Value) == day)
                            {
                                ApplyCategoryColor(cell, item.Category);
                            }
                        }
                    }
                }

                // Highlight today's date.
                if (year == DateTime.Today.Year &&
                    month == DateTime.Today.Month)
                {
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        foreach (DataGridViewCell cell in row.Cells)
                        {
                            if (cell.Value != null &&
                                Convert.ToInt32(cell.Value) == DateTime.Today.Day)
                            {
                                cell.Style.BackColor = Color.LightBlue;
                                cell.Style.ForeColor = Color.Black;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to highlight calendar dates: " + ex.Message,
                    "Error Message",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void ApplyCategoryColor(DataGridViewCell cell, string category)
        {
            switch (category?.Trim())
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

                case "Multiple Events":
                    cell.Style.BackColor = Color.LightPink;
                    cell.Style.ForeColor = Color.White;
                    break;
            }
        }
    }
}