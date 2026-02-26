using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagementSystem
{
    internal class EmployeeDashboardData
    {
        LoginView loginView;
        SqlConnection connect =
            new SqlConnection("Data Source=LAPTOP-0OGAQKFF\\SQLEXPRESS;Initial Catalog=DB_Employee;User ID=ja;Password=john");
        public EmployeeDashboardData(LoginView loginView)
        {
            this.loginView = loginView;
        }
        public int OnTimeData()
        {
            connect.Open();
            string onTime = $"SELECT COUNT(*) FROM TBL_ATTENDANCE WHERE STATUS = 'On Time' AND ACCOUNT_ID = @ACCOUNT_ID AND (YEAR(DATE) = {DateTime.Today.Year} AND MONTH(DATE) = {DateTime.Today.Month})";
            SqlCommand cmd = new SqlCommand(onTime, connect);
            cmd.Parameters.AddWithValue("@ACCOUNT_ID", loginView.ID);
            int countOnTime = Convert.ToInt32(cmd.ExecuteScalar());
            connect.Close();

            return countOnTime;
        }
        public int LateData()
        {
            connect.Open();
            string late = $"SELECT COUNT(*) FROM TBL_ATTENDANCE WHERE STATUS = 'Late' AND ACCOUNT_ID = @ACCOUNT_ID AND (YEAR(DATE) = {DateTime.Today.Year} AND MONTH(DATE) = {DateTime.Today.Month})";
            SqlCommand com = new SqlCommand(late, connect);
            com.Parameters.AddWithValue("@ACCOUNT_ID", loginView.ID);
            int countLate = Convert.ToInt32(com.ExecuteScalar());
            connect.Close();
            return countLate;
        }
        public TimeSpan OvertimeData()
        {
            TimeSpan overtime = TimeSpan.Zero;
            connect.Open();
            string late = "SELECT CONVERT(TIME, DATEADD(SECOND, SUM(DATEDIFF(SECOND, '00:00:00', OVERTIME)), 0)) AS TotalDuration " +
                "FROM TBL_ATTENDANCE " +
                $"WHERE ACCOUNT_ID = @ACCOUNT_ID AND (YEAR(DATE) = {DateTime.Today.Year} AND MONTH(DATE) = {DateTime.Today.Month})";
            SqlCommand com = new SqlCommand(late, connect);
            com.Parameters.AddWithValue("@ACCOUNT_ID", loginView.ID);
            using (SqlDataReader reader = com.ExecuteReader())
            {
                while (reader.Read())
                {
                    overtime = reader.GetTimeSpan(0);
                }
            }
            
            connect.Close();
            return overtime;
        }
    }
}
