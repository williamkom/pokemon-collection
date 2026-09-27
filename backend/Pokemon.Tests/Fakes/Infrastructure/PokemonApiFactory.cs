using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Pokemon.Api.Data;
using Pokemon.Api.Models;
using Pokemon.Api.Services;
using Pokemon.Tests.Fakes;

namespace Pokemon.Tests.Infrastructure;

public sealed class PokemonApiFactory
    : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(
        IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.UseSetting(
            "Jwt:Issuer",
            "Pokemon.Tests");

        builder.UseSetting(
            "Jwt:Audience",
            "Pokemon.Tests");

        builder.UseSetting(
            "Jwt:Key",
            "E/aV4D7hgartoNHi7AnebTeMKigb9MyLr1QvuBjzr3Y=");

        builder.UseSetting(
            "Jwt:ExpirationMinutes",
            "60");


        builder.ConfigureServices(services =>
        {
            var dbContextOptionsDescriptor =
                services.SingleOrDefault(
                    service =>
                        service.ServiceType ==
                        typeof(
                            IDbContextOptionsConfiguration<PokemonDbContext>
                        )
                );

            if (dbContextOptionsDescriptor is not null)
            {
                services.Remove(
                    dbContextOptionsDescriptor
                );
            }


            services.RemoveAll<
                DbContextOptions<PokemonDbContext>>();

            services.RemoveAll<
                PokemonDbContext>();


            services.AddDbContext<PokemonDbContext>(
                options =>
                {
                    options.UseInMemoryDatabase(
                        "PokemonTests"
                    );
                });


            services.RemoveAll<
                IPokemonService>();

            services.AddSingleton<
                IPokemonService,
                FakePokemonService>();


            var provider =
                services.BuildServiceProvider();

            using var scope =
                provider.CreateScope();

            var db =
                scope.ServiceProvider
                    .GetRequiredService<PokemonDbContext>();


            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            SeedTrainers(
                scope.ServiceProvider,
                db
            );
        });
    }


    private static void SeedTrainers(
        IServiceProvider services,
        PokemonDbContext db)
    {
        var passwordHasher =
            services.GetRequiredService<
                IPasswordHasher<Trainer>>();


        var trainer1 = new Trainer
        {
            Email = "trainer1@example.com"
        };

        trainer1.PasswordHash =
            passwordHasher.HashPassword(
                trainer1,
                "Trainer123!");


        var trainer2 = new Trainer
        {
            Email = "trainer2@example.com"
        };

        trainer2.PasswordHash =
            passwordHasher.HashPassword(
                trainer2,
                "Trainer123!");


        db.Trainers.AddRange(
            trainer1,
            trainer2);

        db.SaveChanges();
    }
}