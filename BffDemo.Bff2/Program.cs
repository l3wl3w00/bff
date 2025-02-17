using Duende.Bff.Yarp;
using BffDemo.Bff2;
using Duende.Bff;
using IdentityModel;
using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4201")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials(); // Important for sending/receiving cookies.
    });
});
builder.Services    
    .AddBff(o => o.BackchannelLogoutAllUserSessions = true)
    .AddRemoteApis()
    .AddServerSideSessions();
Configuration config = new();
builder.Configuration.Bind("BFF", config);
builder.Services.AddTransient<IReturnUrlValidator, AnyUrlValidator>();
builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = "cookie";
        options.DefaultChallengeScheme = "oidc";
        options.DefaultSignOutScheme = "oidc";
    })
    .AddCookie("cookie", options =>
    {
        options.Cookie.Name = "__Host-bff2";
        options.Cookie.SameSite = SameSiteMode.None;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    })
    .AddOpenIdConnect("oidc", options =>
    {
        options.Authority = config.Authority;
        options.ClientId = config.ClientId;
        options.ClientSecret = config.ClientSecret;
        
        options.ResponseType = "code";
        options.ResponseMode = "query";

        options.GetClaimsFromUserInfoEndpoint = true;
        options.MapInboundClaims = true;
        options.SaveTokens = true;

        options.Scope.Clear();
        foreach (var scope in config.Scopes)
        {
            options.Scope.Add(scope);
        }

        options.TokenValidationParameters = new()
        {
            NameClaimType = JwtClaimTypes.Name,
            RoleClaimType = JwtClaimTypes.Role
        };
    });


var app = builder.Build();
app.UseCors("AllowAngular");

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseAuthentication();
app.UseBff();

app.MapBffManagementUserEndpoint();
app.MapBffManagementSilentLoginEndpoints();
app.MapBffManagementBackchannelEndpoint();
app.MapBffDiagnosticsEndpoint();
app.MapBffManagementLogoutEndpoint();
app.MapGet("/bff/login", async (HttpContext context) =>
{
    if (context.User?.Identity?.IsAuthenticated != true)
    {
        var properties = new AuthenticationProperties { RedirectUri = "http://localhost:4201" };
        await context.ChallengeAsync("oidc", properties);
    }
    else
    {
        context.Response.Redirect("/");
    }
});



foreach (var api in config.Apis)
{
    app.MapRemoteBffApiEndpoint(api.LocalPath, api.RemoteUrl!)
        .RequireAccessToken(api.RequiredToken);
}

app.Run();

public class AnyUrlValidator : IReturnUrlValidator
{
    public Task<bool> IsValidAsync(string returnUrl)
    {
        return Task.FromResult(!string.IsNullOrEmpty(returnUrl));
    }
}
