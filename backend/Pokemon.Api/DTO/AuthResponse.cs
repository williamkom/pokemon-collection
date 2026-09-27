namespace Pokemon.Api.DTO;
/// <summary>
/// Represents the response returned after a successful authentication, containing the access token, its expiration time, and the authenticated trainer's information.
/// </summary>
/// <param name="AccessToken"></param>
/// <param name="ExpiresAt"></param>
/// <param name="Trainer"></param>
public sealed record AuthResponse(
    string AccessToken,
    DateTimeOffset ExpiresAt,
    TrainerResponse Trainer
);

public sealed record TrainerResponse(
    int Id,
    string Email
);