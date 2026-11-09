using EmployeeManagementSystem.Models;
using EmployeeManagementSystem.Repositories;
using System;
using System.Collections.Generic;

namespace EmployeeManagementSystem.Services
{
    internal class EventService
    {
        private readonly EventRepository _eventRepository;

        public EventService()
        {
            _eventRepository = new EventRepository();
        }

        // ADD EVENT
        public void AddEvent(int accountId, string title, string description, string location, DateTime eventDate, TimeSpan startTime, TimeSpan endTime)
        {
            ValidateEvent(accountId, title, startTime, endTime);

            title = title.Trim();

            if (_eventRepository.TitleExists(title, eventDate))
            {
                throw new InvalidOperationException(
                    "The title is already taken for this date. Please choose another title.");
            }

            bool isAdded = _eventRepository.AddEvent(
                title,
                description?.Trim() ?? string.Empty,
                location?.Trim() ?? string.Empty,
                eventDate,
                startTime,
                endTime,
                accountId
            );

            if (!isAdded)
                throw new InvalidOperationException("Unable to save the event.");
        }

        public void AddEvent(int accountId, string title, string description, string category, string location, DateTime eventDate, TimeSpan startTime, TimeSpan endTime)
        {
            ValidateEvent(accountId, title, startTime, endTime);

            title = title.Trim();

            if (_eventRepository.TitleExists(title, eventDate))
            {
                throw new InvalidOperationException(
                    "The title is already taken for this date. Please choose another title.");
            }

            bool isAdded = _eventRepository.AddEvent(
                title,
                description?.Trim() ?? string.Empty,
                category,
                location?.Trim() ?? string.Empty,
                eventDate,
                startTime,
                endTime,
                accountId
            );

            if (!isAdded)
                throw new InvalidOperationException("Unable to save the event.");
        }

        // GET EVENT TITLES
        public List<string> GetEventTitles(DateTime eventDate, int accountId)
        {
            if (accountId < 0)
                throw new InvalidOperationException("Invalid user account.");

            return _eventRepository.GetEventTitles(eventDate, accountId);
        }

        // GET EVENT DETAILS
        public Event GetEvent(string title, DateTime eventDate, int accountId)
        {
            if (string.IsNullOrWhiteSpace(title))
                return null;

            return _eventRepository.GetEvent(title, eventDate, accountId);
        }

        // UPDATE EVENT
        public void UpdateEvent(int accountId, string originalTitle, string newTitle, string description, string location, DateTime eventDate, TimeSpan startTime, TimeSpan endTime)
        {
            ValidateEvent(accountId, newTitle, startTime, endTime);

            if (string.IsNullOrWhiteSpace(originalTitle))
                throw new InvalidOperationException("Please select an event to update.");

            newTitle = newTitle.Trim();

            if (_eventRepository.TitleExistsForUpdate(
                newTitle,
                originalTitle,
                eventDate,
                accountId))
            {
                throw new InvalidOperationException(
                    "The title is already taken for this date. Please choose another title.");
            }

            bool isUpdated = _eventRepository.UpdateEvent(
                originalTitle,
                newTitle,
                description?.Trim() ?? string.Empty,
                location?.Trim() ?? string.Empty,
                eventDate,
                startTime,
                endTime,
                accountId
            );

            if (!isUpdated)
                throw new InvalidOperationException("Unable to update the event.");
        }

        public void UpdateEvent(int accountId, string originalTitle, string newTitle, string description, string category, string location, DateTime eventDate, TimeSpan startTime, TimeSpan endTime)
        {
            ValidateEvent(accountId, newTitle, startTime, endTime);

            if (string.IsNullOrWhiteSpace(originalTitle))
                throw new InvalidOperationException("Please select an event to update.");

            newTitle = newTitle.Trim();

            if (_eventRepository.TitleExistsForUpdate(
                newTitle,
                originalTitle,
                eventDate,
                accountId))
            {
                throw new InvalidOperationException(
                    "The title is already taken for this date. Please choose another title.");
            }

            bool isUpdated = _eventRepository.UpdateEvent(
                originalTitle,
                newTitle,
                description?.Trim() ?? string.Empty,
                category?.Trim() ?? string.Empty,
                location?.Trim() ?? string.Empty,
                eventDate,
                startTime,
                endTime,
                accountId
            );

            if (!isUpdated)
                throw new InvalidOperationException("Unable to update the event.");
        }

        // DELETE EVENT
        public void DeleteEvent(string title, DateTime eventDate, int accountId)
        {
            if (accountId < 0)
                throw new InvalidOperationException("Invalid user account.");

            if (string.IsNullOrWhiteSpace(title))
                throw new InvalidOperationException("Please select an event to delete.");

            bool isDeleted = _eventRepository.DeleteEvent(
                title,
                eventDate,
                accountId
            );

            if (!isDeleted)
                throw new InvalidOperationException("Unable to delete the event.");
        }

        // COMMON VALIDATION
        private void ValidateEvent(
            int accountId,
            string title,
            TimeSpan startTime,
            TimeSpan endTime)
        {
            if (accountId < 0)
                throw new InvalidOperationException("Invalid user account.");

            if (string.IsNullOrWhiteSpace(title))
                throw new InvalidOperationException("The event title cannot be empty.");

            if (endTime <= startTime)
                throw new InvalidOperationException(
                    "The event end time must be later than the start time.");
        }

        public List<Event> GetEventsForDate(
            DateTime eventDate,
            int accountId)
        {
            if (accountId < 0)
                throw new InvalidOperationException("Invalid account.");

            return _eventRepository.GetEventsForDate(
                eventDate,
                accountId
            );
        }

        public List<Event> GetEventsForMonth(DateTime startDate, DateTime endDate, int accountId)
        {
            if (accountId < 0)
                throw new InvalidOperationException("Invalid account.");

            return _eventRepository.GetEventsForMonth(
                startDate,
                endDate,
                accountId
            );
        }
    }
}