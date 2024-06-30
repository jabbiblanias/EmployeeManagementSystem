using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EmployeeManagementSystem
{
    public partial class EventForm : Form
    {
        UserControl currentControl;
        LoginView loginView;
        public string WordDate {  get; set; }
        public string Date { get; set; }
        public EventForm(EventCalendar eventCalendar, LoginView loginView)
        {
            InitializeComponent();
            this.loginView = loginView;
            WordDate = eventCalendar.WordDate;
            Date = eventCalendar.Date;
            changeCurrentPanel(new AddEvent(this, loginView));

        }
        private void changeCurrentPanel(UserControl newUserControl)
        {
            panel3.Controls.Remove(currentControl);
            panel3.Controls.Add(newUserControl);
            currentControl = newUserControl;

        }

        private void exit_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            label1.Text = "Edit the Event";
            this.Size = new Size(249, 450);
            panel2.Size = new Size(249, 450);
            panel3.Size = new Size(330, 450);
            changeCurrentPanel(new EditEvent(this, loginView));
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            label1.Text = "New Event";
            this.Size = new Size(249, 410);
            panel2.Size = new Size(249, 410);
            panel3.Size = new Size(330, 410);
            changeCurrentPanel(new AddEvent(this, loginView));
        }
    }
}
