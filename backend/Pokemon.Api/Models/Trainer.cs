namespace Pokemon.Api.Models
{
    /// <summary>
    /// Represents a trainer in the Pokemon application.
    /// </summary>
    public sealed class Trainer
    {
        // Primary Key
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public ICollection<CollectionData> Collections { get; set; } = new List<CollectionData>();

    }
}
