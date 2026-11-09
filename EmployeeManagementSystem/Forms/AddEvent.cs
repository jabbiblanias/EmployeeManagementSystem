using EmployeeManagementSystem.Models;
using EmployeeManagementSystem.Services;
using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace EmployeeManagementSystem
{
    public partial class AddEvent : UserControl
    {
        private readonly EventForm _eventForm;
        private readonly User _currentUser;
        private readonly EventService _eventService;

        private readonly string _date;

        public AddEvent(EventForm eventForm, User currentUser)
        {
            InitializeComponent();

            _eventForm = eventForm;
            _currentUser = currentUser;

            _eventService = new EventService();

            _date = eventForm.Date;
            lblDate.Text = eventForm.WordDate;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (!DateTime.TryParse(_date, out DateTime eventDate))
                {
                    MessageBox.Show(
                        "Invalid event date.",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );

                    return;
                }

                _eventService.AddEvent(
                    _currentUser.ID,
                    textBoxTitle.Text,
                    textBoxDescription.Text,
                    textBoxLocation.Text,
                    eventDate,
                    dateTimePickerFrom.Value.TimeOfDay,
                    dateTimePickerTo.Value.TimeOfDay
                );

                MessageBox.Show(
                    $"'{textBoxTitle.Text.Trim()}' has been saved!",
                    "Information Message",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                _eventForm.Dispose();
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Event Information",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }
            catch (SqlException)
            {
                MessageBox.Show(
                    "A database error occurred while saving the event.",
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "An unexpected error occurred: " + ex.Message,
                    "Error Message",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            _eventForm.Dispose();
        }
    }
}