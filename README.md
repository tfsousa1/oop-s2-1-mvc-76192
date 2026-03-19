# Community Library Desk

This project is an ASP.NET Core MVC application developed as part of the OOP assignment.

## Features
- Books management (Create, Read, Update, Delete)
- Members management (Create, Read, Update, Delete)
- Loans management (Create, Read, Update, Delete)
- Loan return functionality
- Book availability tracking
- Book search, filtering and sorting using IQueryable
- Admin role management page
- Sample data generation (seed)
- CI pipeline using GitHub Actions

## Technologies Used
- ASP.NET Core MVC
- Entity Framework Core (Code-First)
- SQL Server LocalDB
- ASP.NET Identity
- xUnit (unit testing)

## How to Run the Application
1. Clone the repository
2. Open the solution in Visual Studio or VS Code
3. Run the application
4. The database is created automatically on first run

## CI Pipeline
The project includes a GitHub Actions workflow that:
- Builds the application
- Runs automated tests

## Notes
- The project follows a code-first approach using Entity Framework Core
- Database schema is created through migrations
- Sample data is automatically generated when the application starts
