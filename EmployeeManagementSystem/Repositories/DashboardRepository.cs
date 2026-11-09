using EmployeeManagementSystem.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace EmployeeManagementSystem
{
    internal class DashboardRepository
    {
        public int OnTimeData(int accountId)
        {
            using (SqlConnection connect = DatabaseConnection.GetConnection())
            {
                connect.Open();

                DateTime startDate = new DateTime(
                    DateTime.Today.Year,
                    DateTime.Today.Month,
                    1
                );

                DateTime endDate = startDate.AddMonths(1);

                string query = @"
                    SELECT COUNT(*)
                    FROM TBL_ATTENDANCE
                    WHERE STATUS = @STATUS
                    AND ACCOUNT_ID = @ACCOUNT_ID
                    AND DATE >= @START_DATE
                    AND DATE < @END_DATE";

                using (SqlCommand cmd = new SqlCommand(query, connect))
                {
                    cmd.Parameters.Add("@STATUS", SqlDbType.VarChar, 50).Value = "On Time";
                    cmd.Parameters.Add("@ACCOUNT_ID", SqlDbType.Int).Value = accountId;
                    cmd.Parameters.Add("@START_DATE", SqlDbType.DateTime).Value = startDate;
                    cmd.Parameters.Add("@END_DATE", SqlDbType.DateTime).Value = endDate;

                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        // Get the total Late attendance records for the current month
        public int LateData(int accountId)
        {
            using (SqlConnection connect = DatabaseConnection.GetConnection())
            {
                connect.Open();

                DateTime startDate = new DateTime(
                    DateTime.Today.Year,
                    DateTime.Today.Month,
                    1
                );

                DateTime endDate = startDate.AddMonths(1);

                string query = @"
                    SELECT COUNT(*)
                    FROM TBL_ATTENDANCE
                    WHERE STATUS = @STATUS
                    AND ACCOUNT_ID = @ACCOUNT_ID
                    AND DATE >= @START_DATE
                    AND DATE < @END_DATE";

                using (SqlCommand cmd = new SqlCommand(query, connect))
                {
                    cmd.Parameters.Add("@STATUS", SqlDbType.VarChar, 50).Value = "Late";
                    cmd.Parameters.Add("@ACCOUNT_ID", SqlDbType.Int).Value = accountId;
                    cmd.Parameters.Add("@START_DATE", SqlDbType.DateTime).Value = startDate;
                    cmd.Parameters.Add("@END_DATE", SqlDbType.DateTime).Value = endDate;

                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        // Get the total overtime duration for the current month
        public TimeSpan OvertimeData(int accountId)
        {
            using (SqlConnection connect = DatabaseConnection.GetConnection())
            {
                connect.Open();

                DateTime startDate = new DateTime(
                    DateTime.Today.Year,
                    DateTime.Today.Month,
                    1
                );

                DateTime endDate = startDate.AddMonths(1);

                string query = @"
                    SELECT ISNULL(
                        SUM(DATEDIFF(SECOND, '00:00:00', OVERTIME)),
                        0
                    )
                    FROM TBL_ATTENDANCE
                    WHERE ACCOUNT_ID = @ACCOUNT_ID
                    AND DATE >= @START_DATE
                    AND DATE < @END_DATE";

                using (SqlCommand cmd = new SqlCommand(query, connect))
                {
                    cmd.Parameters.Add("@ACCOUNT_ID", SqlDbType.Int).Value = accountId;
                    cmd.Parameters.Add("@START_DATE", SqlDbType.DateTime).Value = startDate;
                    cmd.Parameters.Add("@END_DATE", SqlDbType.DateTime).Value = endDate;

                    int totalSeconds = Convert.ToInt32(cmd.ExecuteScalar());

                    return TimeSpan.FromSeconds(totalSeconds);
                }
            }
        }

        public int totalOfEmployees()
        {
            using (SqlConnection connect = DatabaseConnection.GetConnection())
            {
                connect.Open();

                string selectData = "SELECT COUNT(ID) FROM TBL_EMPLOYEE_INFO";

                SqlCommand cmd = new SqlCommand(selectData, connect);
                int count = Convert.ToInt32(cmd.ExecuteScalar());

                connect.Close();
                return count;
            }
        }
        public int numberOfActiveEmployees()
        {
            using (SqlConnection connect = DatabaseConnection.GetConnection())
            {
                connect.Open();

                string selectData = "SELECT COUNT(ID) FROM TBL_EMPLOYEE_INFO WHERE STATUS = @STATUS";

                SqlCommand cmd = new SqlCommand(selectData, connect);
                cmd.Parameters.AddWithValue("@STATUS", "active");
                int count = Convert.ToInt32(cmd.ExecuteScalar());

                connect.Close();
                return count;
            }
        }
        public int numberOfInactiveEmployees()
        {
            using (SqlConnection connect = DatabaseConnection.GetConnection())
            {
                connect.Open();

                string selectData = "SELECT COUNT(ID) FROM TBL_EMPLOYEE_INFO WHERE STATUS = @STATUS";

                SqlCommand cmd = new SqlCommand(selectData, connect);
                cmd.Parameters.AddWithValue("@STATUS", "inactive");
                int count = Convert.ToInt32(cmd.ExecuteScalar());

                connect.Close();
                return count;
            }
        }
        public int OnTimeData(DateTime date)
        {
            using (SqlConnection connect = DatabaseConnection.GetConnection())
            {
                connect.Open();
                string onTime = "SELECT COUNT(*) FROM TBL_ATTENDANCE WHERE STATUS = 'On Time' AND DATE = @DATE";
                SqlCommand cmd = new SqlCommand(onTime, connect);
                cmd.Parameters.AddWithValue("@DATE", date);
                int countOnTime = Convert.ToInt32(cmd.ExecuteScalar());
                connect.Close();

                return countOnTime;
            }
        }
        public int LateData(DateTime date)
        {
            using (SqlConnection connect = DatabaseConnection.GetConnection())
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

        public List<string> DisplayEvents(int userId)
        {
            List<string> events = new List<string>();
            using (SqlConnection connect = DatabaseConnection.GetConnection())
            {
                connect.Open();
                string selectData = "SELECT * FROM TBL_EVENTS INNER JOIN TBL_USERS ON TBL_EVENTS.ACCOUNT_ID = TBL_USERS.ACCOUNT_ID WHERE TBL_EVENTS.EVENT_DATE = @EVENT_DATE AND TBL_USERS.ROLE = 'ADMINISTRATOR'";
                using (SqlCommand cmd = new SqlCommand(selectData, connect))
                {
                    cmd.Parameters.AddWithValue("@ACCOUNT_ID", userId);
                    cmd.Parameters.AddWithValue("@EVENT_DATE", DateTime.Today);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        events.Add("Title: " + reader["TITLE"].ToString());
                        events.Add($"Time: {DateTime.Today.Add(reader.GetTimeSpan(reader.GetOrdinal("START_TIME"))).ToString("hh:mm tt")} - {DateTime.Today.Add(reader.GetTimeSpan(reader.GetOrdinal("END_TIME"))).ToString("hh:mm tt")}");
                        events.Add("");
                    }

                    return events;
                }
            }
        }
        public List<string> DisplayTopFive()
        {
            List<string> top = new List<string>();
            using (SqlConnection connect = DatabaseConnection.GetConnection())
            {
                connect.Open();
                string selectData = "SELECT DISTINCT DATE, FULL_NAME FROM (" +
                    "SELECT TBL_ATTENDANCE.DATE,TBL_EMPLOYEE_INFO.FULL_NAME,ROW_NUMBER() OVER (ORDER BY TBL_ATTENDANCE.DATE) AS RowNum FROM TBL_EMPLOYEE_INFO INNER JOIN TBL_USERS ON TBL_EMPLOYEE_INFO.ID = TBL_USERS.ID INNER JOIN TBL_ATTENDANCE ON TBL_USERS.ACCOUNT_ID = TBL_ATTENDANCE.ACCOUNT_ID WHERE TBL_ATTENDANCE.DATE = @DATE AND TBL_ATTENDANCE.STATUS = 'On Time'" +
                    ")AS RowConstrainedResult " +
                    "WHERE RowNum >= 1 AND RowNum <= 5 " +
                    "ORDER BY DATE";
                using (SqlCommand cmd = new SqlCommand(selectData, connect))
                {
                    cmd.Parameters.AddWithValue("@DATE", DateTime.Today);
                    SqlDataReader reader = cmd.ExecuteReader();
                    int i = 1;
                    while (reader.Read())
                    {
                        top.Add($"{i}: " + reader["FULL_NAME"].ToString());
                        i++;
                    }
                }
            }
            return top;
        }
    }
}
