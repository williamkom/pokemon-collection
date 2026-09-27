using System.ComponentModel.DataAnnotations;

namespace Pokemon.Api.DTO
{
    /// <summary>
    /// ASP.NET therefore already validates the following:
    /// Email address provided
    /// Valid email format
    /// Password provided
    /// </summary>
    public sealed class LoginRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
