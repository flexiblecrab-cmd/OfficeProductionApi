# Vega Systems Office Production API

# Overview
This project is an ASP.NET Core Minimal API created as a production-readiness package for an office reporting application.

# The application includes:

- Externalized application settings
- Strongly typed Office configuration
- SQLite database connectivity
- Structured logging
- Mock authentication
- Staff-only authorization
- Health verification
- Framework-dependent deployment
- Production deployment checklist

# Requirements
.NET 10 SDK
Git
A command-line tool such as Command Prompt or PowerShell

# Safe Configuration
The repository includes appsettings.example.json as a safe configuration template.

To configure the application:

Copy appsettings.example.json.
Rename the copied file to appsettings.json.
Replace the placeholder values with appropriate local settings.
Do not commit passwords, API keys, access tokens, private certificates, or production credentials.
Example configuration:

{
  "Office": {
    "Code": "YOUR-OFFICE-CODE",
    "Name": "YOUR-OFFICE-NAME",
    "ReportRetentionDays": 90,
    "MaximumReportTitleLength": 100
  },
  "ConnectionStrings": {
    "OfficeDatabase": "Data Source=Data/office.db"
  }
}