using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace EmployeeManagementSystem
{
    public partial class EmployeeAttendance : UserControl
    {
        DateTime today = DateTime.Now;
        LoginView loginView;
        SqlConnection connect =
            new SqlConnection(@"Data Source=LAPTOP-0OGAQKFF\SQLEXPRESS;Initial Catalog=DB_Employee;User ID=ja;Password=john");
        public EmployeeAttendance(LoginView loginView)
        {
            InitializeComponent();
            this.loginView = loginView;
            LimitDate();
            EmployeeDetail();
            displayEmployeeAttendaceListData();
            CurrentTimeAndDate();
        }
        private void LimitDate()
        {
            dateTimePickerDate.Value = today;
            dateTimePickerDate.MaxDate = today;
            dateTimePickerFrom.Value = today;
            dateTimePickerFrom.MaxDate = today;
            dateTimePickerTo.Value = today;
            dateTimePickerTo.MaxDate = today;
        }
        public void displayEmployeeAttendaceListData()
        {
            EmployeeAttendanceData ed = new EmployeeAttendanceData(loginView);
            List<EmployeeAttendanceData> listData = ed.employeeAttendanceListData();

            dataGridView1.DataSource = listData;
            try
            {

                connect.Open();
                string selectData = "SELECT CLOCK_IN, CLOCK_OUT FROM TBL_ATTENDANCE WHERE DATE = @DATE AND ACCOUNT_ID = @ACCOUNT_ID";
                using (SqlCommand cmd = new SqlCommand(selectData, connect))
                {
                    cmd.Parameters.AddWithValue("@DATE", today.Date);
                    cmd.Parameters.AddWithValue("@ACCOUNT_ID", loginView.ID);
                    int count = cmd.ExecuteNonQuery();
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        lblStart.Text = DateTime.Today.Add(reader.GetTimeSpan(0)).ToString("hh:mm tt");

                        if (!reader.IsDBNull(1))
                        {
                            lblEnd.Text = DateTime.Today.Add(reader.GetTimeSpan(1)).ToString("hh:mm tt");
                        }
                        else
                        {
                            btnClockIn.Text = "Clock Out";
                        }
                    }
                    else
                    {
                        btnClockIn.Text = "Clock In";
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
        private void EmployeeDetail()
        {
            try
            {
                connect.Open();
                string selectData = "SELECT TBL_EMPLOYEE_INFO.IMAGE, TBL_EMPLOYEE_INFO.FULL_NAME, TBL_EMPLOYEE_INFO.PROFESSION, TBL_EMPLOYEE_INFO.ID FROM TBL_EMPLOYEE_INFO INNER JOIN TBL_USERS ON TBL_EMPLOYEE_INFO.ID = TBL_USERS.ID WHERE TBL_USERS.ACCOUNT_ID = @ACCOUNT_ID";
                using (SqlCommand cmd = new SqlCommand(selectData, connect))
                {
                    cmd.Parameters.AddWithValue("@ACCOUNT_ID", loginView.ID);
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        pictureBox1.ImageLocation = reader.GetString(0);
                        lblEmpDisp.Text = reader.GetString(1);
                        lblProfessionDisp.Text = reader.GetString(2);
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

        private void btnClockIn_Click(object sender, EventArgs e)
        {
            string formattedDate = today.ToString("yyyy-MM-dd"); ;
            string formattedTime = formattedTime = today.ToString("hh:mm tt");
            if (btnClockIn.Text == "Clock In")
            {
                try
                {
                    connect.Open();
                    string selectData = @"IF NOT EXISTS (SELECT 1 FROM TBL_ATTENDANCE WHERE DATE = @DATE AND ACCOUNT_ID = @ACCOUNT_ID) 
                        BEGIN
                             INSERT INTO TBL_ATTENDANCE (DATE, CLOCK_IN, ACCOUNT_ID) 
                             VALUES(@DATE, @CLOCK_IN, @ACCOUNT_ID)
                        END";
                    using (SqlCommand cmd = new SqlCommand(selectData, connect))
                    {
                        cmd.Parameters.AddWithValue("@DATE", formattedDate);
                        cmd.Parameters.AddWithValue("@CLOCK_IN", DateTime.Parse(formattedTime).TimeOfDay);
                        cmd.Parameters.AddWithValue("@ACCOUNT_ID", loginView.ID);

                        int count = cmd.ExecuteNonQuery();
                        connect.Close();
                        if (count < 0)
                        {
                            MessageBox.Show("You have already attendance for today"
                                        , "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            btnClockIn.Text = "Clock Out";
                            lblStart.Text = formattedTime;
                            displayEmployeeAttendaceListData();
                        }
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
                try
                {

                    connect.Open();
                    string selectData = "UPDATE TBL_ATTENDANCE SET CLOCK_OUT = @CLOCK_OUT WHERE ACCOUNT_ID = @ACCOUNT_ID AND DATE = @DATE AND CLOCK_OUT IS NULL ";
                    using (SqlCommand cmd = new SqlCommand(selectData, connect))
                    {
                        cmd.Parameters.AddWithValue("@DATE", formattedDate);
                        cmd.Parameters.AddWithValue("@CLOCK_OUT", DateTime.Parse(formattedTime).TimeOfDay);
                        cmd.Parameters.AddWithValue("@ACCOUNT_ID", loginView.ID);

                        cmd.ExecuteNonQuery();
                        connect.Close();
                    }
                    btnClockIn.Text = "Clock In";
                    lblEnd.Text = formattedTime;
                    displayEmployeeAttendaceListData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex
                        , "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private async void CurrentTimeAndDate()
        {
            while (true)
            {
                toolStripLabelTime.Text = today.ToString("hh:mm:ss tt");

                toolStripLabelDate.Text = today.ToString("MM/dd/yyyy");
                await Task.Delay(1000); // Wait for 1 second
            }
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            EmployeeAttendanceData attendanceData = new EmployeeAttendanceData(loginView);
            EmployeeAttendanceData ed = new EmployeeAttendanceData(loginView);
            List<EmployeeAttendanceData> listData = ed.currentDayAttendanceListData(dateTimePickerDate.Value.ToString("yyyy-MM-dd"));

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

            int compare = selectedFrom.CompareTo(selectedTo);
            if (selectedFrom <= selectedTo)
            {
                EmployeeAttendanceData employeeAttendanceData = new EmployeeAttendanceData(loginView);
                List<EmployeeAttendanceData> listData = employeeAttendanceData.MonthYearAttendanceListData(fromMonth, fromYear, toMonth, toYear);
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
