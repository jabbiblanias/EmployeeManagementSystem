using System;
using System.Collections.Generic;
using EmployeeManagementSystem.Repositories;

namespace EmployeeManagementSystem.Services
{
    public class DashboardService
    {
        private readonly DashboardRepository _dashboardRepository;

        public DashboardService()
        {
            _dashboardRepository = new DashboardRepository();
        }

        // Employee dashboard
        public int GetOnTimeCount(int accountId)
        {
            return _dashboardRepository.OnTimeData(accountId);
        }

        public int GetLateCount(int accountId)
        {
            return _dashboardRepository.LateData(accountId);
        }

        public TimeSpan GetOvertime(int accountId)
        {
            return _dashboardRepository.OvertimeData(accountId);
        }

        // Administrator dashboard
        public int GetTotalEmployees()
        {
            return _dashboardRepository.totalOfEmployees();
        }

        public int GetActiveEmployees()
        {
            return _dashboardRepository.numberOfActiveEmployees();
        }

        public int GetInactiveEmployees()
        {
            return _dashboardRepository.numberOfInactiveEmployees();
        }

        public int GetOnTimeCount(DateTime date)
        {
            return _dashboardRepository.OnTimeData(date);
        }

        public int GetLateCount(DateTime date)
        {
            return _dashboardRepository.LateData(date);
        }

        public List<string> GetEvents(int userId)
        {
            return _dashboardRepository.DisplayEvents(userId);
        }

        public List<string> GetTopFiveEmployees()
        {
            return _dashboardRepository.DisplayTopFive();
        }
    }
}