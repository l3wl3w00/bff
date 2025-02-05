using System.Security.Claims;
using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Test;
using IdentityModel;

namespace BffDemo.IdentityServer;
public class ProfileService : IProfileService
{
    private readonly TestUserStore _users;

    public ProfileService(TestUserStore users)
    {
        _users = users;
    }

    public Task GetProfileDataAsync(ProfileDataRequestContext context)
    {
        var subjectId = context.Subject.GetSubjectId();
        var user = _users.FindBySubjectId(subjectId);

        if (user == null)
        {
            return Task.CompletedTask;
        }
        context.IssuedClaims = user.Claims.ToList();
        return Task.CompletedTask;
    }

    public Task IsActiveAsync(IsActiveContext context)
    {
        context.IsActive = true;
        return Task.CompletedTask;
    }
}