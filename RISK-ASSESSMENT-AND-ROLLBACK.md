# Vega Systems Office API Risk Assessment and Rollback Procedure

## 1. Purpose

This document identifies the main risks associated with deploying the Vega Systems Office API and describes the procedure for returning to the previous working version if deployment fails.

The rollback procedure should be used when a deployment causes a critical application, database, security, configuration, or availability problem that cannot be resolved safely within the approved deployment period.

## 2. Risk Rating
The following ratings are used:

- Low: Limited effect and easy to correct.
- Medium: May interrupt some operations or require technical intervention.
- High: May cause service interruption, data loss, unauthorized access, or deployment failure.

## 3. Short Risk Assessment

Risk 1: Incorrect Production Configuration
- Likelihood: Medium
- Impact: High
- Risk level: High
- Description: Incorrect office settings, connection strings, hostnames, logging levels, or environment names may prevent the application from starting or connecting to the correct database.
- Controls:
	Validate configuration before deployment.
	Use the approved Production configuration template.
	Supply sensitive values through environment variables or an approved secret store.
	Run the health check immediately after deployment.

- Rollback trigger: The application cannot start or the health endpoint reports an unhealthy status.

Risk 2: Exposure of Secrets
- Likelihood: Low
- Impact: High
- Risk level: High
- Description: Passwords, tokens, API keys, private certificates, or database credentials may be accidentally committed to GitHub or written to logs.
- Controls:
	Use .gitignore.
	Use appsettings.example.json with safe placeholders.
	Store secrets in environment variables, .NET user secrets, or an approved secret-management system.
	Review staged files before every Git commit.
	Avoid logging secrets and connection credentials.

- Rollback trigger: A secret is exposed through the repository, application output, or logs.
- Immediate response: Revoke or rotate the exposed secret before redeployment.

## Risk 3: Database Migration or Schema Failure
- Likelihood: Medium
- Impact: High
- Risk level: High
- Description: A schema change may fail, make the database incompatible, or prevent the application from reading and writing report records.
- Controls:
	Test database changes in the Testing environment.
	Back up the database before deployment.
	Record the database version and migration performed.
	Verify database read and write operations after deployment.
- Rollback trigger: The application cannot access required tables, database errors occur, or existing records become unavailable.

## Risk 4: Authentication or Authorization Failure
- Likelihood: Medium
- Impact: High
- Risk level: High
- Description: Incorrect authentication or authorization configuration may allow unauthorized access or prevent legitimate Staff users from accessing protected operations.
- Controls:
	Test unauthenticated access and expect 401 Unauthorized.
	Test a non-Staff role and expect 403 Forbidden.
	Test the Staff role and expect 200 OK.
	Replace classroom mock authentication with a trusted identity provider before real production deployment.

- Rollback trigger: Unauthenticated or non-Staff users can access protected endpoints, or approved Staff users cannot access required operations.

## Risk 5: HTTPS or Certificate Failure
- Likelihood: Medium
- Impact: High
- Risk level: High
- Description: An invalid, missing, or expired certificate may prevent secure access or expose network traffic.
- Controls:
	Install a valid production certificate.
	Verify certificate validity and expiration.
	Test the HTTPS address before release.
	Redirect HTTP requests to HTTPS.

- Rollback trigger: The production HTTPS endpoint is unavailable or certificate validation fails.

## Risk 6: Package or Dependency Vulnerability
- Likelihood: Medium
- Impact: Medium
- Risk level: Medium
- Description: A vulnerable or incompatible NuGet package may introduce security problems or cause runtime failures.
- Controls:
	Run the package vulnerability scan before deployment.
	Update vulnerable dependencies to compatible secure versions.
	Rebuild and rerun application tests after package updates.
- Verification command:
dotnet list package --vulnerable --include-transitive
- Rollback trigger: A dependency update causes application or database failures.
## Risk 7: Logging Failure
- Likelihood: Low
- Impact: Medium
- Risk level: Medium
- Description: Missing logs may make operational failures difficult to investigate. Excessive or unsafe logging may expose sensitive information or consume storage.
- Controls:
	Verify startup, successful-operation, validation-failure, and unexpected-failure logs.
	Configure log retention and rotation.
	Do not log passwords, tokens, or private connection information.

- Rollback trigger: Critical failures cannot be traced or sensitive information appears in logs.

## Risk 8: Failed Application Deployment
- Likelihood: Medium
- Impact: High
- Risk level: High
- Description: The new application version may fail to start, return errors, or become incompatible with the target server.
- Controls:
	Build and test before deployment.
	Generate a Release publish package.
	Keep the previous working package.
	Verify that the required .NET runtime is installed for framework-dependent deployment.

- Rollback trigger: The new version fails startup, health verification, or critical API testing.