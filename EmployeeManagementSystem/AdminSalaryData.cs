using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace EmployeeManagementSystem
{
    class AdminSalaryData
    {
        public string EmployeeID { set; get; } // 1
        public string Name { set; get; } // 2
        public string Profession { set; get; } // 5
        public int Salary { set; get; } // 7

        SqlConnection connect =
            new SqlConnection(@"Data Source=LAPTOP-0OGAQKFF\SQLEXPRESS;Initial Catalog=DB_Employee;User ID=ja;Password=john");
        public List<AdminSalaryData> salaryEmployeeListData()
        {
            List<AdminSalaryData> listdata = new List<AdminSalaryData>();

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
                            AdminSalaryData ed = new AdminSalaryData();
                            ed.EmployeeID = reader["EMPLOYEE_ID"].ToString();
                            ed.Name = reader["FULL_NAME"].ToString();
                            ed.Profession = reader["PROFESSION"].ToString();
                            ed.Salary = (int)reader["SALARY"];

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
