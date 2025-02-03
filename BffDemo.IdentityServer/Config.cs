using Duende.IdentityServer.Models;

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
        new("client1"),
        new("client2")
    ];

    public static IEnumerable<Client> Clients { get; } =
    [
        new()
        {
            ClientId = "client1",
            ClientName = "Client1 IS Client",
            AllowedGrantTypes = GrantTypes.Code,
            RedirectUris = { "http://localhost:4200" },
            PostLogoutRedirectUris = { "http://localhost:4200" },
            ClientSecrets = { new Secret("222DD822-CDFB-48AA-B414-F66B97706401".Sha256()) },
            AllowedScopes = { "openid", "profile", "client1" },
            AllowedCorsOrigins = { "http://localhost:4200" },
            Enabled = true
        },
        // new()
        // {
        //     ClientId = "client2",
        //     ClientName = "Client2 IS Client",
        //     AllowedGrantTypes = GrantTypes.Code,
        //     RedirectUris = { "https://localhost:5001/signin-oidc" },
        //     PostLogoutRedirectUris = { "https://localhost:5001/signout-callback-oidc" },
        //     ClientSecrets = { new Secret("73D683B0-FA07-40FF-9EF8-6CEDB26612EF".Sha256()) },
        //     AllowedScopes = { "openid", "profile", "api1" }, 
        // },
        new()
        {
            ClientId = "bff1",
            ClientName = "BFF1 IS Client",
            AllowedGrantTypes = GrantTypes.Code,
            RedirectUris = { "https://localhost:5001/signin-oidc" },
            PostLogoutRedirectUris = { "https://localhost:5001/signout-callback-oidc" },
            ClientSecrets = { new Secret("511536EF-F270-4058-80CA-1C89C192F69A".Sha256()) },
            AllowedScopes = { "openid", "profile", "api1", "offline_access" },
        },
        new()
        {
            ClientId = "bff2",
            ClientSecrets = { new Secret("49C1A7E1-0C79-4A89-A3D6-A37998FB86B0".Sha256()) },
            ClientName = "BFF2 IS Client",
            RedirectUris = { "https://localhost:5002/signin-oidc" },
            PostLogoutRedirectUris = { "https://localhost:5002/signout-callback-oidc" },
            AllowedScopes = { "openid", "profile", "api2" }, 
            AllowedGrantTypes = GrantTypes.Code,
        }
    ];
}
