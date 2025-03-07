using Duende.IdentityServer.Validation;

namespace BffDemo.IdentityServer;

public class CustomAuthorizeRequestValidator : ICustomAuthorizeRequestValidator
{
    public Task ValidateAsync(CustomAuthorizeRequestValidationContext context)
    {
        return Task.CompletedTask;
    }
}