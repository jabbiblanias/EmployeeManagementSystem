using System;

namespace EmployeeManagementSystem.Models
{
    internal class Event
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string Location { get; set; }
        public DateTime EventDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}