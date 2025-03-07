using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

public class SecurityHeadersMiddleware
{
    private readonly RequestDelegate _next;

    public SecurityHeadersMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {

        if (!context.Response.Headers.ContainsKey("Content-Security-Policy"))
        {
            var csp = "frame-src http://localhost:4203 https://localhost:5000 https://id-server.test:5000 http://no-bff-client.test:4203;";
            context.Response.Headers.Append("Content-Security-Policy", csp);
        }
        
        await _next(context);
    }
}