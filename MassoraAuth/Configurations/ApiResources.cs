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
                // API'nizi bir kaynak (resource) olarak tanýmlayýn
                new ApiResource("massoraapi", "Massora API")
                {
                    // Bu kaynaðýn, ayný isimdeki scope'u kullandýðýný belirtin
                    Scopes = { "massoraapi" }
                }
            };
    }
}
