## Vega Systems Office API Deployment Checklist

##1. HTTPS

- Install a valid production HTTPS certificate.
- Confirm that the certificate is valid and not expired.
- Configure the production application to use HTTPS.
- Redirect HTTP requests to HTTPS.
- Verify that the production HTTPS endpoint is reachable.
- Do not use the local development certificate in production.

##2. Secrets and Configuration

- Remove passwords, tokens, and API keys from source code.
- Do not commit production secrets to GitHub.
- Use environment variables or an approved secret store.
- Configure the production database connection string.
- Confirm that Office Code and Office Name are correct.
- Confirm that the production logging levels are appropriate.
- Replace mock authentication with a trusted identity provider.

##3. Database Migration

- Confirm that the target database is reachable.
- Test the database changes in a staging environment.
- Create a backup before applying database changes.
- Apply the approved database migration or schema update.
- Confirm that the Reports table exists.
- Verify that the application can read and write records.
- Record who performed the database update and when it was completed.

##4. Database Backup

- Create a complete database backup before deployment.
- Record the backup date and storage location.
- Confirm that the backup file is accessible.
- Verify the database restoration procedure.
- Protect the backup from unauthorized access.
- Retain the backup according to the organization’s retention policy.

##5. Logging

- Confirm that application startup is logged.
- Confirm that successful operations are logged.
- Confirm that validation failures are logged.
- Confirm that unexpected failures are logged.
- Confirm that passwords, tokens, and secrets are not logged.
- Configure log storage and rotation.
- Configure log-retention requirements.
- Confirm that authorized support personnel can access the logs.

##6. Authentication and Authorization

- Replace classroom mock authentication before production deployment.
- Configure a trusted identity provider.
- Confirm that unauthenticated requests return 401 Unauthorized.
- Confirm that authenticated non-staff requests return 403 Forbidden.
- Confirm that authorized Staff requests succeed.
- Confirm that administrative endpoints require the StaffOnly policy.

##7. Health Verification

- Start the deployed application.
- Request the /health endpoint.
- Confirm that application status is Healthy.
- Confirm that database status is Available.
- Test the public root endpoint.
- Test the protected report-status endpoint.
- Create a test report using an approved test identity.
- Review the logs for unexpected errors.

##8. Rollback

- Keep a copy of the previous working application package.
- Keep the pre-deployment database backup.
- Document how to stop the new application version.
- Document how to restore the previous application version.
- Document how to restore the database backup.
- Identify who is authorized to approve a rollback.
- Start rollback if critical health checks fail.
- Verify the /health endpoint after rollback.
- Document the rollback result.

##9. Final Approval

- Application owner approval received.
- Database administrator approval received, if applicable.
- Information security approval received, if applicable.
- Deployment date and time recorded.
- Application version or Git commit recorded.
- Deployment result documented.