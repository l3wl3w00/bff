﻿using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using IdentityModel;

namespace BffDemo.IdentityServer;

public static class Config
{
    public static string Bff1Url => "https://bff-server-1.test:5001";
    public static string Bff2Url => "https://bff-server-2.test:5002";
    
    public static string Bff1UrlLocalhost => "https://localhost:5001";
    public static string Bff2UrlLocalhost => "https://localhost:5002";
    
    public static string NoClientBffUrl => "http://localhost:4203"; 
    // public static string NoClientBffUrl => "http://no-bff-client.test:4203"; 
    public static IEnumerable<IdentityResource> IdentityResources { get; } =
    [
        new IdentityResources.OpenId(),
        new IdentityResources.Profile()
    ];

    public static IEnumerable<ApiScope> ApiScopes { get; } = 
    [
        new("api1"),
        new("api2"),
        new("no_bff"),
    ];

    public static IEnumerable<Client> Clients { get; } =
    [
        new()
        {
            ClientId = "bff1",
            ClientName = "BFF1 IS Client",
            AllowedGrantTypes = GrantTypes.Code,
            RedirectUris = { $"{Bff1Url}/signin-oidc", Bff1Url },
            PostLogoutRedirectUris = { $"{Bff1Url}" },
            BackChannelLogoutUri = $"{Bff1UrlLocalhost}/bff/backchannel",
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
            RedirectUris = { $"{Bff2Url}/signin-oidc", Bff2Url},
            PostLogoutRedirectUris = { $"{Bff2Url}" },
            BackChannelLogoutUri = $"{Bff2UrlLocalhost}/bff/backchannel",
            BackChannelLogoutSessionRequired = true,
            AllowedScopes = { "openid", "profile", "api2" }, 
            AllowedGrantTypes = GrantTypes.Code,
            AccessTokenType = AccessTokenType.Jwt,
            AlwaysIncludeUserClaimsInIdToken = true,
        },
        new()
        {
            ClientId = "no_bff",
            RequireClientSecret = false,
            ClientName = "No BFF IS Client",
            RedirectUris = { $"{NoClientBffUrl}/main-page", $"{NoClientBffUrl}/silent-refresh.html", $"{NoClientBffUrl}/signout-oidc" },
            FrontChannelLogoutUri = $"{NoClientBffUrl}/signout-oidc",
            PostLogoutRedirectUris = { $"{NoClientBffUrl}/main-page" },
            ClientSecrets = { new Secret("no-bff-secret".Sha256()) },
            BackChannelLogoutSessionRequired = true,
            AllowedScopes = { "openid", "profile", "no_bff", "offline_access" }, 
            AllowedGrantTypes = GrantTypes.Code,
            AccessTokenType = AccessTokenType.Jwt,
            AlwaysIncludeUserClaimsInIdToken = true,
            AllowAccessTokensViaBrowser = true,
            AllowOfflineAccess = true,
            AllowedCorsOrigins = { NoClientBffUrl } 
        }
    ];
}
