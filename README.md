# Employee Management System

A desktop employee management application built with Windows Forms and .NET Framework 4.7.2. The system centralizes employee records, attendance, salary information, and workplace events in one role-based application.

## Features

### Authentication and roles

- User registration and login
- BCrypt password hashing
- Role-based access for employees and administrators
- Separate employee and administrator dashboards

### Employee self-service

- View employee profile information
- Record and review attendance
- Track on-time attendance, late arrivals, and overtime
- View events through a calendar interface
- Create, edit, and delete personal events

### Administration

- Add and manage employee records
- View employee profiles and employment status
- Review attendance records and statistics
- View salary information
- Monitor total, active, and inactive employees
- View upcoming events and top employee summaries

## Technology stack

- C# Windows Forms
- .NET Framework 4.7.2
- Microsoft SQL Server
- ADO.NET with `Microsoft.Data.SqlClient`
- BCrypt.Net-Next for password hashing
- Visual Studio 2022

## Architecture

The application uses a simple layered structure:

- `Forms/` - Windows Forms views and user interaction
- `Services/` - Business logic for authentication, employees, attendance, dashboards, profiles, and events
- `Repositories/` - Database access for each major application area
- `Models/` - Domain models such as `User`, `Employee`, `Attendance`, and `Event`
- `Data/` - Database connection configuration and shared data-access code
- `Program.cs` - Application entry point

## Requirements

- Windows
- Visual Studio 2022 with the .NET desktop development workload
- .NET Framework 4.7.2 developer pack
- SQL Server or SQL Server Express
- A configured `DB_Employee` database with the tables required by the repositories

## Getting started

1. Clone the repository:

   ```text
   git clone https://github.com/jabbiblanias/EmployeeManagement.git
   ```

2. Open `EmployeeManagementSystem.sln` in Visual Studio.
3. Restore the NuGet packages listed in `EmployeeManagementSystem/packages.config`.
4. Create or configure the SQL Server database used by the application.
5. Update the `EmployeeManagementSystem/App.config` connection string for the local SQL Server instance. Do not commit passwords or other secrets to source control.
6. Build the solution and run the application.

> The connection string is environment-specific. Replace the server, database, and authentication values with your own development settings before running the application.

## Using the application

1. Register a user or use an existing account.
2. Sign in through the login screen.
3. Employees can access their profile, attendance, dashboard, and event calendar.
4. Administrators can manage employees and review organization-wide attendance, salary, and event information.

## Security notes

- Store database credentials outside source control whenever possible.
- Use a separate development database rather than production data.
- BCrypt is used for password storage; passwords should not be stored as plain text.
- Review and restrict database permissions for the account used by the application.

## Project structure

```text
EmployeeManagementSystem/
|-- Data/
|-- Forms/
|-- Models/
|-- Repositories/
|-- Services/
|-- App.config
`-- Program.cs
```

## License

No license has been specified for this repository.
