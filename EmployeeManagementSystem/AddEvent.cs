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

namespace EmployeeManagementSystem
{
    public partial class AddEvent : UserControl
    {
        private EventForm eventForm;
        LoginView loginView;
        SqlConnection connect =
            new SqlConnection(@"Data Source=LAPTOP-0OGAQKFF\SQLEXPRESS;Initial Catalog=DB_Employee;User ID=ja;Password=john");
        private string date;
        public AddEvent(EventForm eventForm, LoginView loginView)
        {
            InitializeComponent();
            this.eventForm = eventForm;
            this.loginView = loginView;
            this.date = eventForm.Date;
            lblDate.Text = eventForm.WordDate;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                connect.Open();
                string query = @"SELECT 
                            CASE 
                                WHEN EXISTS (SELECT 1 FROM TBL_EVENTS WHERE TITLE = @TITLE AND EVENT_DATE = @EVENT_DATE) THEN 1 ELSE 0 
                            END AS TitleExists"
                ;

                using (SqlCommand command = new SqlCommand(query, connect))
                {
                    command.Parameters.AddWithValue("@TITLE", textBoxTitle.Text.Trim());
                    command.Parameters.AddWithValue("@EVENT_DATE", date);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            bool TitleExists = reader.GetInt32(0) == 1;

                            if (TitleExists)
                            {
                                MessageBox.Show("The title is already taken, name a different title."
                                    , "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {
                                connect.Close();
                                connect.Open();

                                string updateData = "INSERT INTO TBL_EVENTS (TITLE, DESCRIPTION, LOCATION, EVENT_DATE, START_TIME, END_TIME, ACCOUNT_ID) " +
                                    "VALUES (@TITLE, @DESCRIPTION, @LOCATION, @EVENT_DATE, @START_TIME, @END_TIME, @ACCOUNT_ID)";

                                using (SqlCommand cmd = new SqlCommand(updateData, connect))
                                {
                                    cmd.Parameters.AddWithValue("@TITLE", textBoxTitle.Text.Trim());
                                    cmd.Parameters.AddWithValue("@DESCRIPTION", textBoxDescription.Text.Trim());
                                    cmd.Parameters.AddWithValue("@LOCATION", textBoxLocation.Text.Trim());
                                    cmd.Parameters.AddWithValue("@EVENT_DATE", date);
                                    cmd.Parameters.AddWithValue("@START_TIME", dateTimePickerFrom.Value.TimeOfDay.ToString(@"hh\:mm"));
                                    cmd.Parameters.AddWithValue("@END_TIME", dateTimePickerTo.Value.TimeOfDay.ToString(@"hh\:mm"));
                                    cmd.Parameters.AddWithValue("@ACCOUNT_ID", loginView.ID);

                                    cmd.ExecuteNonQuery();
                                    connect.Close();
                                    MessageBox.Show($"'{textBoxTitle.Text.Trim()}' has been saved!"
                                        , "Information Message", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                    eventForm.Dispose();
                                }
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
        }

        private void button2_Click(object sender, EventArgs e)
        {
            eventForm.Dispose();
        }
    }
}
