using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace EmployeeManagementSystem
{
    public partial class EditEvent : UserControl
    {
        private EventForm eventForm;
        LoginView loginView;
        private string date;
        SqlConnection connect =
            new SqlConnection(@"Data Source=LAPTOP-0OGAQKFF\SQLEXPRESS;Initial Catalog=DB_Employee;User ID=ja;Password=john");
        public EditEvent(EventForm eventForm, LoginView loginView)
        {
            InitializeComponent();
            this.loginView = loginView;
            this.eventForm = eventForm;
            this.date = eventForm.Date;
            lblDate.Text = eventForm.WordDate;
            ComboBoxShowTitles();
        }
        private void ComboBoxShowTitles()
        {
            comboBoxTitles.Items.Clear();
            try
            {
                connect.Close();
                connect.Open();
                string selectData = "SELECT TITLE FROM TBL_EVENTS WHERE EVENT_DATE = @EVENT_DATE AND ACCOUNT_ID = @ACCOUNT_ID";
                using (SqlCommand cmd = new SqlCommand(selectData, connect))
                {
                    cmd.Parameters.AddWithValue("@ACCOUNT_ID", loginView.ID);
                    cmd.Parameters.AddWithValue("@EVENT_DATE", date);

                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        string title = reader.GetString(0);
                        comboBoxTitles.Items.Add(title);
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

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                connect.Open();
                string query = @"SELECT 
                            CASE 
                                WHEN EXISTS (SELECT 1 FROM TBL_EVENTS WHERE TITLE = @TITLE AND TITLE <> @ExcludedTITLE AND EVENT_DATE = @EVENT_DATE) THEN 1 ELSE 0 
                            END AS TitleExists";

                using (SqlCommand command = new SqlCommand(query, connect))
                {
                    command.Parameters.AddWithValue("@TITLE", textBoxTitle.Text);
                    command.Parameters.AddWithValue("@ExcludedTITLE", comboBoxTitles.SelectedItem.ToString());
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

                                string updateData = "UPDATE TBL_EVENTS SET TITLE = @TITLE, DESCRIPTION = @DESCRIPTION, LOCATION = @LOCATION, START_TIME = @START_TIME, END_TIME = @END_TIME " +
                                "WHERE ACCOUNT_ID = @ACCOUNT_ID AND EVENT_DATE = @EVENT_DATE";

                                using (SqlCommand cmd = new SqlCommand(updateData, connect))
                                {
                                    cmd.Parameters.AddWithValue("@TITLE", textBoxTitle.Text.Trim());
                                    cmd.Parameters.AddWithValue("@DESCRIPTION", textBoxDescription.Text.Trim());
                                    cmd.Parameters.AddWithValue("@LOCATION", textBoxLocation.Text.Trim());
                                    cmd.Parameters.AddWithValue("@START_TIME", dateTimePickerFrom.Value.TimeOfDay.ToString(@"hh\:mm"));
                                    cmd.Parameters.AddWithValue("@END_TIME", dateTimePickerTo.Value.TimeOfDay.ToString(@"hh\:mm"));
                                    cmd.Parameters.AddWithValue("@ACCOUNT_ID", loginView.ID);
                                    cmd.Parameters.AddWithValue("@EVENT_DATE", date);

                                    cmd.ExecuteNonQuery();
                                    MessageBox.Show($"'{textBoxTitle.Text.Trim()}' has been saved!"
                                        , "Information Message", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                    Clear();
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
            finally
            {
                connect.Close();
            }
        }

        private void comboBoxTitles_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                connect.Open();
                string selectData = "SELECT * FROM TBL_EVENTS WHERE TITLE = @TITLE AND EVENT_DATE = @EVENT_DATE AND ACCOUNT_ID = @ACCOUNT_ID";
                using (SqlCommand cmd = new SqlCommand(selectData, connect))
                {
                    cmd.Parameters.AddWithValue("@TITLE", comboBoxTitles.SelectedItem.ToString());
                    cmd.Parameters.AddWithValue("@ACCOUNT_ID", loginView.ID);
                    cmd.Parameters.AddWithValue("@EVENT_DATE", date);

                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        textBoxTitle.Text = reader.GetString(1);
                        textBoxDescription.Text = reader.GetString(2);
                        textBoxLocation.Text = reader.GetString(3);
                        dateTimePickerFrom.Value = DateTime.Today.Add(reader.GetTimeSpan(5));
                        dateTimePickerTo.Value = DateTime.Today.Add(reader.GetTimeSpan(6));
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

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            try
            {
                connect.Open();
                string selectData = "DELETE FROM TBL_EVENTS WHERE TITLE = @TITLE AND EVENT_DATE = @EVENT_DATE AND ACCOUNT_ID = @ACCOUNT_ID";
                using (SqlCommand cmd = new SqlCommand(selectData, connect))
                {
                    cmd.Parameters.AddWithValue("@TITLE", comboBoxTitles.SelectedItem.ToString());
                    cmd.Parameters.AddWithValue("@ACCOUNT_ID", loginView.ID);
                    cmd.Parameters.AddWithValue("@EVENT_DATE", date);

                    cmd.ExecuteNonQuery();

                    ComboBoxShowTitles();
                    Clear();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex
                    , "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void Clear()
        {
            textBoxTitle.Text = string.Empty;
            textBoxDescription.Text = string.Empty;
            textBoxLocation.Text = string.Empty;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            eventForm.Dispose();
        }
    }
}
