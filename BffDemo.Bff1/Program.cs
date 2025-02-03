using Duende.Bff.Yarp;
using BffDemo.Bff1;
using Duende.Bff;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddBff()
    .AddRemoteApis();
builder.Services.RemoveAll(typeof(ILoginService));
builder.Services.RemoveAll(typeof(ILogoutService));
builder.Services.AddTransient<ILoginService, LoginService>();
builder.Services.AddTransient<ILogoutService, LogoutService>();
Configuration config = new();
builder.Configuration.Bind("BFF", config);

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = "cookie";
        options.DefaultChallengeScheme = "oidc";
        options.DefaultSignOutScheme = "oidc";
    })
    .AddCookie("cookie", options =>
    {
        options.Cookie.Name = "__Host-bff";
        options.Cookie.SameSite = SameSiteMode.Strict;
    })
    .AddOpenIdConnect("oidc", options =>
    {
        options.Authority = config.Authority;
        options.ClientId = config.ClientId;
        options.ClientSecret = config.ClientSecret;
        
        options.ResponseType = "code";
        options.ResponseMode = "query";

        options.GetClaimsFromUserInfoEndpoint = true;
        options.MapInboundClaims = false;
        options.SaveTokens = true;

        options.Scope.Clear();
        foreach (var scope in config.Scopes)
        {
            options.Scope.Add(scope);
        }

        options.TokenValidationParameters = new()
        {
            NameClaimType = "name",
            RoleClaimType = "role"
        };
    });


var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseAuthentication();
app.UseBff();

// app.MapBffManagementLoginEndpoint();
app.MapBffManagementSilentLoginEndpoints();
// app.MapBffManagementLogoutEndpoint();
app.MapBffManagementBackchannelEndpoint();
app.MapBffDiagnosticsEndpoint();

app.MapGet("/bff/login", async (HttpContext context) =>
{
    if (context.User?.Identity?.IsAuthenticated != true)
    {
        // Redirect back to root after successful login.
        var properties = new AuthenticationProperties { RedirectUri = "http://localhost:4200" };
        await context.ChallengeAsync("oidc", properties);
    }
    else
    {
        context.Response.Redirect("/");
    }
});

app.MapGet("/bff/logout", async (HttpContext context) =>
{
    var properties = new AuthenticationProperties { RedirectUri = "http://localhost:4200" };
    await context.SignOutAsync("cookie", properties);
    await context.SignOutAsync("oidc", properties);
});
app.MapGet("/bff/user", (HttpContext context) =>
{
    if (context.User?.Identity?.IsAuthenticated == true)
    {
        return Results.Ok(new
        {
            Name = context.User.Identity.Name,
            Claims = context.User.Claims.Select(c => new { c.Type, c.Value })
        });
    }

    return Results.Unauthorized();
});

if (config.Apis.Any())
{
    foreach (var api in config.Apis)
    {
        app.MapRemoteBffApiEndpoint(api.LocalPath, api.RemoteUrl!)
            .RequireAccessToken(api.RequiredToken);
    }
}

app.Run();


public class LoginService : ILoginService
{
    public async Task ProcessRequestAsync(HttpContext context)
    {
        if (context.User?.Identity?.IsAuthenticated == true)
        {
            // If already authenticated, simply redirect to the home page.
            context.Response.Redirect("/");
            return;
        }

        // Set a redirect URI after a successful login (in this case, back to the root).
        await context.ChallengeAsync(
            "oidc",
            new AuthenticationProperties
            {
                RedirectUri = "/"
            });
    }
}

public class LogoutService : ILogoutService
{
    public async Task ProcessRequestAsync(HttpContext context)
    {
        var properties = new AuthenticationProperties { RedirectUri = "/" };
        await context.SignOutAsync("cookie", properties);
        await context.SignOutAsync("oidc", properties);
    }
}