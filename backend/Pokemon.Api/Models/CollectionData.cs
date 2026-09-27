namespace Pokemon.Api.Models
{
    public sealed class CollectionData
    {
        public int Id { get; set; }
        // Foreign key properties
        public int TrainerId { get; set; }
        public int PokemonId { get; set; }
        public Trainer Trainer { get; set; } = null!;
        // Timestamp for when the Pokemon was added to the collection
        public DateTimeOffset AddedAt { get; set; } = DateTimeOffset.UtcNow;

    }
}
