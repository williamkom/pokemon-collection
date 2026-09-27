using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Pokemon.Api.Auth;
using Pokemon.Api.Data;
using Pokemon.Api.DTO;
using Pokemon.Api.Models;
using Pokemon.Api.Services;

namespace Pokemon.Api.Controllers;
/// <summary>
/// Controller for managing a trainer's Pokémon collection.
/// </summary>
[ApiController]
[Authorize]
[Route("api/collection")]
public sealed class CollectionController : ControllerBase
{
    private readonly PokemonDbContext _db;
    private readonly IPokemonService _pokemonService;


    public CollectionController(PokemonDbContext db, IPokemonService pokemonService)
    {
        _db = db;
        _pokemonService = pokemonService;
    }


    // GET /api/collection
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CollectionEntryResponse>>> GetCollection()
    {
        // Check if the user is authenticated and retrieve the trainer ID
        if (!User.TryGetTrainerId(out var trainerId))
        {
            return Unauthorized();
        }
        // Retrieve the collection entries for the authenticated trainer
        var entries =
            await _db.CollectionData
                .AsNoTracking()
                .Where(entry =>entry.TrainerId == trainerId)
                .OrderByDescending(entry =>entry.AddedAt)
                .ToListAsync();

        // For each entry, fetch the detailed Pokémon data from the Pokémon service
        var detailTasks =
            entries.Select(
                async entry =>
                {
                    var pokemon =await _pokemonService.GetPokemonAsync(entry.PokemonId);

                    return pokemon is null
                        ? null
                        : new CollectionEntryResponse(
                            entry.PokemonId,
                            entry.AddedAt,
                            pokemon);
                });

        CollectionEntryResponse?[] collection;

        try
        {
            collection = await Task.WhenAll(detailTasks);
        }
        catch (HttpRequestException)
        {
            return StatusCode(
                StatusCodes.Status503ServiceUnavailable,
                new
                {
                    message ="Pokémon data is currently unavailable."
                });
        }

        return Ok(
            collection
                .Where(entry => entry is not null)
                .Select(entry => entry!)
                .ToList());

    }


    // POST /api/collection/25
    [HttpPost("{pokemonId:int}")]
    public async Task<ActionResult<CollectionEntryResponse>> AddPokemon(int pokemonId)
    {
        // Check if the user is authenticated and retrieve the trainer ID
        if (!User.TryGetTrainerId(out var trainerId))
        {
            return Unauthorized();
        }

        if (pokemonId <= 0)
        {
            return BadRequest(
                new
                {
                    message = "Pokemon ID must be greater than zero."
                });
        }
        PokemonResponse? pokemon;

        try
        {
            pokemon =await _pokemonService.GetPokemonAsync(pokemonId);
        }
        catch (HttpRequestException)
        {
            return StatusCode(
                StatusCodes.Status503ServiceUnavailable,
                new
                {
                    message =
                        "Pokémon data is currently unavailable."
                });
        }

        if (pokemon is null)
        {
            return NotFound(
                new
                {
                    message ="Pokemon does not exist."
                });
        }
        var alreadyExists = await _db.CollectionData.AnyAsync(entry => entry.TrainerId == trainerId && entry.PokemonId == pokemonId);

        if (alreadyExists)
        {
            return Conflict(
                new
                {
                    message ="Pokemon is already in your collection."
                });
        }

        var entry = new CollectionData
        {
            TrainerId = trainerId,
            PokemonId = pokemonId,
            AddedAt = DateTimeOffset.UtcNow
        };

        _db.CollectionData.Add(entry);

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException exception)
            when (
                exception.InnerException
                    is PostgresException postgresException
                &&
                postgresException.SqlState ==
                    PostgresErrorCodes.UniqueViolation)
        {
            return Conflict(
                new
                {
                    message ="Pokemon is already in your collection."
                });
        }

        var collectionResponse =
            new CollectionEntryResponse(
                entry.PokemonId,
                entry.AddedAt,
                pokemon!);

        return StatusCode( StatusCodes.Status201Created,collectionResponse);
    }
}