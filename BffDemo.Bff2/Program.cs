using Duende.Bff.Yarp;
using BffDemo.Bff2;
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
builder.Services.AddBff()
    .AddRemoteApis();
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

app.MapBffManagementSilentLoginEndpoints();
app.MapBffManagementBackchannelEndpoint();
app.MapBffDiagnosticsEndpoint();

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

app.MapGet("/bff/logout", async (HttpContext context) =>
{
    var properties = new AuthenticationProperties { RedirectUri = "http://localhost:4201" };
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


foreach (var api in config.Apis)
{
    app.MapRemoteBffApiEndpoint(api.LocalPath, api.RemoteUrl!)
        .RequireAccessToken(api.RequiredToken);
}

app.Run();
