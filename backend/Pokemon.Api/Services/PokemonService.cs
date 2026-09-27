using Pokemon.Api.DTO;
using Pokemon.Api.External;
using System.Net;

namespace Pokemon.Api.Services;
/// <summary>
/// Represents a service that provides operations related to Pokemon data, including retrieving individual Pokemon details and fetching a list of Pokemon from an external API.
/// </summary>
public sealed class PokemonService : IPokemonService
{
    private readonly HttpClient _httpClient;

    public PokemonService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <summary>
    /// Retrieves the details of a specific Pokemon by its ID from the external API.
    /// </summary>
    /// <param name="pokemonId"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<PokemonResponse?> GetPokemonAsync(int pokemonId)
    {
        if (pokemonId <= 0)
        {
            return null;
        }

        using var response =await _httpClient.GetAsync( $"pokemon/{pokemonId}");

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();

        var pokemon = await response.Content.ReadFromJsonAsync<PokeApiPokemon>();

        if (pokemon is null)
        {
            throw new InvalidOperationException("PokéAPI returned an empty response.");
        }
        return MapPokemon(pokemon);
    }

    /// <summary>
    /// Retrieves a list of Pokemon from the external API, with support for pagination through limit and offset parameters.
    /// Each Pokemon in the list is fetched in detail using its name.
    /// </summary>
    /// <param name="limit"></param>
    /// <param name="offset"></param>
    /// <returns></returns>
    public async Task<IReadOnlyList<PokemonResponse>> GetPokemonListAsync(int limit,int offset)
    {
        using var response = await _httpClient.GetAsync($"pokemon?limit={limit}&offset={offset}");
        response.EnsureSuccessStatusCode();

        var list = await response.Content.ReadFromJsonAsync<PokeApiListResponse>();

        if (list is null)
        {
            return [];
        }
        var detailTasks =list.Results.Select(resource =>GetPokemonByNameAsync(resource.Name)).ToArray();

        var pokemon = await Task.WhenAll(detailTasks);

        return pokemon
            .Where(item => item is not null)
            .Select(item => item!)
            .ToList();
    }

    /// <summary>
    /// Retrieves the details of a specific Pokemon by its name from the external API.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    private async Task<PokemonResponse?> GetPokemonByNameAsync(string name)
    {
        using var response =await _httpClient.GetAsync($"pokemon/{Uri.EscapeDataString(name)}");

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();

        var pokemon =await response.Content.ReadFromJsonAsync<PokeApiPokemon>();

        return pokemon is null? null: MapPokemon(pokemon);
    }

    /// <summary>
    /// Maps a PokeApiPokemon object to a PokemonResponse object, extracting relevant properties such as ID, name, height, and weight.
    /// </summary>
    /// <param name="pokemon"></param>
    /// <returns></returns>
    private static PokemonResponse MapPokemon(PokeApiPokemon pokemon)
    {
        var types =
            pokemon.Types
                .OrderBy(type => type.Slot)
                .Select(type => type.Type.Name)
                .ToList();

        return new PokemonResponse(
            pokemon.Id,
            pokemon.Name,
            pokemon.Sprites.Other
                .OfficialArtwork
                .FrontDefault,
            pokemon.Height,
            pokemon.Weight,
            types
        );
    }
}