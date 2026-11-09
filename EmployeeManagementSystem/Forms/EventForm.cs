using EmployeeManagementSystem.Models;
using EmployeeManagementSystem.Services;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace EmployeeManagementSystem
{
    public partial class EventForm : Form
    {
        private readonly User _currentUser;
        private readonly EventCalendar calendar;
        private readonly EventService _eventService;

        private string _selectedEventTitle;

        public string WordDate { get; set; }
        public string Date { get; set; }

        public EventForm(EventCalendar eventCalendar, User user)
        {
            InitializeComponent();

            _currentUser = user;
            calendar = eventCalendar;
            _eventService = new EventService();

            lblDate.Text = eventCalendar.WordDate;
            Date = eventCalendar.Date;

            AddButton();
        }

        private DateTime GetEventDate()
        {
            DateTime eventDate;

            if (!DateTime.TryParse(Date, out eventDate))
            {
                throw new InvalidOperationException("Invalid event date.");
            }

            return eventDate.Date;
        }

        private void exit_Click(object sender, EventArgs e)
        {
            Dispose();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Dispose();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            AddButton();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (_currentUser.Role.Equals("Employee"))
            {
                Size = new Size(249, 450);
                panel2.Size = new Size(249, 450);
                panel3.Size = new Size(249, 450);
            }
            else
            {
                Size = new Size(249, 500);
                panel2.Size = new Size(249, 500);
                panel3.Size = new Size(249, 500);
            }

            label1.Text = "Edit the Event";

            panelCurrentTitle.Visible = true;
            buttonDelete.Visible = true;
            buttonSave.Text = "update";

            Clear();
            _selectedEventTitle = null;

            ComboBoxShowTitles();
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime eventDate = GetEventDate();

                string title = textBoxTitle.Text.Trim();
                string description = textBoxDescription.Text.Trim();
                string location = textBoxLocation.Text.Trim();

                TimeSpan startTime = dateTimePickerFrom.Value.TimeOfDay;
                TimeSpan endTime = dateTimePickerTo.Value.TimeOfDay;

                if (buttonSave.Text.Equals(
                    "save", StringComparison.OrdinalIgnoreCase))
                {
                    _eventService.AddEvent(
                        _currentUser.ID,
                        title,
                        description,
                        comboBoxCategory.SelectedItem?.ToString(),
                        location,
                        eventDate,
                        startTime,
                        endTime
                    );

                    MessageBox.Show(
                        $"'{title}' has been saved!",
                        "Information Message",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );

                    calendar.DisplayEvents();
                    Dispose();
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(_selectedEventTitle))
                    {
                        MessageBox.Show(
                            "Please select an event to update.",
                            "Error Message",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning
                        );

                        return;
                    }

                    _eventService.UpdateEvent(
                        _currentUser.ID,
                        _selectedEventTitle,
                        title,
                        description,
                        comboBoxCategory.SelectedItem?.ToString(),
                        location,
                        eventDate,
                        startTime,
                        endTime
                    );

                    MessageBox.Show(
                        $"'{title}' has been updated!",
                        "Information Message",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );

                    calendar.DisplayEvents();

                    // Refresh the event titles after updating.
                    ComboBoxShowTitles();

                    Clear();
                    _selectedEventTitle = null;
                }
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Validation Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "An error occurred: " + ex.Message,
                    "Error Message",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_selectedEventTitle))
            {
                MessageBox.Show(
                    "Please select an event to delete.",
                    "Information Message",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return;
            }

            DialogResult result = MessageBox.Show(
                "Are you sure you want to delete this event?",
                "Confirmation Message",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result != DialogResult.Yes)
                return;

            try
            {
                DateTime eventDate = GetEventDate();

                _eventService.DeleteEvent(
                    _selectedEventTitle,
                    eventDate,
                    _currentUser.ID
                );

                MessageBox.Show(
                    "Event deleted successfully.",
                    "Information Message",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                calendar.DisplayEvents();

                ComboBoxShowTitles();

                Clear();
                _selectedEventTitle = null;
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Error Message",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "An error occurred: " + ex.Message,
                    "Error Message",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void ComboBoxShowTitles()
        {
            try
            {
                comboBoxTitles.Items.Clear();
                _selectedEventTitle = null;

                DateTime eventDate = GetEventDate();

                var titles = _eventService.GetEventTitles(
                    eventDate,
                    _currentUser.ID
                );

                foreach (string title in titles)
                {
                    comboBoxTitles.Items.Add(title);
                }

                comboBoxTitles.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to load event titles: " + ex.Message,
                    "Error Message",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void comboBoxTitles_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxTitles.SelectedItem == null)
                return;

            try
            {
                string selectedTitle = comboBoxTitles.SelectedItem.ToString();

                DateTime eventDate = GetEventDate();

                Event selectedEvent = _eventService.GetEvent(
                    selectedTitle,
                    eventDate,
                    _currentUser.ID
                );

                if (selectedEvent == null)
                {
                    MessageBox.Show(
                        "The selected event could not be found.",
                        "Information Message",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );

                    return;
                }

                // Keep the original title for the update operation.
                _selectedEventTitle = selectedTitle;

                textBoxTitle.Text = selectedEvent.Title;
                textBoxDescription.Text = selectedEvent.Description ?? string.Empty;
                textBoxLocation.Text = selectedEvent.Location ?? string.Empty;
                comboBoxCategory.SelectedItem = selectedEvent.Category.Trim() ?? string.Empty;

                dateTimePickerFrom.Value =
                    DateTime.Today.Add(selectedEvent.StartTime);

                dateTimePickerTo.Value =
                    DateTime.Today.Add(selectedEvent.EndTime);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to load event details: " + ex.Message,
                    "Error Message",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void Clear()
        {
            textBoxTitle.Clear();
            textBoxDescription.Clear();
            textBoxLocation.Clear();

            dateTimePickerFrom.Value = DateTime.Today;
            dateTimePickerTo.Value = DateTime.Today;
        }

        private void AddButton()
        {
            if (_currentUser.Role.Equals("Employee"))
            {
                Size = new Size(249, 400);
                panel2.Size = new Size(249, 400);
                panel3.Size = new Size(249, 400);

                panelCategory.Visible = false;
            }
            else
            {
                Size = new Size(249, 450);
                panel2.Size = new Size(249, 450);
                panel3.Size = new Size(249, 450);
            }

            label1.Text = "New Event";

            panelCurrentTitle.Visible = false;
            buttonDelete.Visible = false;

            buttonSave.Text = "save";

            _selectedEventTitle = null;

            Clear();
        }
    }
}