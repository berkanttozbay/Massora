using Duende.IdentityServer.Models;
using System.Collections.Generic;

namespace IdentityServer.Configurations
{
    public static class ApiResources
    {
        public static IEnumerable<ApiScope> GetApiScopes() =>
            new List<ApiScope>
            {
                new ApiScope("massoraapi", "Massora API Access")
            };
        public static IEnumerable<ApiResource> GetApiResources() =>
            new List<ApiResource>
            {
                // API'nizi bir kaynak (resource) olarak tan�mlay�n
                new ApiResource("massoraapi", "Massora API")
                {
                    // Bu kayna��n, ayn� isimdeki scope'u kulland���n� belirtin
                    Scopes = { "massoraapi" }
                }
            };
    }
}
