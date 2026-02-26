using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace EmployeeManagementSystem
{
    internal class AdminDashboardData
    {
        SqlConnection connect =
            new SqlConnection("Data Source=LAPTOP-0OGAQKFF\\SQLEXPRESS;Initial Catalog=DB_Employee;User ID=ja;Password=john");
        public int totalOfEmployees()
        {
            connect.Open();

            string selectData = "SELECT COUNT(ID) FROM TBL_EMPLOYEE_INFO";

            SqlCommand cmd = new SqlCommand(selectData, connect);
            int count = Convert.ToInt32(cmd.ExecuteScalar());

            connect.Close();
            return count;
        }
        public int numberOfActiveEmployees()
        {
            connect.Open();

            string selectData = "SELECT COUNT(ID) FROM TBL_EMPLOYEE_INFO WHERE STATUS = @STATUS";

            SqlCommand cmd = new SqlCommand(selectData, connect);
            cmd.Parameters.AddWithValue("@STATUS", "active");
            int count = Convert.ToInt32(cmd.ExecuteScalar());

            connect.Close();
            return count;
        }
        public int numberOfInactiveEmployees()
        {
            connect.Open();

            string selectData = "SELECT COUNT(ID) FROM TBL_EMPLOYEE_INFO WHERE STATUS = @STATUS";

            SqlCommand cmd = new SqlCommand(selectData, connect);
            cmd.Parameters.AddWithValue("@STATUS", "inactive");
            int count = Convert.ToInt32(cmd.ExecuteScalar());

            connect.Close();
            return count;
        }
        public int OnTimeData(DateTime date)
        {
            connect.Open();
            string onTime = "SELECT COUNT(*) FROM TBL_ATTENDANCE WHERE STATUS = 'On Time' AND DATE = @DATE";
            SqlCommand cmd = new SqlCommand(onTime, connect);
            cmd.Parameters.AddWithValue("@DATE", date);
            int countOnTime = Convert.ToInt32(cmd.ExecuteScalar());
            connect.Close();

            return countOnTime;
        }
        public int LateData(DateTime date)
        {
            connect.Open();
            string late = "SELECT COUNT(*) FROM TBL_ATTENDANCE WHERE STATUS = 'Late' AND DATE = @DATE";
            SqlCommand com = new SqlCommand(late, connect);
            com.Parameters.AddWithValue("@DATE", date);
            int countLate = Convert.ToInt32(com.ExecuteScalar());
            connect.Close();
            return countLate;
        }
    }
}
