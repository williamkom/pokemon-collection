namespace Pokemon.Api.Auth
{
    /// <summary>
    /// Represents the configuration options for JWT (JSON Web Token) authentication.
    /// </summary>
    public sealed class JwtOptions
    {
        public const string SectionName = "Jwt";
      
        public string Issuer { get; set; } = string.Empty;

        public string Audience { get; set; } = string.Empty;

        public string Key { get; set; } = string.Empty;

        public int ExpirationMinutes { get; set; } = 60;
    }
}
