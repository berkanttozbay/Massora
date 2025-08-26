using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using System.Collections.Generic;

namespace IdentityServer.Configurations
{
    public static class Clients
    {
        public static IEnumerable<Client> GetClients() =>
            new List<Client>
            {
                // 1. MEVCUT ANGULAR CLIENT TANIMIN (DEÐÝÞÝKLÝK YOK)
                new Client
                {
                    ClientId = "angular-client",
                    ClientName = "Angular SPA Client",
                    AllowedGrantTypes = GrantTypes.Code, // Code Flow
                    RequirePkce = true, // PKCE zorunlu
                    RequireClientSecret = false, // SPA'lerde client secret olmaz
                    RedirectUris = { "http://localhost:4200", "http://localhost:4200/silent-renew.html" },
                    PostLogoutRedirectUris = { "http://localhost:5139/Account/Login" },
                    AllowedCorsOrigins = { "http://localhost:4200" },

                    AllowedScopes = { "openid", "profile", "massoraapi", "offline_access" },
                    AllowAccessTokensViaBrowser = true,
                    AccessTokenLifetime = 3600, // 1 saat
                    AllowOfflineAccess = true
                },

                // 2. YENÝ EKLENEN MOBÝL UYGULAMA CLIENT TANIMI
                new Client
            {
                ClientId = "mobil-hafriyat-client",
                ClientName = "Hafriyat Mobil Uygulama",
                AllowedGrantTypes = GrantTypes.ResourceOwnerPassword, // ? BURAYI DEÐÝÞTÝRÝYORSUN
                RequirePkce = false, // ROPC için PKCE gerekmez
                RequireClientSecret = false,

                RedirectUris =
                {
                    "com.massora.hafriyatapp:/oauth2redirect",
                    "https://www.getpostman.com/oauth2/callback",
                },

                PostLogoutRedirectUris =
                {
                    "com.massora.hafriyatapp:/oauth2redirect"
                },

                AllowedScopes =
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    "massoraapi",
                    "offline_access"
                },

                AllowOfflineAccess = true,
                AccessTokenLifetime = 14400,
                RefreshTokenUsage = TokenUsage.OneTimeOnly,
                RefreshTokenExpiration = TokenExpiration.Sliding,
                SlidingRefreshTokenLifetime = 2592000
            }
            };
    }
}