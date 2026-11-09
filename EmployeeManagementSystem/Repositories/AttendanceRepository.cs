using EmployeeManagementSystem.Data;
using EmployeeManagementSystem.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;


namespace EmployeeManagementSystem.Repositories
{
    public class AttendanceRepository
    {
        public bool ClockIn(int accountId, DateTime clockIn, string status)
        {
            using (SqlConnection connection = DatabaseConnection.GetConnection())
            {
                connection.Open();

                string query = @"
            INSERT INTO TBL_ATTENDANCE
            ([DATE], CLOCK_IN, STATUS, ACCOUNT_ID)
            SELECT @DATE, @CLOCK_IN, @STATUS, @ACCOUNT_ID
            WHERE NOT EXISTS
            (
                SELECT 1
                FROM TBL_ATTENDANCE WITH (UPDLOCK, HOLDLOCK)
                WHERE [DATE] = @DATE
                AND ACCOUNT_ID = @ACCOUNT_ID
            )";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add("@DATE", SqlDbType.Date).Value =
                        clockIn.Date;

                    command.Parameters.Add("@CLOCK_IN", SqlDbType.Time).Value =
                        TimeSpan.FromMinutes(
                            Math.Floor(clockIn.TimeOfDay.TotalMinutes)
                        );

                    command.Parameters.Add("@STATUS", SqlDbType.VarChar, 50).Value =
                        status;

                    command.Parameters.Add("@ACCOUNT_ID", SqlDbType.Int).Value =
                        accountId;

                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        // CLOCK OUT
        public bool ClockOut(int accountId, DateTime clockOut, TimeSpan overtime)
        {
            using (SqlConnection connection = DatabaseConnection.GetConnection())
            {
                connection.Open();

                string query = @"
            UPDATE TBL_ATTENDANCE
            SET CLOCK_OUT = @CLOCK_OUT,
                OVERTIME = @OVERTIME
            WHERE ACCOUNT_ID = @ACCOUNT_ID
            AND [DATE] = @DATE
            AND CLOCK_OUT IS NULL
            AND CLOCK_IN <= @CLOCK_OUT";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add("@DATE", SqlDbType.Date).Value =
                        clockOut.Date;

                    command.Parameters.Add("@CLOCK_OUT", SqlDbType.Time).Value =
                        TimeSpan.FromMinutes(
                            Math.Floor(clockOut.TimeOfDay.TotalMinutes)
                        );

                    command.Parameters.Add("@OVERTIME", SqlDbType.Time).Value =
                        TimeSpan.FromMinutes(
                            Math.Floor(overtime.TotalMinutes)
                        );

                    command.Parameters.Add("@ACCOUNT_ID", SqlDbType.Int).Value =
                        accountId;

                    return command.ExecuteNonQuery() > 0;
                }
            }
        }
        public List<Attendance> employeeAttendanceListData(User user)
        {
            List<Attendance> listdata = new List<Attendance>();

            using (SqlConnection connect = DatabaseConnection.GetConnection())
            {
                connect.Open();

                string selectData = @"
        SELECT [DATE], CLOCK_IN, CLOCK_OUT, STATUS, OVERTIME
        FROM TBL_ATTENDANCE
        WHERE ACCOUNT_ID = @ACCOUNT_ID
        ORDER BY [DATE] DESC";

                using (SqlCommand cmd = new SqlCommand(selectData, connect))
                {
                    cmd.Parameters.Add("@ACCOUNT_ID", SqlDbType.Int).Value = user.ID;

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Attendance attendance = new Attendance();

                            attendance.Date = reader.GetDateTime(
                                reader.GetOrdinal("DATE"));

                            attendance.ClockIn = reader.IsDBNull(reader.GetOrdinal("CLOCK_IN"))
                                ? (TimeSpan?)null
                                : reader.GetTimeSpan(reader.GetOrdinal("CLOCK_IN"));

                            attendance.ClockOut = reader.IsDBNull(reader.GetOrdinal("CLOCK_OUT"))
                                ? (TimeSpan?)null
                                : reader.GetTimeSpan(reader.GetOrdinal("CLOCK_OUT"));

                            attendance.Status = reader["STATUS"].ToString();

                            attendance.Overtime = reader.IsDBNull(reader.GetOrdinal("OVERTIME"))
                                ? (TimeSpan?)null
                                : reader.GetTimeSpan(reader.GetOrdinal("OVERTIME"));

                            listdata.Add(attendance);
                        }
                    }
                }
            }

            return listdata;
        }

        public List<Attendance> currentDayAttendanceListData(string date, User user)
        {
            List<Attendance> listdata = new List<Attendance>();

            using (SqlConnection connect = DatabaseConnection.GetConnection())
            {
                connect.Open();

                string selectData = "SELECT DATE, CLOCK_IN, CLOCK_OUT, STATUS, OVERTIME FROM " +
                    "TBL_ATTENDANCE WHERE ACCOUNT_ID = @ACCOUNT_ID AND DATE = @DATE; ";

                using (SqlCommand cmd = new SqlCommand(selectData, connect))
                {
                    cmd.Parameters.AddWithValue("@ACCOUNT_ID", user.ID);
                    cmd.Parameters.AddWithValue("@DATE", date);
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Attendance attendance = new Attendance();
                        attendance.Date = (DateTime)reader["DATE"];
                        if (!reader.IsDBNull(reader.GetOrdinal("CLOCK_IN")))
                        {
                            attendance.ClockIn = (TimeSpan)reader["CLOCK_IN"];
                        }

                        if (!reader.IsDBNull(reader.GetOrdinal("CLOCK_OUT")))
                        {
                            attendance.ClockOut = (TimeSpan)reader["CLOCK_OUT"];
                        }
                        attendance.Status = reader["STATUS"].ToString();
                        attendance.Overtime = reader.IsDBNull(reader.GetOrdinal("OVERTIME"))
                            ? (TimeSpan?)null
                            : (TimeSpan)reader["OVERTIME"];

                        listdata.Add(attendance);
                    }
                }
            }
            return listdata;
        }
        public List<Attendance> MonthYearAttendanceListData(int fromMonth, int fromYear, int toMonth, int toYear, User user)
        {
            List<Attendance> listdata = new List<Attendance>();

            using (SqlConnection connect = DatabaseConnection.GetConnection())
            {
                connect.Open();

                string selectData = "SELECT DATE, CLOCK_IN, CLOCK_OUT,STATUS, OVERTIME FROM TBL_ATTENDANCE " +
                    "WHERE ACCOUNT_ID = @ACCOUNT_ID AND (YEAR(DATE) >= @StartYear AND MONTH(DATE) >= @StartMonth) " +
                    "AND (YEAR(DATE) <= @EndYear AND MONTH(DATE) <= @EndMonth) " +
                    "ORDER BY DATE ASC";

                using (SqlCommand cmd = new SqlCommand(selectData, connect))
                {
                    cmd.Parameters.AddWithValue("@ACCOUNT_ID", user.ID);
                    cmd.Parameters.AddWithValue("@StartMonth", fromMonth);
                    cmd.Parameters.AddWithValue("@StartYear", fromYear);
                    cmd.Parameters.AddWithValue("@EndMonth", toMonth);
                    cmd.Parameters.AddWithValue("@EndYear", toYear);
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Attendance attendance = new Attendance();
                        attendance.Date = (DateTime)reader["DATE"];
                        if (!reader.IsDBNull(reader.GetOrdinal("CLOCK_IN")))
                        {
                            attendance.ClockIn = (TimeSpan)reader["CLOCK_IN"];
                        }

                        if (!reader.IsDBNull(reader.GetOrdinal("CLOCK_OUT")))
                        {
                            attendance.ClockOut = (TimeSpan)reader["CLOCK_OUT"];
                        }
                        attendance.Status = reader["STATUS"].ToString();
                        attendance.Overtime = reader.IsDBNull(reader.GetOrdinal("OVERTIME"))
                            ? (TimeSpan?)null
                            : (TimeSpan)reader["OVERTIME"];

                        listdata.Add(attendance);
                    }
                }
            }
            return listdata;
        }

        public List<Attendance> employeesAttendanceListData(int entry)
        {
            int endRow = 10 * entry;
            int startRow = endRow - 9;
            List<Attendance> listdata = new List<Attendance>();

            using (SqlConnection connect = DatabaseConnection.GetConnection())
            {
                connect.Open();

                string selectData = $@"
                        SELECT DATE, CLOCK_IN, CLOCK_OUT, STATUS, OVERTIME, EMPLOYEE_ID, FULL_NAME, PROFESSION 
                        FROM (
                            SELECT 
                                TBL_ATTENDANCE.DATE, 
                                TBL_ATTENDANCE.CLOCK_IN, 
                                TBL_ATTENDANCE.CLOCK_OUT, 
                                TBL_ATTENDANCE.STATUS, 
                                TBL_ATTENDANCE.OVERTIME, 
                                TBL_EMPLOYEE_INFO.EMPLOYEE_ID, 
                                TBL_EMPLOYEE_INFO.FULL_NAME, 
                                TBL_EMPLOYEE_INFO.PROFESSION, 
                                ROW_NUMBER() OVER (ORDER BY TBL_ATTENDANCE.DATE DESC) AS RowNum 
                            FROM TBL_ATTENDANCE 
                            INNER JOIN TBL_USERS ON TBL_ATTENDANCE.ACCOUNT_ID = TBL_USERS.ACCOUNT_ID 
                            INNER JOIN TBL_EMPLOYEE_INFO ON TBL_USERS.ID = TBL_EMPLOYEE_INFO.ID
                        ) AS RowConstrainedResult 
                        WHERE RowNum >= {startRow} AND RowNum <= {endRow} 
                        ORDER BY DATE DESC";

                using (SqlCommand cmd = new SqlCommand(selectData, connect))
                {
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Attendance aad = new Attendance();
                        aad.EmployeeID = reader["EMPLOYEE_ID"].ToString();
                        aad.Name = reader["FULL_NAME"].ToString();
                        aad.Profession = reader.GetString(reader.GetOrdinal("PROFESSION"));
                        aad.Date = (DateTime)reader["DATE"];
                        if (!reader.IsDBNull(reader.GetOrdinal("CLOCK_IN")))
                        {
                            aad.ClockIn = (TimeSpan)reader["CLOCK_IN"];
                        }

                        if (!reader.IsDBNull(reader.GetOrdinal("CLOCK_OUT")))
                        {
                            aad.ClockOut = (TimeSpan)reader["CLOCK_OUT"];
                        }
                        aad.Status = reader["STATUS"].ToString();
                        aad.Overtime = reader.IsDBNull(reader.GetOrdinal("OVERTIME"))
                            ? (TimeSpan?)null
                            : (TimeSpan)reader["OVERTIME"];

                        listdata.Add(aad);
                    }
                }
            }
            return listdata;
        }
        public List<Attendance> SearchEmployeeAttendanceListData(int entry, string name)
        {
            int endRow = 10 * entry;
            int startRow = endRow - 9;
            List<Attendance> listdata = new List<Attendance>();

            using (SqlConnection connect = DatabaseConnection.GetConnection())
            {
                connect.Open();
                string selectData = $@"
                        SELECT FULL_NAME,DATE, CLOCK_IN, CLOCK_OUT, STATUS, OVERTIME, EMPLOYEE_ID,  PROFESSION 
                        FROM (
                            SELECT 
                                TBL_ATTENDANCE.DATE, 
                                TBL_ATTENDANCE.CLOCK_IN, 
                                TBL_ATTENDANCE.CLOCK_OUT, 
                                TBL_ATTENDANCE.STATUS, 
                                TBL_ATTENDANCE.OVERTIME, 
                                TBL_EMPLOYEE_INFO.EMPLOYEE_ID, 
                                TBL_EMPLOYEE_INFO.FULL_NAME, 
                                TBL_EMPLOYEE_INFO.PROFESSION, 
                                ROW_NUMBER() OVER (ORDER BY TBL_ATTENDANCE.DATE DESC) AS RowNum 
                            FROM TBL_ATTENDANCE 
                            INNER JOIN TBL_USERS ON TBL_ATTENDANCE.ACCOUNT_ID = TBL_USERS.ACCOUNT_ID 
                            INNER JOIN TBL_EMPLOYEE_INFO ON TBL_USERS.ID = TBL_EMPLOYEE_INFO.ID
                            WHERE TBL_EMPLOYEE_INFO.FULL_NAME = @FULL_NAME
                        ) AS RowConstrainedResult 
                        WHERE RowNum >= {startRow} AND RowNum <= {endRow}
                        ORDER BY DATE DESC";

                using (SqlCommand cmd = new SqlCommand(selectData, connect))
                {
                    cmd.Parameters.AddWithValue("@FULL_NAME", name);
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Attendance aad = new Attendance();
                        aad.EmployeeID = reader["EMPLOYEE_ID"].ToString();
                        aad.Name = reader["FULL_NAME"].ToString();
                        aad.Profession = reader["PROFESSION"].ToString();
                        aad.Date = (DateTime)reader["DATE"];
                        if (!reader.IsDBNull(reader.GetOrdinal("CLOCK_IN")))
                        {
                            aad.ClockIn = (TimeSpan)reader["CLOCK_IN"];
                        }

                        if (!reader.IsDBNull(reader.GetOrdinal("CLOCK_OUT")))
                        {
                            aad.ClockOut = (TimeSpan)reader["CLOCK_OUT"];
                        }
                        aad.Status = reader["STATUS"].ToString();
                        aad.Overtime = reader.IsDBNull(reader.GetOrdinal("OVERTIME"))
                            ? (TimeSpan?)null
                            : (TimeSpan)reader["OVERTIME"];

                        listdata.Add(aad);
                    }
                }
            }
            return listdata;
        }
        public int recentNumberOfEntries()
        {
            using (SqlConnection connect = DatabaseConnection.GetConnection())
            {
                connect.Open();
                string query = @"SELECT CEILING(COUNT(*) / 10.0) AS NumberOfGroups FROM TBL_ATTENDANCE";
                SqlCommand com = new SqlCommand(query, connect);
                int entries = Convert.ToInt32(com.ExecuteScalar());

                return entries;
            }
        }
        public int searchNumberOfEntries(string name)
        {
            using (SqlConnection connect = DatabaseConnection.GetConnection())
            {
                connect.Open();
                string query = @"SELECT CEILING(COUNT(*) / 10.0) AS NumberOfGroups FROM TBL_ATTENDANCE INNER JOIN TBL_USERS ON TBL_ATTENDANCE.ACCOUNT_ID = TBL_USERS.ACCOUNT_ID 
                            INNER JOIN TBL_EMPLOYEE_INFO ON TBL_USERS.ID = TBL_EMPLOYEE_INFO.ID WHERE FULL_NAME = @FULL_NAME";
                SqlCommand com = new SqlCommand(query, connect);
                com.Parameters.AddWithValue("@FULL_NAME", name);
                int entries = Convert.ToInt32(com.ExecuteScalar());

                return entries;
            }
        }
        public int filterNumberOfEntries(string profession, string status, DateTime dateFrom, DateTime dateTo)
        {
            using (SqlConnection connect = DatabaseConnection.GetConnection())
            {
                connect.Open();
                string query = @"SELECT CEILING(COUNT(*) / 10.0) AS NumberOfGroups FROM TBL_ATTENDANCE 
                            INNER JOIN TBL_USERS ON TBL_ATTENDANCE.ACCOUNT_ID = TBL_USERS.ACCOUNT_ID 
                            INNER JOIN TBL_EMPLOYEE_INFO ON TBL_USERS.ID = TBL_EMPLOYEE_INFO.ID
                            WHERE 
                                (TBL_ATTENDANCE.DATE >= @StartDate AND TBL_ATTENDANCE.DATE <= @EndDate) 
		                        AND (@STATUS = 'All Status' OR TBL_ATTENDANCE.STATUS = @STATUS AND @PROFESSION = 'All Profession' OR TBL_EMPLOYEE_INFO.PROFESSION = @PROFESSION)";
                SqlCommand cmd = new SqlCommand(query, connect);
                cmd.Parameters.AddWithValue("@StartDate", dateFrom);
                cmd.Parameters.AddWithValue("@EndDate", dateTo);
                cmd.Parameters.AddWithValue("@STATUS", status);
                cmd.Parameters.AddWithValue("@PROFESSION", profession);
                int entries = Convert.ToInt32(cmd.ExecuteScalar());

                return entries;
            }
        }

        public List<Attendance> filterEmployeesAttendanceListData(int entry, string profession, string status, DateTime dateFrom, DateTime dateTo)
        {
            int endRow = 10 * entry;
            int startRow = endRow - 9;
            List<Attendance> listdata = new List<Attendance>();

            using (SqlConnection connect = DatabaseConnection.GetConnection())
            {
                connect.Open();

                string selectData = $@"
                        SELECT DATE, CLOCK_IN, CLOCK_OUT, STATUS, OVERTIME, EMPLOYEE_ID, FULL_NAME, PROFESSION 
                        FROM (
                            SELECT 
                                TBL_ATTENDANCE.DATE, 
                                TBL_ATTENDANCE.CLOCK_IN, 
                                TBL_ATTENDANCE.CLOCK_OUT, 
                                TBL_ATTENDANCE.STATUS, 
                                TBL_ATTENDANCE.OVERTIME, 
                                TBL_EMPLOYEE_INFO.EMPLOYEE_ID, 
                                TBL_EMPLOYEE_INFO.FULL_NAME, 
                                TBL_EMPLOYEE_INFO.PROFESSION, 
                                ROW_NUMBER() OVER (ORDER BY TBL_ATTENDANCE.DATE DESC) AS RowNum 
                            FROM TBL_ATTENDANCE 
                            INNER JOIN TBL_USERS ON TBL_ATTENDANCE.ACCOUNT_ID = TBL_USERS.ACCOUNT_ID 
                            INNER JOIN TBL_EMPLOYEE_INFO ON TBL_USERS.ID = TBL_EMPLOYEE_INFO.ID
                            WHERE 
                                (TBL_ATTENDANCE.DATE >= @StartDate AND TBL_ATTENDANCE.DATE <= @EndDate) 
		                        AND (@STATUS = 'All Status' OR TBL_ATTENDANCE.STATUS = @STATUS) AND (@PROFESSION = 'All Profession' OR TBL_EMPLOYEE_INFO.PROFESSION = @PROFESSION)
                        ) AS RowConstrainedResult 
                        WHERE  
                        RowNum >= {startRow} AND RowNum <= {endRow}
                        ORDER BY DATE DESC";

                using (SqlCommand cmd = new SqlCommand(selectData, connect))
                {
                    cmd.Parameters.AddWithValue("@StartDate", dateFrom);
                    cmd.Parameters.AddWithValue("@EndDate", dateTo);
                    cmd.Parameters.AddWithValue("@STATUS", status);
                    cmd.Parameters.AddWithValue("@PROFESSION", profession);

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Attendance aad = new Attendance();
                        aad.EmployeeID = reader["EMPLOYEE_ID"].ToString();
                        aad.Name = reader["FULL_NAME"].ToString();
                        aad.Profession = reader.GetString(reader.GetOrdinal("PROFESSION"));
                        aad.Date = (DateTime)reader["DATE"];
                        if (!reader.IsDBNull(reader.GetOrdinal("CLOCK_IN")))
                        {
                            aad.ClockIn = (TimeSpan)reader["CLOCK_IN"];
                        }

                        if (!reader.IsDBNull(reader.GetOrdinal("CLOCK_OUT")))
                        {
                            aad.ClockOut = (TimeSpan)reader["CLOCK_OUT"];
                        }
                        aad.Status = reader["STATUS"].ToString();
                        aad.Overtime = reader.IsDBNull(reader.GetOrdinal("OVERTIME"))
                            ? (TimeSpan?)null
                            : (TimeSpan)reader["OVERTIME"];

                        listdata.Add(aad);
                    }
                }
            }
            return listdata;
        }

        public Attendance AttendanceClock(int accountId)
        {
            using (SqlConnection connection = DatabaseConnection.GetConnection())
            {
                connection.Open();
                string query = "SELECT CLOCK_IN, CLOCK_OUT FROM TBL_ATTENDANCE WHERE DATE = @DATE AND ACCOUNT_ID = @ACCOUNT_ID";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add("@ACCOUNT_ID", SqlDbType.Int).Value =
                        accountId;

                    command.Parameters.Add("@DATE", SqlDbType.Date).Value =
                        DateTime.Today;

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Attendance
                            {
                                ClockIn = reader.IsDBNull(reader.GetOrdinal("CLOCK_IN"))
                                    ? (TimeSpan?)null
                                    : reader.GetTimeSpan(reader.GetOrdinal("CLOCK_IN")),

                                ClockOut = reader.IsDBNull(reader.GetOrdinal("CLOCK_OUT"))
                                    ? (TimeSpan?)null
                                    : reader.GetTimeSpan(reader.GetOrdinal("CLOCK_OUT"))
                            };
                        }
                    }
                }
            }
            return null;
        }

        public AutoCompleteStringCollection employeeNamesData()
        {
            AutoCompleteStringCollection employeeNames = new AutoCompleteStringCollection();
            using (SqlConnection connect = DatabaseConnection.GetConnection())
            {
                connect.Open();

                string selectData = "SELECT FULL_NAME FROM TBL_EMPLOYEE_INFO";

                using (SqlCommand cmd = new SqlCommand(selectData, connect))
                {
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Attendance aad = new Attendance();
                        aad.Name = reader.GetString(0);

                        employeeNames.Add(aad.Name);
                    }
                }
            }
            return employeeNames;
        }
    }
}
