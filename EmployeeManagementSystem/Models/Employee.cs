using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace EmployeeManagementSystem.Models
{
    public class Employee
    {
        public int ID { set; get; }
        public string EmployeeID { set; get; }
        public string Name { set; get; }
        public DateTime DateofBirth { set; get; }
        public string Gender { set; get; }
        public string Contact { set; get; }
        public string Profession { set; get; }
        public string PrimaryContactNum { get; set; }
        public string FullName { get; set; }
        public string SecContactNum { get; set; }
        public string EmailAdd { get; set; }
        public string EmergencyContactNum { get; set; }
        public string CurrentAdd { get; set; }
        public string Image { get; set; }
        public string Status { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public int Salary { set; get; }
    }

    public class OtherInfo
    {

    }
}
