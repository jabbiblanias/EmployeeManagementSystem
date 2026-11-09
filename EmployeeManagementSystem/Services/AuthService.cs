using EmployeeManagementSystem.Models;
using EmployeeManagementSystem.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagementSystem.Services
{
    internal class AuthService
    {
        private readonly UserRepository _userRepository;
        public AuthService()
        {
            _userRepository = new UserRepository();
        }

        public User Login(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrEmpty(password))
            {
                return null;
            }

            User user = _userRepository.GetByUsername(
                username.Trim());

            if (user == null)
                return null;

            bool isValid = BCrypt.Net.BCrypt.Verify(
                password, user.Password);

            if (!isValid)
                return null;

            // Do not expose the password hash to the UI.
            user.Password = null;

            return user;
        }

        public void Register(int id, string username, string password)
        {
            if (!_userRepository.EmployeeIdExists(id))
            {
                throw new InvalidOperationException(
                    "The ID is not recorded in the employee records.");
            }

            if (_userRepository.UserIdExists(id))
            {
                throw new InvalidOperationException(
                    "This employee ID already has an existing account.");
            }

            if (_userRepository.UsernameExists(username))
            {
                throw new InvalidOperationException(
                    "The username is already taken.");
            }

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

            _userRepository.CreateUser(id, username, passwordHash);
        }

        public void UpdateUser(User user, string username, string oldPassword, string newPassword)
        {
            if (user == null)
                throw new InvalidOperationException("Invalid user account.");

            if (string.IsNullOrWhiteSpace(username))
                throw new InvalidOperationException("Username is required.");

            if (string.IsNullOrWhiteSpace(oldPassword))
                throw new InvalidOperationException("Current password is required.");

            if (string.IsNullOrWhiteSpace(newPassword))
                throw new InvalidOperationException("New password is required.");

            if (newPassword.Length < 8)
                throw new InvalidOperationException(
                    "New password must be at least 8 characters long.");

            User existingUser = _userRepository.GetByUsername(user.Username);

            if (existingUser == null || existingUser.ID != user.ID)
                throw new InvalidOperationException("Unable to verify your account.");

            if (!BCrypt.Net.BCrypt.Verify(oldPassword, existingUser.Password))
                throw new InvalidOperationException("Current password does not match.");

            if (_userRepository.UsernameExists(username.Trim()))
                throw new InvalidOperationException("The username is already taken.");

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);

            bool updated = _userRepository.UpdateUser(
                user.ID,
                username.Trim(),
                passwordHash
            );

            if (!updated)
                throw new InvalidOperationException("Unable to update account information.");

            user.Username = username.Trim();
            user.Password = null;
        }
    }
}
