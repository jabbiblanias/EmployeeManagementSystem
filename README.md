# Employee Management System

A Windows Forms application for managing employee events, attendance, and administrative tasks. Built with .NET Framework 4.7.2.

## Features

- User authentication and role-based access (Employee, Administrator)
- Event calendar with event creation, editing, and categorization
- Attendance tracking and reporting
- Admin dashboard for managing users and viewing attendance statistics

## Getting Started

### Prerequisites

- Visual Studio 2022
- .NET Framework 4.7.2
- SQL Server (update connection strings as needed)

### Setup

1. Clone the repository.
2. Open the solution in Visual Studio 2022.
3. Restore NuGet packages if required.
4. Update the database connection string in the code to match your SQL Server instance.
5. Build and run the application.

## Usage

- Log in with your credentials.
- Employees can view and manage their events and attendance.
- Administrators have access to additional management features.

## Project Structure

- `AdminDashboard.cs` - Admin dashboard logic
- `EventCalendar.cs` - Event calendar and event display
- `EditEvent.cs` - Event editing functionality
- `EventForm.cs` - Event creation and details
- `Program.cs` - Application entry point

## License

Specify your license here.
