using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApi.Authentication.Entity;

namespace WebApi.Authentication.Services;

public class JwtTokenService
{
    private readonly List<User> _users = new()
    {
        new("admin", "admin@123", "Administrator", new[] {"catalog.read"}),
        new("user", "user@123", "User", new[] {"catalog.read"})
    };

    public AuthenticationToken? GenerateAuthToken(LoginModel loginModel)
    {
        var user = _users.FirstOrDefault(u => u.Username == loginModel.Username
                                           && u.Password == loginModel.Password);

        if (user is null)
        {
            return null;
        }

        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtExtensions.SecurityKey));
        var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
        var expirationTimeStamp = DateTime.Now.AddMinutes(5);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Name, user.Username),
            new Claim("role", user.Role),
            new Claim("scope", string.Join(" ", user.Scopes))
        };

        var tokenOptions = new JwtSecurityToken(
            issuer: "https://localhost:5100",
            claims: claims,
            expires: expirationTimeStamp,
            signingCredentials: signingCredentials
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

        return new AuthenticationToken
        {
            Token = tokenString,
            ExpiresIn = (int)expirationTimeStamp.Subtract(DateTime.Now).TotalSeconds
        };

    }
}

public class AuthenticationToken
{
    public string Token { get; set; }
    public int ExpiresIn { get; set; }
}
