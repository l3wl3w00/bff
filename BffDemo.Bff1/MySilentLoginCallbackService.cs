using System.Text;
using Duende.Bff;
using IdentityModel;
using Microsoft.AspNetCore.Authentication;

namespace BffDemo.Bff2;

public class MySilentLoginCallbackService : ISilentLoginCallbackService
{

    private readonly ILogger _logger;

    public MySilentLoginCallbackService(ILogger<MySilentLoginCallbackService> logger)
    {
        _logger = logger;
    }

    public virtual async Task ProcessRequestAsync(HttpContext context)
    {
        _logger.LogDebug("Processing silent login callback request");
    
        var result = (await context.AuthenticateAsync()).Succeeded ? "true" : "false";
        var json = $"{{source:'bff-silent-login', isLoggedIn:{result}}}";
        
        var nonce = CryptoRandom.CreateUniqueId(format:CryptoRandom.OutputFormat.Hex);
        
        var html = $"<script nonce='{nonce}'>window.parent.postMessage({json}, '*');</script>";

        context.Response.StatusCode = 200;
        context.Response.ContentType = "text/html";
        
        context.Response.Headers["Content-Security-Policy"] = $"script-src 'nonce-{nonce}';";
        context.Response.Headers["Cache-Control"] = "no-store, no-cache, max-age=0";
        context.Response.Headers["Pragma"] = "no-cache";

        _logger.LogDebug("Silent login endpoint rendering HTML with JS postMessage to '*' with isLoggedIn {IsLoggedIn}", result);
    
        await context.Response.WriteAsync(html, Encoding.UTF8);
    }
}