using System;
using EmployeeManagementSystem.Models;
using EmployeeManagementSystem.Repositories;

namespace EmployeeManagementSystem.Services
{
    public class ProfileService
    {
        private readonly ProfileRepository _profileRepository;

        public ProfileService()
        {
            _profileRepository = new ProfileRepository();
        }

        public Employee GetUserProfile(int userId)
        {
            if (userId <= 0)
                throw new ArgumentException("Invalid user ID.", nameof(userId));

            Employee employee =
                _profileRepository.GetUserProfile(userId);

            if (employee == null)
                throw new InvalidOperationException(
                    "Employee profile not found.");

            return employee;
        }

        public Employee GetEmployeeProfile(string employeeId)
        {
            Employee employee =
                _profileRepository.GetEmployeeProfile(employeeId);

            if (employee == null)
                throw new InvalidOperationException(
                    "Employee profile not found.");

            return employee;
        }
    }
}
