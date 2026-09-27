using Microsoft.AspNetCore.Mvc.Testing;
using Pokemon.Api.DTO;
using Pokemon.Tests.Infrastructure;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Pokemon.Tests;

public sealed class CollectionTests: IClassFixture<PokemonApiFactory>
{
    private readonly HttpClient _client;

    public CollectionTests(PokemonApiFactory factory)
    {
        _client = factory.CreateClient(
            new WebApplicationFactoryClientOptions
            {
                BaseAddress =
                    new Uri("https://localhost")
            });
    }


    [Fact]
    public async Task Trainer_CanOnlySeeOwnCollection()
    {
        // ---------------------------
        // Trainer 1 login
        // ---------------------------

        var trainer1Token =
            await LoginAsync(
                "trainer1@example.com",
                "Trainer123!");
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                trainer1Token);

        // ---------------------------
        // Trainer 1 adds Pikachu
        // ---------------------------

        var addResponse =
            await _client.PostAsync(
                "/api/collection/25",
                null);

        Assert.Equal(HttpStatusCode.Created,addResponse.StatusCode);


        // ---------------------------
        // Trainer 1 sees Pikachu
        // ---------------------------

        var trainer1Collection =
            await _client.GetFromJsonAsync<
                List<CollectionEntryResponse>>(
                "/api/collection");


        Assert.NotNull(
            trainer1Collection);


        Assert.Contains(
            trainer1Collection,
            entry =>
                entry.PokemonId == 25);


        // ---------------------------
        // Trainer 2 login
        // ---------------------------

        var trainer2Token =
            await LoginAsync(
                "trainer2@example.com",
                "Trainer123!");


        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                trainer2Token);


        // ---------------------------
        // Trainer 2 gets collection
        // ---------------------------

        var trainer2Collection =
            await _client.GetFromJsonAsync<
                List<CollectionEntryResponse>>(
                "/api/collection");


        Assert.NotNull(
            trainer2Collection);


        // ---------------------------
        // Critical requirement
        // ---------------------------

        Assert.DoesNotContain(
            trainer2Collection,
            entry =>
                entry.PokemonId == 25);
    }
    private async Task<string> LoginAsync(
        string email,
        string password)
    {
        var response =
            await _client.PostAsJsonAsync(
                "/api/auth/login",
                new LoginRequest
                {
                    Email = email,
                    Password = password
                });


        response.EnsureSuccessStatusCode();


        var auth =
            await response.Content
                .ReadFromJsonAsync<AuthResponse>();


        Assert.NotNull(auth);


        return auth.AccessToken;
    }
}