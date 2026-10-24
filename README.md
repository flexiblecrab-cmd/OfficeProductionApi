# Vega Systems Office Production API

# Overview
This project is an ASP.NET Core Minimal API created as a production-readiness package for an office reporting application.

## The application includes:

- Externalized application settings
- Strongly typed Office configuration
- SQLite database connectivity
- Structured logging
- Mock authentication
- Staff-only authorization
- Health verification
- Framework-dependent deployment
- Production deployment checklist

## Requirements
.NET 10 SDK
Git
A command-line tool such as Command Prompt or PowerShell

## Safe Configuration
The repository includes appsettings.example.json as a safe configuration template.

To configure the application:

1. Copy appsettings.example.json.
2. Rename the copied file to appsettings.json.
3. Replace the placeholder values with appropriate local settings.
4. Do not commit passwords, API keys, access tokens, private certificates, or production credentials.

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

## Environment Configuration

Development, Testing, and Production settings are documented in CONFIGURATION-MATRIX.md.

The environments use separate office settings, databases, logging levels, and security controls. Production credentials must be supplied through environment variables or an approved secret-management system and must not be committed to GitHub.

## Build and Run