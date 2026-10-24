using System.Security.Claims;
using OfficeProductionApi.Authentication;
using OfficeProductionApi.DTOs;
using OfficeProductionApi.Options;
using OfficeProductionApi.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// 1. Configuration and strongly typed options

builder.Services
    .AddOptions<OfficeOptions>()
    .Bind(
        builder.Configuration.GetSection(
            OfficeOptions.SectionName))
    .Validate(
        options => !string.IsNullOrWhiteSpace(options.Code),
        "Office code is required.")
    .Validate(
        options => !string.IsNullOrWhiteSpace(options.Name),
        "Office name is required.")
    .Validate(
        options => options.ReportRetentionDays > 0,
        "Report retention days must be greater than zero.")
    .Validate(
        options => options.MaximumReportTitleLength > 0,
        "Maximum report title length must be greater than zero.")
    .ValidateOnStart();

// 2. Dependency injection

builder.Services.AddScoped<ReportService>();

// 3. Classroom mock authentication

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme =
            DemoAuthenticationHandler.SchemeName;

        options.DefaultChallengeScheme =
            DemoAuthenticationHandler.SchemeName;
    })
    .AddScheme<AuthenticationSchemeOptions,
        DemoAuthenticationHandler>(
        DemoAuthenticationHandler.SchemeName,
        options => { });

// 4. Authorization policy

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("StaffOnly", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireRole("Staff");
    });
});

var app = builder.Build();

// 5. Unexpected failure logging

app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception exception)
    {
        var logger =
            context.RequestServices
                .GetRequiredService<ILogger<Program>>();

        logger.LogError(
            exception,
            "Unexpected failure while processing {Method} {Path}",
            context.Request.Method,
            context.Request.Path);

        context.Response.StatusCode =
            StatusCodes.Status500InternalServerError;

        await context.Response.WriteAsJsonAsync(new
        {
            Message =
                "An unexpected server error occurred.",
            TraceId =
                context.TraceIdentifier
        });
    }
});

// 6. HTTPS, authentication, and authorization middleware

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

// 7. Start-up operation and structured start-up logging

using (var scope = app.Services.CreateScope())
{
    var startupLogger =
        scope.ServiceProvider
            .GetRequiredService<ILogger<Program>>();

    var officeOptions =
    scope.ServiceProvider
        .GetRequiredService<IOptions<OfficeOptions>>()
        .Value;

    var reportService =
        scope.ServiceProvider
            .GetRequiredService<ReportService>();

    startupLogger.LogInformation(
    "Starting Office Production API for office {OfficeCode}. " +
    "Retention is {RetentionDays} days",
    officeOptions.Code,
    officeOptions.ReportRetentionDays);

    try
    {
        await reportService.InitializeDatabaseAsync();

    startupLogger.LogInformation(
    "Office Production API startup completed successfully");

    }
    catch (Exception exception)
    {
        startupLogger.LogCritical(
            exception,
            "Application startup failed because the database " +
            "could not be initialized");

        throw;
    }
}

// 8. Public endpoint

app.MapGet("/", () =>
{
    return Results.Ok(new
    {
        Application = "Vega Systems Office API",
        Status = "Running",
        Message =
            "Use /health or /api/reports/status"
    });
})
.AllowAnonymous();

// 9. Health verification endpoint

app.MapGet("/health",
    async (ReportService reports) =>
{
    bool databaseAvailable =
        await reports.CanConnectAsync();

    if (!databaseAvailable)
    {
        return Results.Json(
            new
            {
                Status = "Unhealthy",
                Database = "Unavailable",
                CheckedAtUtc = DateTime.UtcNow
            },
            statusCode:
                StatusCodes.Status503ServiceUnavailable);
    }

    return Results.Ok(new
    {
        Status = "Healthy",
        Database = "Available",
        CheckedAtUtc = DateTime.UtcNow
    });
})
.AllowAnonymous();

// 10. Protected staff report-status endpoint

app.MapGet(
    "/api/reports/status",
    async (
        IOptions<OfficeOptions> options,
        ReportService reports,
        ILogger<Program> logger,
        ClaimsPrincipal user)=>
    {
        int activeReportCount =
            await reports.GetActiveCountAsync();

        string userName =
            user.Identity?.Name ?? "Unknown";

        logger.LogInformation(
    "Report status requested successfully by {UserName} " +
    "for office {OfficeCode}. Active report count is " +
    "{ActiveReportCount}",
    userName,
    options.Value.Code,
    activeReportCount);

       return Results.Ok(new
{
    OfficeCode = options.Value.Code,
    OfficeName = options.Value.Name,
    ActiveReports = activeReportCount,
    ReportRetentionDays =
        options.Value.ReportRetentionDays,
    RequestedBy = userName
});
    });

// 11. Protected staff report-creation endpoint

app.MapPost(
    "/api/admin/reports",
    async (
        CreateReportRequest request,
        IOptions<OfficeOptions> options,
        ReportService reports,
        ILogger<Program> logger,
        ClaimsPrincipal user) =>
    {
        string userName =
            user.Identity?.Name ?? "Unknown";

        if (string.IsNullOrWhiteSpace(request.Title))
        {
            logger.LogWarning(
                "Report validation failed for user {UserName}. " +
                "Reason: title was empty",
                userName);

            return Results.BadRequest(new
            {
                Message = "The report title is required."
            });
        }

        if (request.Title.Length >
            options.Value.MaximumReportTitleLength)
        {
            logger.LogWarning(
                "Report validation failed for user {UserName}. " +
                "Title length {TitleLength} exceeded the maximum " +
                "{MaximumLength}",
                userName,
                request.Title.Length,
                options.Value.MaximumReportTitleLength);

            return Results.BadRequest(new
            {
                Message =
                    $"The title cannot exceed " +
                    $"{options.Value.MaximumReportTitleLength} characters."
            });
        }

        if (string.IsNullOrWhiteSpace(request.Description))
        {
            logger.LogWarning(
                "Report validation failed for user {UserName}. " +
                "Reason: description was empty",
                userName);

            return Results.BadRequest(new
            {
                Message = "The report description is required."
            });
        }

        long reportId =
            await reports.CreateReportAsync(
                request,
                userName);

       logger.LogInformation(
    "Report {ReportId} was created successfully " +
    "by {UserName} for office {OfficeCode}",
    reportId,
    userName,
    options.Value.Code);

        return Results.Created(
            $"/api/admin/reports/{reportId}",
            new
            {
                Id = reportId,
                request.Title,
                request.Description,
                OfficeCode = options.Value.Code,
                CreatedBy = userName,
                Message = "Report created successfully."
            });
    });

// 12. Classroom endpoint for unexpected-error testing

app.MapGet(
    "/api/admin/test-error",
    () =>
    {
        throw new InvalidOperationException(
            "Classroom demonstration exception.");
    })
    .RequireAuthorization("StaffOnly");

app.Run();