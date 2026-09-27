using Pokemon.Api.DTO;
using Pokemon.Api.Services;

namespace Pokemon.Tests.Fakes;

public sealed class FakePokemonService : IPokemonService
{
    public Task<PokemonResponse?> GetPokemonAsync(int pokemonId)
    {
        if (pokemonId != 25)
        {
            return Task.FromResult<PokemonResponse?>(null);
        }

        var pikachu = new PokemonResponse(
            25,
            "pikachu",
            "https://example.test/pikachu.png",
            4,
            60,
            ["electric"]);

        return Task.FromResult<PokemonResponse?>(
            pikachu);
    }


    public Task<IReadOnlyList<PokemonResponse>>
        GetPokemonListAsync(int limit,int offset )
    {
        IReadOnlyList<PokemonResponse> pokemon =
        [
            new PokemonResponse(
                25,
                "pikachu",
                "https://example.test/pikachu.png",
                4,
                60,
                ["electric"])
        ];

        return Task.FromResult(pokemon);
    }
}