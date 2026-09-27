using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Pokemon.Api.Auth;
/// <summary>
/// Extension methods for the ClaimsPrincipal class to retrieve the trainer ID, from the authenticated user's claims.
/// </summary>
public static class ClaimsPrincipalExtensions
{
    public static bool TryGetTrainerId(this ClaimsPrincipal user,out int trainerId)
    {
        var trainerIdValue = user.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

        return int.TryParse(trainerIdValue,out trainerId);
    }
}