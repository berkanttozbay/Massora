using IdentityServer.Models;

public interface ITokenService
{
    string GenerateToken(ApplicationUser user);
}
