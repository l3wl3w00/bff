using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = "https://localhost:5000"; // IdentityServer
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false
        };
    });
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ApiScope", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("scope", "api1", "api2");
    });
});
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/email", (ClaimsPrincipal user, HttpContext context) =>
    {
        Console.WriteLine(context.Request.Headers.Authorization);
        var email = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        return Results.Ok($"Logged in by {email}");
    })
    .RequireAuthorization("ApiScope");
    
app.Run();