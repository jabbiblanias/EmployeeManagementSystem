using EmployeeManagementSystem.Data;
using EmployeeManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace EmployeeManagementSystem.Repositories
{
    internal class EventRepository
    {
        // CHECK DUPLICATE EVENT TITLE
        public bool TitleExists(string title, DateTime eventDate)
        {
            using (SqlConnection connection = DatabaseConnection.GetConnection())
            {
                connection.Open();

                string query = @"
                    SELECT COUNT(*)
                    FROM TBL_EVENTS
                    WHERE TITLE = @TITLE
                    AND EVENT_DATE = @EVENT_DATE";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add("@TITLE", SqlDbType.NVarChar, 255).Value = title;
                    command.Parameters.Add("@EVENT_DATE", SqlDbType.Date).Value = eventDate.Date;

                    return Convert.ToInt32(command.ExecuteScalar()) > 0;
                }
            }
        }

        // CHECK DUPLICATE TITLE WHEN UPDATING
        public bool TitleExistsForUpdate(string title, string originalTitle, DateTime eventDate, int accountId)
        {
            using (SqlConnection connection = DatabaseConnection.GetConnection())
            {
                connection.Open();

                string query = @"
                    SELECT COUNT(*)
                    FROM TBL_EVENTS
                    WHERE TITLE = @TITLE
                    AND EVENT_DATE = @EVENT_DATE
                    AND NOT (
                        TITLE = @ORIGINAL_TITLE
                        AND ACCOUNT_ID = @ACCOUNT_ID
                    )";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add("@TITLE", SqlDbType.NVarChar, 255).Value = title;
                    command.Parameters.Add("@ORIGINAL_TITLE", SqlDbType.NVarChar, 255).Value = originalTitle;
                    command.Parameters.Add("@EVENT_DATE", SqlDbType.Date).Value = eventDate.Date;
                    command.Parameters.Add("@ACCOUNT_ID", SqlDbType.Int).Value = accountId;

                    return Convert.ToInt32(command.ExecuteScalar()) > 0;
                }
            }
        }

        // GET EVENT TITLES FOR A PARTICULAR DATE AND USER
        public List<string> GetEventTitles(DateTime eventDate, int accountId)
        {
            List<string> titles = new List<string>();

            using (SqlConnection connection = DatabaseConnection.GetConnection())
            {
                connection.Open();

                string query = @"
                    SELECT TITLE
                    FROM TBL_EVENTS
                    WHERE EVENT_DATE = @EVENT_DATE
                    AND ACCOUNT_ID = @ACCOUNT_ID
                    ORDER BY TITLE";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add("@EVENT_DATE", SqlDbType.Date).Value = eventDate.Date;
                    command.Parameters.Add("@ACCOUNT_ID", SqlDbType.Int).Value = accountId;

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (!reader.IsDBNull(0))
                                titles.Add(reader.GetString(0));
                        }
                    }
                }
            }

            return titles;
        }

        // GET EVENT DETAILS
        public Event GetEvent(string title, DateTime eventDate, int accountId)
        {
            using (SqlConnection connection = DatabaseConnection.GetConnection())
            {
                connection.Open();

                string query = @"
                    SELECT TITLE, DESCRIPTION, CATEGORY, LOCATION, START_TIME, END_TIME
                    FROM TBL_EVENTS
                    WHERE TITLE = @TITLE
                    AND EVENT_DATE = @EVENT_DATE
                    AND ACCOUNT_ID = @ACCOUNT_ID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add("@TITLE", SqlDbType.NVarChar, 255).Value = title;
                    command.Parameters.Add("@EVENT_DATE", SqlDbType.Date).Value = eventDate.Date;
                    command.Parameters.Add("@ACCOUNT_ID", SqlDbType.Int).Value = accountId;

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Event
                            {
                                Title = reader.IsDBNull(0) ? string.Empty : reader.GetString(0),
                                Description = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                                Category = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                                Location = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                                StartTime = reader.IsDBNull(4) ? TimeSpan.Zero : reader.GetTimeSpan(4),
                                EndTime = reader.IsDBNull(5) ? TimeSpan.Zero : reader.GetTimeSpan(5)
                            };
                        }
                    }
                }
            }

            return null;
        }

        // ADD EVENT
        public bool AddEvent(string title, string description, string location, DateTime eventDate, TimeSpan startTime, TimeSpan endTime, int accountId)
        {
            using (SqlConnection connection = DatabaseConnection.GetConnection())
            {
                connection.Open();

                string query = @"
                    INSERT INTO TBL_EVENTS
                    (TITLE, DESCRIPTION, CATEGORY, LOCATION, EVENT_DATE,
                     START_TIME, END_TIME, ACCOUNT_ID)
                    VALUES
                    (@TITLE, @DESCRIPTION, @CATEGORY, @LOCATION, @EVENT_DATE,
                     @START_TIME, @END_TIME, @ACCOUNT_ID)";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add("@TITLE", SqlDbType.NVarChar, 255).Value = title;
                    command.Parameters.Add("@DESCRIPTION", SqlDbType.NVarChar, -1).Value = description;
                    command.Parameters.Add("@CATEGORY", SqlDbType.NVarChar, 255).Value = "Personal";
                    command.Parameters.Add("@LOCATION", SqlDbType.NVarChar, 255).Value = location;
                    command.Parameters.Add("@EVENT_DATE", SqlDbType.Date).Value = eventDate.Date;
                    command.Parameters.Add("@START_TIME", SqlDbType.Time).Value = startTime;
                    command.Parameters.Add("@END_TIME", SqlDbType.Time).Value = endTime;
                    command.Parameters.Add("@ACCOUNT_ID", SqlDbType.Int).Value = accountId;

                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool AddEvent(string title, string description, string category, string location, DateTime eventDate, TimeSpan startTime, TimeSpan endTime, int accountId)
        {
            using (SqlConnection connection = DatabaseConnection.GetConnection())
            {
                connection.Open();

                string query = @"
                    INSERT INTO TBL_EVENTS
                    (TITLE, DESCRIPTION, CATEGORY, LOCATION, EVENT_DATE,
                     START_TIME, END_TIME, ACCOUNT_ID)
                    VALUES
                    (@TITLE, @DESCRIPTION, @CATEGORY, @LOCATION, @EVENT_DATE,
                     @START_TIME, @END_TIME, @ACCOUNT_ID)";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add("@TITLE", SqlDbType.NVarChar, 255).Value = title;
                    command.Parameters.Add("@DESCRIPTION", SqlDbType.NVarChar, -1).Value = description;
                    command.Parameters.Add("@CATEGORY", SqlDbType.NVarChar, 255).Value = category;
                    command.Parameters.Add("@LOCATION", SqlDbType.NVarChar, 255).Value = location;
                    command.Parameters.Add("@EVENT_DATE", SqlDbType.Date).Value = eventDate.Date;
                    command.Parameters.Add("@START_TIME", SqlDbType.Time).Value = startTime;
                    command.Parameters.Add("@END_TIME", SqlDbType.Time).Value = endTime;
                    command.Parameters.Add("@ACCOUNT_ID", SqlDbType.Int).Value = accountId;

                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        // UPDATE EVENT
        public bool UpdateEvent(string originalTitle, string newTitle, string description, string location, DateTime eventDate, TimeSpan startTime, TimeSpan endTime, int accountId)
        {
            using (SqlConnection connection = DatabaseConnection.GetConnection())
            {
                connection.Open();

                string query = @"
                    UPDATE TBL_EVENTS
                    SET TITLE = @TITLE,
                        DESCRIPTION = @DESCRIPTION,
                        LOCATION = @LOCATION,
                        START_TIME = @START_TIME,
                        END_TIME = @END_TIME
                    WHERE TITLE = @ORIGINAL_TITLE
                    AND EVENT_DATE = @EVENT_DATE
                    AND ACCOUNT_ID = @ACCOUNT_ID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add("@TITLE", SqlDbType.NVarChar, 255).Value = newTitle;
                    command.Parameters.Add("@DESCRIPTION", SqlDbType.NVarChar, -1).Value = description;
                    command.Parameters.Add("@LOCATION", SqlDbType.NVarChar, 255).Value = location;
                    command.Parameters.Add("@START_TIME", SqlDbType.Time).Value = startTime;
                    command.Parameters.Add("@END_TIME", SqlDbType.Time).Value = endTime;
                    command.Parameters.Add("@ORIGINAL_TITLE", SqlDbType.NVarChar, 255).Value = originalTitle;
                    command.Parameters.Add("@EVENT_DATE", SqlDbType.Date).Value = eventDate.Date;
                    command.Parameters.Add("@ACCOUNT_ID", SqlDbType.Int).Value = accountId;

                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool UpdateEvent(string originalTitle, string newTitle, string description, string category, string location, DateTime eventDate, TimeSpan startTime, TimeSpan endTime, int accountId)
        {
            using (SqlConnection connection = DatabaseConnection.GetConnection())
            {
                connection.Open();

                string query = @"
                    UPDATE TBL_EVENTS
                    SET TITLE = @TITLE,
                        DESCRIPTION = @DESCRIPTION,
                        CATEGORY = @CATEGORY,
                        LOCATION = @LOCATION,
                        START_TIME = @START_TIME,
                        END_TIME = @END_TIME
                    WHERE TITLE = @ORIGINAL_TITLE
                    AND EVENT_DATE = @EVENT_DATE
                    AND ACCOUNT_ID = @ACCOUNT_ID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add("@TITLE", SqlDbType.NVarChar, 255).Value = newTitle;
                    command.Parameters.Add("@DESCRIPTION", SqlDbType.NVarChar, -1).Value = description;
                    command.Parameters.Add("@CATEGORY", SqlDbType.NVarChar, 255).Value = category;
                    command.Parameters.Add("@LOCATION", SqlDbType.NVarChar, 255).Value = location;
                    command.Parameters.Add("@START_TIME", SqlDbType.Time).Value = startTime;
                    command.Parameters.Add("@END_TIME", SqlDbType.Time).Value = endTime;
                    command.Parameters.Add("@ORIGINAL_TITLE", SqlDbType.NVarChar, 255).Value = originalTitle;
                    command.Parameters.Add("@EVENT_DATE", SqlDbType.Date).Value = eventDate.Date;
                    command.Parameters.Add("@ACCOUNT_ID", SqlDbType.Int).Value = accountId;

                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        // DELETE EVENT
        public bool DeleteEvent(string title, DateTime eventDate, int accountId)
        {
            using (SqlConnection connection = DatabaseConnection.GetConnection())
            {
                connection.Open();

                string query = @"
                    DELETE FROM TBL_EVENTS
                    WHERE TITLE = @TITLE
                    AND EVENT_DATE = @EVENT_DATE
                    AND ACCOUNT_ID = @ACCOUNT_ID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add("@TITLE", SqlDbType.NVarChar, 255).Value = title;
                    command.Parameters.Add("@EVENT_DATE", SqlDbType.Date).Value = eventDate.Date;
                    command.Parameters.Add("@ACCOUNT_ID", SqlDbType.Int).Value = accountId;

                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        public List<Event> GetEventsForDate(
            DateTime eventDate,
            int accountId)
        {
            List<Event> events = new List<Event>();

            string query = @"
                SELECT
                    E.TITLE,
                    E.DESCRIPTION,
                    E.CATEGORY,
                    E.LOCATION,
                    E.EVENT_DATE,
                    E.START_TIME,
                    E.END_TIME
                FROM TBL_EVENTS E
                INNER JOIN TBL_USERS U
                    ON E.ACCOUNT_ID = U.ACCOUNT_ID
                WHERE E.EVENT_DATE = @EVENT_DATE
                  AND (E.ACCOUNT_ID = @ACCOUNT_ID
                       OR U.ROLE = 'ADMINISTRATOR')
                ORDER BY E.START_TIME";

            using (SqlConnection connection = DatabaseConnection.GetConnection())
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.Add("@EVENT_DATE", SqlDbType.Date).Value =
                    eventDate.Date;

                command.Parameters.Add("@ACCOUNT_ID", SqlDbType.Int).Value =
                    accountId;

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        events.Add(new Event
                        {
                            Title = Convert.ToString(reader["TITLE"]),
                            Description = Convert.ToString(reader["DESCRIPTION"]),
                            Category = Convert.ToString(reader["CATEGORY"]),
                            Location = Convert.ToString(reader["LOCATION"]),
                            EventDate = Convert.ToDateTime(reader["EVENT_DATE"]),
                            StartTime = (TimeSpan)reader["START_TIME"],
                            EndTime = (TimeSpan)reader["END_TIME"]
                        });
                    }
                }
            }

            return events;
        }

        public List<Event> GetEventsForMonth(
            DateTime startDate,
            DateTime endDate,
            int accountId)
        {
            List<Event> events = new List<Event>();

            string query = @"
                SELECT
                    E.TITLE,
                    E.CATEGORY,
                    E.EVENT_DATE
                FROM TBL_EVENTS E
                INNER JOIN TBL_USERS U
                    ON E.ACCOUNT_ID = U.ACCOUNT_ID
                WHERE E.EVENT_DATE >= @START_DATE
                  AND E.EVENT_DATE < @END_DATE
                  AND (E.ACCOUNT_ID = @ACCOUNT_ID
                       OR U.ROLE = 'ADMINISTRATOR')
                ORDER BY E.EVENT_DATE, E.CATEGORY";

            using (SqlConnection connection = DatabaseConnection.GetConnection())
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.Add("@START_DATE", SqlDbType.Date).Value =
                    startDate.Date;

                command.Parameters.Add("@END_DATE", SqlDbType.Date).Value =
                    endDate.Date;

                command.Parameters.Add("@ACCOUNT_ID", SqlDbType.Int).Value =
                    accountId;

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        events.Add(new Event
                        {
                            Title = Convert.ToString(reader["TITLE"]),
                            Category = Convert.ToString(reader["CATEGORY"]),
                            EventDate = Convert.ToDateTime(reader["EVENT_DATE"])
                        });
                    }
                }
            }

            return events;
        }
    }
}