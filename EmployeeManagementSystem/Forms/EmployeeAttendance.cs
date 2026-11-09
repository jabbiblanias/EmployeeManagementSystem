using EmployeeManagementSystem.Models;
using EmployeeManagementSystem.Repositories;
using EmployeeManagementSystem.Services;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EmployeeManagementSystem
{
    public partial class EmployeeAttendance : UserControl
    {
        private readonly AttendanceService _attendanceService;
        private readonly EmployeeService _employeeService;
        private readonly User _currentUser;

        public EmployeeAttendance(User user)
        {
            InitializeComponent();
            _attendanceService = new AttendanceService();
            _employeeService = new EmployeeService();
            this._currentUser = user;
            CurrentTimeAndDate();
            LimitDate();
            DisplayEmployeeDetail();
            displayEmployeeAttendaceListData();
        }
        private void LimitDate()
        {
            DateTime today = DateTime.Today;

            dateTimePickerDate.Value = today;
            dateTimePickerDate.MaxDate = today;


            dateTimePickerFrom.Value = today;
            dateTimePickerFrom.MaxDate = today;


            dateTimePickerTo.Value = today;
            dateTimePickerTo.MaxDate = today;
        }
        public void displayEmployeeAttendaceListData()
        {
            List<Attendance> listData =
                _attendanceService.GetEmployeeAttendance(_currentUser);

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

            dataGridView1.DataSource = listData;

            Attendance attendance =
                _attendanceService.GetAttendanceClock(_currentUser.ID);

            if (attendance == null)
            {
                lblStart.Text = "--:--";
                lblEnd.Text = "--:--";
                btnClockIn.Text = "Clock In";
                return;
            }

            // Clock In
            if (attendance.ClockIn.HasValue)
            {
                lblStart.Text = DateTime.Today
                    .Add(attendance.ClockIn.Value)
                    .ToString("hh:mm tt");

                // Clock Out
                if (attendance.ClockOut.HasValue)
                {
                    lblEnd.Text = DateTime.Today
                        .Add(attendance.ClockOut.Value)
                        .ToString("hh:mm tt");

                    btnClockIn.Text = "Clock In";
                }
                else
                {
                    lblEnd.Text = "--:--";
                    btnClockIn.Text = "Clock Out";
                }
            }
            else
            {
                lblStart.Text = "--:--";
                lblEnd.Text = "--:--";
                btnClockIn.Text = "Clock In";
            }
        }
        private void DisplayEmployeeDetail()
        {
            Employee employeeDetail = _employeeService.GetEmployeeDetails(_currentUser);
            pictureBox1.ImageLocation = employeeDetail.Image;
            lblEmpDisp.Text = employeeDetail.FullName;
            lblProfessionDisp.Text = employeeDetail.Profession;
        }

        private void btnClockIn_Click(object sender, EventArgs e)
        {
            DateTime currentDateTime = DateTime.Now;

            int accountId = _currentUser.ID;

            try
            {
                if (btnClockIn.Text == "Clock In")
                {
                    // CLOCK IN
                    string status = _attendanceService.ClockIn(
                        accountId,
                        currentDateTime
                    );

                    btnClockIn.Text = "Clock Out";

                    lblStart.Text = currentDateTime.ToString("hh:mm tt");

                    displayEmployeeAttendaceListData();
                }
                else
                {
                    // CLOCK OUT
                    TimeSpan overtime = _attendanceService.ClockOut(
                        accountId,
                        currentDateTime
                    );

                    lblEnd.Text = currentDateTime.ToString("hh:mm tt");

                    btnClockIn.Text = "Clock In";

                    displayEmployeeAttendaceListData();
                }
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Attendance Information",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            catch (SqlException)
            {
                MessageBox.Show(
                    "A database error occurred while processing your attendance.",
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "An unexpected error occurred: " + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }
        private async void CurrentTimeAndDate()
        {
            while (true)
            {
                DateTime dateTime = DateTime.Now;
                toolStripLabelTime.Text = dateTime.ToString("hh:mm:ss tt");

                toolStripLabelDate.Text = dateTime.ToString("MM/dd/yyyy");
                await Task.Delay(1000); // Wait for 1 second
            }
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            List<Attendance> listData = _attendanceService.GetCurrentDayAttendance(dateTimePickerDate.Value.ToString("yyyy-MM-dd"), _currentUser);
            dataGridView1.DataSource = listData;
        }

        private void btnEnter_Click(object sender, EventArgs e)
        {
            // Get the selected date from the DateTimePicker
            DateTime selectedFrom = dateTimePickerFrom.Value;
            int fromMonth = selectedFrom.Month;
            int fromYear = selectedFrom.Year;

            DateTime selectedTo = dateTimePickerTo.Value;
            // Extract the month and year
            int toMonth = selectedTo.Month;
            int toYear = selectedTo.Year;

            if (selectedFrom <= selectedTo)
            {
                List<Attendance> listData = _attendanceService.GetAttendanceByMonthYear(fromMonth, fromYear, toMonth, toYear, _currentUser);
                dataGridView1.DataSource = listData;
            }
            else
            {
                MessageBox.Show("The date is not applicable.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            displayEmployeeAttendaceListData();
        }
    }
}
