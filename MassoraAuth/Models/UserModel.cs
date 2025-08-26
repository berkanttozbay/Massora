using System.Collections.Generic;
using System.Linq;

namespace IdentityServer.Models
{
    public class UserModel
    {
        public ApplicationUser User { get; set; }
        public IDictionary<string, IList<string>> Claims { get; set; }
        public string Name { get; set; }
    }
}
