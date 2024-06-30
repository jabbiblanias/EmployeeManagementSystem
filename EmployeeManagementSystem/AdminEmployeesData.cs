using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace EmployeeManagementSystem
{
    class AdminEmployeesData
    {
        public int ID { set; get; }
        public string EmployeeID { set; get; }
        public string Name { set; get; }
        public DateTime DateofBirth { set; get; }
        public string Gender { set; get; }
        public string Contact { set; get; }
        public string Profession { set; get; }
        public string Image { set; get; }

        SqlConnection connect =
            new SqlConnection(@"Data Source=LAPTOP-0OGAQKFF\SQLEXPRESS;Initial Catalog=DB_Employee;User ID=ja;Password=john");
        public List<AdminEmployeesData> employeeListData()
        {
            List<AdminEmployeesData> listdata = new List<AdminEmployeesData>();

            if (connect.State != ConnectionState.Open)
            {
                try
                {
                    connect.Open();

                    string selectData = "SELECT * FROM TBL_EMPLOYEE_INFO WHERE STATUS = 'active'";

                    using (SqlCommand cmd = new SqlCommand(selectData, connect))
                    {
                        SqlDataReader reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            AdminEmployeesData ed = new AdminEmployeesData();
                            ed.ID = (int)reader["ID"];
                            ed.EmployeeID = reader["EMPLOYEE_ID"].ToString();
                            ed.Name = reader["FULL_NAME"].ToString();
                            ed.DateofBirth = (DateTime)reader["DATEOFBIRTH"];
                            ed.Gender = reader["GENDER"].ToString();
                            ed.Contact = reader["CONTACT_NUMBER"].ToString();
                            ed.Profession = reader["PROFESSION"].ToString();
                            ed.Image = reader["IMAGE"].ToString();

                            listdata.Add(ed);
                        }
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex);
                }
                finally
                {
                    connect.Close();
                }
            }
            return listdata;
        }
        
        public List<AdminEmployeesData> ArchiveEmployeeListData()
        {
            List<AdminEmployeesData> listdata = new List<AdminEmployeesData>();

            if (connect.State != ConnectionState.Open)
            {
                try
                {
                    connect.Open();

                    string selectData = "SELECT * FROM TBL_EMPLOYEE_INFO WHERE STATUS = 'inactive'";

                    using (SqlCommand cmd = new SqlCommand(selectData, connect))
                    {
                        SqlDataReader reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            AdminEmployeesData ed = new AdminEmployeesData();
                            ed.ID = (int)reader["ID"];
                            ed.EmployeeID = reader["EMPLOYEE_ID"].ToString();
                            ed.Name = reader["FULL_NAME"].ToString();
                            ed.DateofBirth = (DateTime)reader["DATEOFBIRTH"];
                            ed.Gender = reader["GENDER"].ToString();
                            ed.Contact = reader["CONTACT_NUMBER"].ToString();
                            ed.Profession = reader["PROFESSION"].ToString();
                            ed.Image = reader["IMAGE"].ToString();

                            listdata.Add(ed);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex);
                }
                finally
                {
                    connect.Close();
                }
            }
            return listdata;
        }
    }
}
