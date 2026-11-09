using EmployeeManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using EmployeeManagementSystem.Repositories;

namespace EmployeeManagementSystem.Services
{
    public class AttendanceService
    {
        private readonly AttendanceRepository _attendanceRepository;

        public AttendanceService()
        {
            _attendanceRepository = new AttendanceRepository();
        }

        public string ClockIn(int accountId, DateTime currentDateTime)
        {
            if (accountId <= 0)
                throw new InvalidOperationException("Invalid employee account.");

            // Check if today is Saturday or Sunday.
            if (currentDateTime.DayOfWeek == DayOfWeek.Saturday ||
                currentDateTime.DayOfWeek == DayOfWeek.Sunday)
            {
                throw new InvalidOperationException(
                    "There's currently no work for today.");
            }

            TimeSpan timeStart = new TimeSpan(7, 30, 0);

            string status = currentDateTime.TimeOfDay <= timeStart
                ? "On Time"
                : "Late";

            bool isClockedIn = _attendanceRepository.ClockIn(
                accountId,
                currentDateTime,
                status
            );

            if (!isClockedIn)
            {
                throw new InvalidOperationException(
                    "You already have an attendance record for today.");
            }

            return status;
        }

        // CLOCK OUT
        public TimeSpan ClockOut(int accountId, DateTime currentDateTime)
        {
            if (accountId <= 0)
                throw new InvalidOperationException("Invalid employee account.");

            if (currentDateTime.DayOfWeek == DayOfWeek.Saturday ||
                currentDateTime.DayOfWeek == DayOfWeek.Sunday)
            {
                throw new InvalidOperationException(
                    "There's currently no work for today.");
            }

            TimeSpan timeEnd = new TimeSpan(16, 30, 0);

            TimeSpan overtime = TimeSpan.Zero;

            if (currentDateTime.TimeOfDay >= timeEnd)
            {
                overtime = currentDateTime.TimeOfDay - timeEnd;
            }

            bool isClockedOut = _attendanceRepository.ClockOut(
                accountId,
                currentDateTime,
                overtime
            );

            if (!isClockedOut)
            {
                throw new InvalidOperationException(
                    "No active clock-in record was found for today, or you have already clocked out.");
            }

            return overtime;
        }
        public List<Attendance> GetEmployeeAttendance(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return _attendanceRepository.employeeAttendanceListData(user);
        }

        public List<Attendance> GetCurrentDayAttendance(string date, User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return _attendanceRepository.currentDayAttendanceListData(date, user);
        }

        public List<Attendance> GetAttendanceByMonthYear(int fromMonth, int fromYear, int toMonth, int toYear, User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return _attendanceRepository.MonthYearAttendanceListData(
                fromMonth, fromYear, toMonth, toYear, user);
        }

        public List<Attendance> GetAllEmployeesAttendance(int entry)
        {
            return _attendanceRepository.employeesAttendanceListData(entry);
        }

        public List<Attendance> SearchEmployeeAttendance(int entry, string name)
        {
            return _attendanceRepository.SearchEmployeeAttendanceListData(entry, name);
        }

        public int GetRecentNumberOfEntries()
        {
            return _attendanceRepository.recentNumberOfEntries();
        }

        public int GetSearchNumberOfEntries(string name)
        {
            return _attendanceRepository.searchNumberOfEntries(name);
        }

        public int GetFilterNumberOfEntries(string profession, string status, DateTime dateFrom, DateTime dateTo)
        {
            return _attendanceRepository.filterNumberOfEntries(
                profession, status, dateFrom, dateTo);
        }

        public AutoCompleteStringCollection GetEmployeeNames()
        {
            return _attendanceRepository.employeeNamesData();
        }

        public List<Attendance> GetFilterEmployeesAttendance(
            int entry, string profession, string status,
            DateTime dateFrom, DateTime dateTo)
        {
            return _attendanceRepository.filterEmployeesAttendanceListData(
                entry, profession, status, dateFrom, dateTo);
        }

        public Attendance GetAttendanceClock(int accountId)
        {
            if (accountId <= 0)
                throw new ArgumentException("Invalid employee account.", nameof(accountId));

            return _attendanceRepository.AttendanceClock(accountId);
        }
    }
}
