using Duende.IdentityServer.Models;
using System.Collections.Generic;

namespace IdentityServer.Configurations
{
    public static class IdentityResourcesConfig
    {
        public static IEnumerable<IdentityResource> GetIdentityResources() =>
            new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile()
            };
    }
}
