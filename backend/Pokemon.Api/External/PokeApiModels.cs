using System.Text.Json.Serialization;

namespace Pokemon.Api.External;
/// <summary>
/// Represents a Pokemon as returned by the PokeAPI, including its ID, name, height, weight, sprites, and types.
/// </summary>
public sealed class PokeApiPokemon
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public int Height { get; set; }

    public int Weight { get; set; }

    public PokeApiSprites Sprites { get; set; } = new();

    public List<PokeApiTypeSlot> Types { get; set; } = new();
}


public sealed class PokeApiSprites
{
    public PokeApiOtherSprites Other { get; set; } = new();
}

public sealed class PokeApiOtherSprites
{
    [JsonPropertyName("official-artwork")]
    public PokeApiOfficialArtwork OfficialArtwork { get; set; } = new();
}


public sealed class PokeApiOfficialArtwork
{
    [JsonPropertyName("front_default")]
    public string? FrontDefault { get; set; }
}


public sealed class PokeApiTypeSlot
{
    public int Slot { get; set; }

    public PokeApiNamedResource Type { get; set; } = new();
}

public sealed class PokeApiNamedResource
{
    public string Name { get; set; } = string.Empty;

    public string Url { get; set; } = string.Empty;
}
public sealed class PokeApiListResponse
{
    public int Count { get; set; }

    public List<PokeApiNamedResource> Results { get; set; }= new();
}