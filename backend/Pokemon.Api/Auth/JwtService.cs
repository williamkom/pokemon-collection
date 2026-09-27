using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Pokemon.Api.Models;

namespace Pokemon.Api.Auth;
public sealed class JwtService
{
    private readonly JwtOptions _options;

    public JwtService(IOptions<JwtOptions> options)
    {
        _options = options.Value;
    }
    // This method creates a JWT token for the given trainer and returns the token along with its expiration time.
    public (string Token, DateTimeOffset ExpiresAt)CreateToken(Trainer trainer)
    {
        var expiresAt = DateTimeOffset.UtcNow.AddMinutes(_options.ExpirationMinutes);

        var claims = new List<Claim>
        {
            new( JwtRegisteredClaimNames.Sub,trainer.Id.ToString()),

            new(JwtRegisteredClaimNames.Email,trainer.Email),

            new(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key));

        var credentials = new SigningCredentials( key,SecurityAlgorithms.HmacSha256);

        // Create the JWT token with the specified issuer, audience, claims, expiration time, and signing credentials
        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            expires: expiresAt.UtcDateTime,
            signingCredentials: credentials
        );
        // Serialize the JWT token to a string representation
        var tokenValue = new JwtSecurityTokenHandler().WriteToken(token);

        return (tokenValue, expiresAt);
    }
}