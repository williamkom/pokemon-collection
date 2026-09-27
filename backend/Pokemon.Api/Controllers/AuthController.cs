using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pokemon.Api.Auth;
using Pokemon.Api.Data;
using Pokemon.Api.DTO;
using Pokemon.Api.Models;
using System.IdentityModel.Tokens.Jwt;
/// <summary>
/// Controller for handling authentication-related operations.
/// </summary>
namespace Pokemon.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly PokemonDbContext _db;
    private readonly IPasswordHasher<Trainer> _passwordHasher;
    private readonly JwtService _jwtService;

    public AuthController(PokemonDbContext db, IPasswordHasher<Trainer> passwordHasher, JwtService jwtService)
    {
        _db = db;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
    }

    /// <summary>
    /// Logs in a trainer and returns an authentication token.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        // Normalize the email to ensure case-insensitive comparison
        var normalizedEmail =request.Email.Trim().ToLowerInvariant();

        // Check if a trainer with the provided email exists in the database
        var trainer =await _db.Trainers.SingleOrDefaultAsync(trainer =>trainer.Email == normalizedEmail);

        if (trainer is null)
        {
            return Unauthorized( new{message ="Invalid email or password."});
        }
        // Verify the provided password against the stored password hash
        var verificationResult =_passwordHasher.VerifyHashedPassword(trainer, trainer.PasswordHash, request.Password);

        if (verificationResult == PasswordVerificationResult.Failed)
        {
            return Unauthorized( new {message ="Invalid email or password."});
        }
        // Generate a JWT token for the authenticated trainer
        var (token, expiresAt) =_jwtService.CreateToken(trainer);

        return Ok(
            new AuthResponse(
                token,
                expiresAt,
                new TrainerResponse(
                    trainer.Id,
                    trainer.Email)));
    }


    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<TrainerResponse>> Me()
    {
        // Retrieve the trainer ID from the JWT claims
        var trainerIdValue =User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

        if (!int.TryParse(trainerIdValue,out var trainerId))
        {
            return Unauthorized();
        }

        var trainer = await _db.Trainers.AsNoTracking().SingleOrDefaultAsync(trainer =>trainer.Id == trainerId);

        if (trainer is null)
        {
            return Unauthorized();
        }

        return Ok(
            new TrainerResponse(
                trainer.Id,
                trainer.Email));
    }


    [Authorize]
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        return NoContent();
    }
}