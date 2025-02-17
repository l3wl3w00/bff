using Duende.Bff;

namespace BffDemo.Bff2;

public class AnyUrlValidator : IReturnUrlValidator
{
    public Task<bool> IsValidAsync(string returnUrl)
    {
        return Task.FromResult(!string.IsNullOrEmpty(returnUrl));
    }
}