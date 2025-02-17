using Duende.Bff.Yarp;
using BffDemo.Bff1;
using Duende.Bff;
using IdentityModel;
using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials(); // Important for sending/receiving cookies.
    });
});
builder.Services
    .AddBff(o =>
    {
        o.BackchannelLogoutAllUserSessions = true;
    })
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
        options.Cookie.Name = "__Host-bff1";
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

app.MapBffManagementEndpoints();

foreach (var api in config.Apis)
{
    app.MapRemoteBffApiEndpoint(api.LocalPath, api.RemoteUrl!)
        .RequireAccessToken(api.RequiredToken);
}

app.Run();