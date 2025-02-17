using Duende.Bff;

namespace BffDemo.Bff1;

public class AnyUrlValidator : IReturnUrlValidator
{
    public Task<bool> IsValidAsync(string returnUrl)
    {
        return Task.FromResult(!string.IsNullOrEmpty(returnUrl));
    }
}