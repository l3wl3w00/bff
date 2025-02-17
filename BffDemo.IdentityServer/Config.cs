using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using IdentityModel;

namespace BffDemo.IdentityServer;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources { get; } =
    [
        new IdentityResources.OpenId(),
        new IdentityResources.Profile()
    ];

    public static IEnumerable<ApiScope> ApiScopes { get; } = 
    [
        new("api1"),
        new("api2"),
    ];

    public static IEnumerable<Client> Clients { get; } =
    [
        new()
        {
            ClientId = "bff1",
            ClientName = "BFF1 IS Client",
            AllowedGrantTypes = GrantTypes.Code,
            RedirectUris = { "https://localhost:5001/signin-oidc" },
            PostLogoutRedirectUris = { "https://localhost:5001/signout-callback-oidc" },
            BackChannelLogoutUri = "https://localhost:5001/bff/backchannel",
            BackChannelLogoutSessionRequired = true,
            ClientSecrets = { new Secret("511536EF-F270-4058-80CA-1C89C192F69A".Sha256()) },
            AllowedScopes = { "openid", "profile", "api1" },
            AccessTokenType = AccessTokenType.Jwt,
            AlwaysIncludeUserClaimsInIdToken = true,

        },
        new()
        {
            ClientId = "bff2",
            ClientSecrets = { new Secret("49C1A7E1-0C79-4A89-A3D6-A37998FB86B0".Sha256()) },
            ClientName = "BFF2 IS Client",
            RedirectUris = { "https://localhost:5002/signin-oidc" },
            PostLogoutRedirectUris = { "https://localhost:5002/signout-callback-oidc" },
            BackChannelLogoutUri = "https://localhost:5002/bff/backchannel",
            BackChannelLogoutSessionRequired = true,
            AllowedScopes = { "openid", "profile", "api2" }, 
            AllowedGrantTypes = GrantTypes.Code,
            AccessTokenType = AccessTokenType.Jwt,
            AlwaysIncludeUserClaimsInIdToken = true,

        }
    ];
}
