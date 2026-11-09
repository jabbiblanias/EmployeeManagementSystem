using System;
using System.Collections.Generic;
using EmployeeManagementSystem.Models;
using EmployeeManagementSystem.Repositories;

namespace EmployeeManagementSystem.Services
{
    public class EmployeeService
    {
        private readonly EmployeeRepository _employeeRepository;

        public EmployeeService()
        {
            _employeeRepository = new EmployeeRepository();
        }

        public void AddEmployee(Employee employee)
        {
            if (employee == null)
                throw new ArgumentNullException(nameof(employee));

            if (string.IsNullOrWhiteSpace(employee.EmployeeID))
                throw new InvalidOperationException("Employee ID is required.");

            if (_employeeRepository.EmployeeIdExists(employee.EmployeeID))
                throw new InvalidOperationException(
                    employee.EmployeeID + " is already taken.");

            _employeeRepository.AddEmployee(employee);
        }

        public void UpdateEmployee(Employee employee)
        {
            if (employee == null)
                throw new ArgumentNullException(nameof(employee));

            if (string.IsNullOrWhiteSpace(employee.EmployeeID))
                throw new InvalidOperationException("Employee ID is required.");

            if (string.IsNullOrWhiteSpace(employee.FullName))
                throw new InvalidOperationException("Employee name is required.");

            _employeeRepository.UpdateEmployee(employee);
        }

        public void UpdateSalary(string employeeID, int salary)
        {
            if (string.IsNullOrWhiteSpace(employeeID))
                throw new ArgumentException("Employee ID is required.");

            if (salary < 0)
                throw new ArgumentException("Salary cannot be negative.");

            _employeeRepository.UpdateSalary(employeeID, salary);
        }

        public void SetEmployeeArchiveStatus(string employeeId, bool isArchived)
        {
            if (string.IsNullOrWhiteSpace(employeeId))
                throw new InvalidOperationException("Employee ID is required.");

            if (isArchived)
            {
                _employeeRepository.ArchiveEmployee(employeeId);
            }
            else
            {
                _employeeRepository.UnarchiveEmployee(employeeId);
            }
        }

        public List<Employee> GetEmployees()
        {
            return _employeeRepository.employeeListData();
        }

        public List<Employee> GetArchivedEmployees()
        {
            return _employeeRepository.ArchiveEmployeeListData();
        }

        public List<Employee> GetEmployeesForSalary()
        {
            return _employeeRepository.SalaryEmployeeListData();
        }

        public Employee GetEmployeeDetails(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return _employeeRepository.EmployeeDetail(user);
        }
    }
}
