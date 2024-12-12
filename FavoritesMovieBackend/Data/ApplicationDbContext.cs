using FavoritesMovieBackend.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FavoritesMovieBackend.Data
{

    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
        }
        // Add DbSet for FavoriteMovie
        public DbSet<FavoriteMovie> FavoriteMovies { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure the Favorite_Movies table using Fluent API
            builder.Entity<FavoriteMovie>(entity =>
            {
                entity.ToTable("Favorite_Movies"); // Set the table name
                entity.HasKey(e => e.Id); // Define the primary key

                entity.Property(e => e.Title)
                      .IsRequired()
                      .HasMaxLength(200);

                entity.Property(e => e.UserId)
                      .IsRequired();

                entity.HasOne(e => e.User) // Configure the relationship with IdentityUser
                      .WithMany()
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade); // Cascade delete when a user is removed
            });
        }
    }


}
