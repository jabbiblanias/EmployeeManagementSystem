using EmployeeManagementSystem.Models;
using EmployeeManagementSystem.Services;
using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace EmployeeManagementSystem
{
    public partial class EditEvent : UserControl
    {
        private readonly EventForm _eventForm;
        private readonly User _currentUser;
        private readonly EventService _eventService;

        private readonly DateTime _eventDate;

        public EditEvent(EventForm eventForm, User user)
        {
            InitializeComponent();

            _eventForm = eventForm;
            _currentUser = user;

            _eventService = new EventService();

            if (!DateTime.TryParse(eventForm.Date, out _eventDate))
            {
                throw new ArgumentException("Invalid event date.");
            }

            lblDate.Text = eventForm.WordDate;

            ComboBoxShowTitles();
        }

        // DISPLAY EVENT TITLES
        private void ComboBoxShowTitles()
        {
            try
            {
                comboBoxTitles.Items.Clear();

                var titles = _eventService.GetEventTitles(
                    _eventDate,
                    _currentUser.ID
                );

                foreach (string title in titles)
                {
                    comboBoxTitles.Items.Add(title);
                }

                comboBoxTitles.SelectedIndex = -1;
            }
            catch (SqlException)
            {
                MessageBox.Show(
                    "A database error occurred while loading event titles.",
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // LOAD SELECTED EVENT DETAILS
        private void comboBoxTitles_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxTitles.SelectedItem == null)
                return;

            try
            {
                string selectedTitle = comboBoxTitles.SelectedItem.ToString();

                Event selectedEvent = _eventService.GetEvent(
                    selectedTitle,
                    _eventDate,
                    _currentUser.ID
                );

                if (selectedEvent == null)
                {
                    MessageBox.Show(
                        "Event not found.",
                        "Information",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );

                    return;
                }

                textBoxTitle.Text = selectedEvent.Title;
                textBoxDescription.Text = selectedEvent.Description;
                textBoxLocation.Text = selectedEvent.Location;

                dateTimePickerFrom.Value =
                    DateTime.Today.Add(selectedEvent.StartTime);

                dateTimePickerTo.Value =
                    DateTime.Today.Add(selectedEvent.EndTime);
            }
            catch (SqlException)
            {
                MessageBox.Show(
                    "A database error occurred while retrieving event details.",
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // UPDATE EVENT
        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBoxTitles.SelectedItem == null)
            {
                MessageBox.Show(
                    "Please select an event to update.",
                    "Information",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return;
            }

            try
            {
                string originalTitle = comboBoxTitles.SelectedItem.ToString();

                _eventService.UpdateEvent(
                    _currentUser.ID,
                    originalTitle,
                    textBoxTitle.Text,
                    textBoxDescription.Text,
                    textBoxLocation.Text,
                    _eventDate,
                    dateTimePickerFrom.Value.TimeOfDay,
                    dateTimePickerTo.Value.TimeOfDay
                );

                MessageBox.Show(
                    $"'{textBoxTitle.Text.Trim()}' has been updated!",
                    "Information Message",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                Clear();
                ComboBoxShowTitles();
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
                    "A database error occurred while updating the event.",
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

        // DELETE EVENT
        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (comboBoxTitles.SelectedItem == null)
            {
                MessageBox.Show(
                    "Please select an event to delete.",
                    "Information",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return;
            }

            string selectedTitle = comboBoxTitles.SelectedItem.ToString();

            DialogResult result = MessageBox.Show(
                $"Are you sure you want to delete '{selectedTitle}'?",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result != DialogResult.Yes)
                return;

            try
            {
                _eventService.DeleteEvent(
                    selectedTitle,
                    _eventDate,
                    _currentUser.ID
                );

                MessageBox.Show(
                    "Event deleted successfully.",
                    "Information",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                Clear();
                ComboBoxShowTitles();
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
                    "A database error occurred while deleting the event.",
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

        // CLEAR INPUT FIELDS
        private void Clear()
        {
            comboBoxTitles.SelectedIndex = -1;

            textBoxTitle.Clear();
            textBoxDescription.Clear();
            textBoxLocation.Clear();

            dateTimePickerFrom.Value = DateTime.Today;
            dateTimePickerTo.Value = DateTime.Today;
        }

        // CANCEL
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            _eventForm.Dispose();
        }
    }
}