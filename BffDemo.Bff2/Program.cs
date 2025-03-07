using Duende.Bff.Yarp;
using BffDemo.Bff2;
using Duende.Bff;
using IdentityModel;

var builder = WebApplication.CreateBuilder(args);

Configuration config = new()
{
    ServerUrl = Environment.GetEnvironmentVariable("ASPNETCORE_URLS")?.Split(';')[0]!,
};
builder.Configuration.Bind("BFF", config);
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins(config.ClientUrl!)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials(); // Important for sending/receiving cookies.
    });
});
builder.Services    
    .AddBff()
    .AddRemoteApis()
    .AddServerSideSessions();
builder.Services.AddTransient<IReturnUrlValidator, AnyUrlValidator>();
builder.Services.AddTransient<ISilentLoginCallbackService, MySilentLoginCallbackService>();
builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = "cookie";
        options.DefaultChallengeScheme = "oidc";
        options.DefaultSignOutScheme = "oidc";
    })
    .AddCookie("cookie", options =>
    {
        options.Cookie.Name = "__Host-bff";
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
        options.RequireHttpsMetadata = false;
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