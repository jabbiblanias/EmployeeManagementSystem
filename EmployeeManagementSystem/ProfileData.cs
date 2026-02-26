using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagementSystem
{
    internal abstract class Profile
    {
        public string EmployeeID { get; set; }
        public string Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Profession { get; set; }
        public string PrimaryContactNum { get; set; }
        public string FullName { get; set; }
        public string SecContactNum { get; set; }
        public string EmailAdd { get; set; }
        public string EmergencyContactNum { get; set; }
        public string CurrentAdd { get; set; }
        public string Image { get; set; }
    }
    internal class DisplayUserProfileData: Profile
    {
        protected LoginView loginView;
        

        SqlConnection connect = new SqlConnection(@"Data Source=LAPTOP-0OGAQKFF\SQLEXPRESS;Initial Catalog=DB_Employee;User ID=ja;Password=john");
        public DisplayUserProfileData(LoginView loginView)
        {
            this.loginView = loginView;
        }
        public void displayData()
        {
            connect.Open();
            string query = @"SELECT * FROM TBL_EMPLOYEE_INFO INNER JOIN TBL_USERS ON TBL_EMPLOYEE_INFO.ID = TBL_USERS.ID WHERE TBL_USERS.ACCOUNT_ID = @ACCOUNT_ID";
            using (SqlCommand cmd = new SqlCommand(query, connect))
            {
                cmd.Parameters.AddWithValue("@ACCOUNT_ID", loginView.ID);
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    FullName = reader["FULL_NAME"].ToString();
                    SecContactNum = reader["SECONDARY_CONTACT_NUM"].ToString();
                    EmailAdd = reader["EMAIL_ADD"].ToString();
                    EmergencyContactNum = reader["EMERGENCY_CONTACT_NUM"].ToString();
                    CurrentAdd = reader["CURRENT_ADD"].ToString();
                    EmployeeID = reader["EMPLOYEE_ID"].ToString();
                    Gender = reader["GENDER"].ToString();
                    DateOfBirth = (DateTime)reader["DATEOFBIRTH"];
                    Profession = reader["PROFESSION"].ToString();
                    PrimaryContactNum = reader["PRIMARY_CONTACT_NUM"].ToString();
                    Image = reader["IMAGE"].ToString();
                }
            }
            connect.Close();
        }
    }
    internal class ProfileData : Profile
    {
        public string Status { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }

        public SqlConnection connect = new SqlConnection(@"Data Source=LAPTOP-0OGAQKFF\SQLEXPRESS;Initial Catalog=DB_Employee;User ID=ja;Password=john");
        public void EmployeeInfo()
        {
            connect.Open();
            string query = @"SELECT * FROM TBL_EMPLOYEE_INFO WHERE EMPLOYEE_ID = @EMPLOYEE_ID";
            using (SqlCommand cmd = new SqlCommand(query, connect))
            {
                cmd.Parameters.AddWithValue("@EMPLOYEE_ID", EmployeeID);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    FullName = reader["FULL_NAME"].ToString();
                    SecContactNum = reader["SECONDARY_CONTACT_NUM"].ToString();
                    EmailAdd = reader["EMAIL_ADD"].ToString();
                    EmergencyContactNum = reader["EMERGENCY_CONTACT_NUM"].ToString();
                    CurrentAdd = reader["CURRENT_ADD"].ToString();
                    EmployeeID = reader["EMPLOYEE_ID"].ToString();
                    Gender = reader["GENDER"].ToString();
                    DateOfBirth = (DateTime)reader["DATEOFBIRTH"];
                    Profession = reader["PROFESSION"].ToString();
                    PrimaryContactNum = reader["PRIMARY_CONTACT_NUM"].ToString();
                    Status = reader["STATUS"].ToString();
                    DateCreated = (DateTime)reader["DATE_CREATED"];
                    DateModified = (DateTime)reader["DATE_MODIFIED"];
                    Image = reader["IMAGE"].ToString();
                }
            }
            connect.Close();
        }
    }
}
