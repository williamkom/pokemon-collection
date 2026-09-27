using Pokemon.Api.DTO;

namespace Pokemon.Api.Services;
/// <summary>
/// Defines the contract for a service that provides operations related to Pokemon data, including retrieving individual Pokemon details and fetching a list of Pokemon.
/// </summary>
public interface IPokemonService
{
    Task<PokemonResponse?> GetPokemonAsync(int pokemonId);
    Task<IReadOnlyList<PokemonResponse>> GetPokemonListAsync(int limit, int offset);
}