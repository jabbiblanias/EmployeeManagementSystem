using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagementSystem.Models
{
    public class Attendance: Employee
    {
        public DateTime Date { get; set; }
        public TimeSpan? ClockIn { get; set; }
        public TimeSpan? ClockOut { get; set; }
        public TimeSpan? Overtime { get; set; }
    }
}
