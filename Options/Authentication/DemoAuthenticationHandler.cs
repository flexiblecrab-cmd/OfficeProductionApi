using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace OfficeProductionApi.Authentication;

public sealed class DemoAuthenticationHandler
    : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly IConfiguration _configuration;

    public const string SchemeName = "Demo";

    public DemoAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        IConfiguration configuration)
        : base(options, logger, encoder)
    {
        _configuration = configuration;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        string roleHeaderName =
            _configuration["DemoAuthentication:RoleHeaderName"]
            ?? "X-Demo-Role";

        string userHeaderName =
            _configuration["DemoAuthentication:UserHeaderName"]
            ?? "X-Demo-User";

        if (!Request.Headers.TryGetValue(
                roleHeaderName,
                out var roleHeaderValue))
        {
            return Task.FromResult(
                AuthenticateResult.NoResult());
        }

        string role = roleHeaderValue.ToString();

        string userName =
            Request.Headers.TryGetValue(
                userHeaderName,
                out var userHeaderValue)
                ? userHeaderValue.ToString()
                : "Classroom User";

        if (string.IsNullOrWhiteSpace(role))
        {
            return Task.FromResult(
                AuthenticateResult.Fail(
                    "The demo role header is empty."));
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, userName),
            new(ClaimTypes.Role, role),
            new("role", role)
        };

        var identity = new ClaimsIdentity(
            claims,
            SchemeName);

        var principal = new ClaimsPrincipal(identity);

        var ticket = new AuthenticationTicket(
            principal,
            SchemeName);

        return Task.FromResult(
            AuthenticateResult.Success(ticket));
    }
}