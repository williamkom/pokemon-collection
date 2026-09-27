namespace Pokemon.Api.DTO;
/// <summary>
/// Represents a response containing detailed information about a Pokemon, including its ID, name, image URL, height, weight, and types.
/// </summary>
/// <param name="Id"></param>
/// <param name="Name"></param>
/// <param name="ImageUrl"></param>
/// <param name="Height"></param>
/// <param name="Weight"></param>
/// <param name="Types"></param>
public sealed record PokemonResponse(
    int Id,
    string Name,
    string? ImageUrl,
    int Height,
    int Weight,
    IReadOnlyList<string> Types
);