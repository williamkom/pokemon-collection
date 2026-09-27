using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Pokemon.Api.Models;

namespace Pokemon.Api.Data;
/// <summary>
/// Seeds demo data into the database, including creating trainers with hashed passwords.
/// </summary>
public static class DemoDataSeeder
{
    public static async Task SeedAsync(
        IServiceProvider services)
    {
        await using var scope = services.CreateAsyncScope();

        var db = scope.ServiceProvider.GetRequiredService<PokemonDbContext>();

        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<Trainer>>();

        await CreateTrainerIfMissingAsync(
            db,
            passwordHasher,
            "trainer1@example.com",
            "Trainer123!");

        await CreateTrainerIfMissingAsync(
            db,
            passwordHasher,
            "trainer2@example.com",
            "Trainer123!");
    }
    /// <summary>
    /// Creates a trainer if one with the specified email does not already exist.
    /// </summary>
    /// <param name="db">The database context.</param>
    /// <param name="passwordHasher">The password hasher.</param>
    /// <param name="email">The trainer's email.</param>
    /// <param name="password">The trainer's password.</param>
    private static async Task CreateTrainerIfMissingAsync(
        PokemonDbContext db,
        IPasswordHasher<Trainer> passwordHasher,
        string email,
        string password)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();

        var exists = await db.Trainers.AnyAsync(trainer =>trainer.Email == normalizedEmail);

        if (exists)
        {
            return;
        }

        var trainer = new Trainer
        {
            Email = normalizedEmail
        };

        trainer.PasswordHash = passwordHasher.HashPassword(trainer,password);

        db.Trainers.Add(trainer);

        await db.SaveChangesAsync();
    }
}