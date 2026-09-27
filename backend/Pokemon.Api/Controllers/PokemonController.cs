using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pokemon.Api.DTO;
using Pokemon.Api.Services;

namespace Pokemon.Api.Controllers;
/// <summary>
/// Controller for handling Pokémon-related operations.
/// </summary>
[ApiController]
[Authorize]
[Route("api/pokemon")]
public sealed class PokemonController : ControllerBase
{
    private readonly IPokemonService _pokemonService;

    public PokemonController(IPokemonService pokemonService)
    {
        _pokemonService = pokemonService;
    }


    // GET /api/pokemon?limit=20&offset=0
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<PokemonResponse>>> GetPokemon([FromQuery] int limit = 20,[FromQuery] int offset = 0)
    {
        if (limit < 1 || limit > 50)
        {
            return BadRequest(
                new
                {
                    message ="Limit must be between 1 and 50."
                });
        }

        if (offset < 0)
        {
            return BadRequest(
                new
                {
                    message ="Offset must be zero or greater."
                });
        }

        try
        {
            var pokemon = await _pokemonService.GetPokemonListAsync(limit, offset);

            return Ok(pokemon);
        }
        catch (HttpRequestException)
        {
            return StatusCode(
                StatusCodes.Status503ServiceUnavailable,
                new
                {
                    message = "Pokémon data is currently unavailable."
                });
        }
    }
}