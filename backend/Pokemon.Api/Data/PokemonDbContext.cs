using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Pokemon.Api.Models;

namespace Pokemon.Api.Data;
/// <summary>
/// Represents the database context for the Pokemon application, providing access to the Trainers and CollectionData tables.
/// </summary>
public sealed class PokemonDbContext : DbContext
{
    public PokemonDbContext( DbContextOptions<PokemonDbContext> options): base(options)
    {
    }
    public DbSet<Trainer> Trainers => Set<Trainer>();

    public DbSet<CollectionData> CollectionData => Set<CollectionData>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureTrainer(modelBuilder);

        ConfigureCollectionData(modelBuilder);
    }
    /// <summary>
    /// Configures the Trainer entity and its properties in the database model.
    /// </summary>
    /// <param name="modelBuilder"></param>
    private static void ConfigureTrainer(ModelBuilder modelBuilder)
    {
        var trainer = modelBuilder.Entity<Trainer>();

        trainer.ToTable("trainers");

        trainer.HasKey(t => t.Id);

        trainer.Property(t => t.Id).HasColumnName("id");

        trainer.Property(t => t.Email)
            .HasColumnName("email")
            .HasMaxLength(320)
            .IsRequired();

        trainer.Property(t => t.PasswordHash)
            .HasColumnName("password")
            .IsRequired();
        //This means that no two coaches can have exactly the same saved email address.
        trainer.HasIndex(t => t.Email)
            .IsUnique();
    }

    private static void ConfigureCollectionData(
        ModelBuilder modelBuilder)
    {
        var collectionData =modelBuilder.Entity<CollectionData>();

        collectionData.ToTable("collection_data");

        collectionData.HasKey(data => data.Id);

        collectionData.Property(data => data.Id).HasColumnName("id");

        collectionData.Property(data => data.TrainerId).HasColumnName("trainer_id");

        collectionData.Property(data => data.PokemonId)
            .HasColumnName("pokemon_id")
            .IsRequired();
        collectionData.Property(data => data.AddedAt)
            .HasColumnName("added_at");

        collectionData
            .HasOne(data => data.Trainer)
            .WithMany(trainer => trainer.Collections)
            .HasForeignKey(data => data.TrainerId)
            .OnDelete(DeleteBehavior.Cascade);
        // This means that a trainer cannot have the same Pokemon in their collection more than once.
        collectionData
            .HasIndex(data => new
            {
                data.TrainerId,
                data.PokemonId
            })
            .IsUnique();
    }
}