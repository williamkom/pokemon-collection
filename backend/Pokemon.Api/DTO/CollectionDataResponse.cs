namespace Pokemon.Api.DTO;
/// <summary>
/// Represents a response containing information about a Pokemon collection entry, including the Pokemon's ID, 
/// the date it was added to the collection, and detailed information about the Pokemon itself.
/// </summary>
/// <param name="PokemonId"></param>
/// <param name="AddedAt"></param>
/// <param name="Pokemon"></param>
public sealed record CollectionEntryResponse(
    int PokemonId,
    DateTimeOffset AddedAt,
    PokemonResponse Pokemon
);