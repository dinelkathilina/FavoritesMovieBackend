using Microsoft.AspNetCore.Identity;

namespace FavoritesMovieBackend.Models
{
    public class FavoriteMovie
    {
        public int Id { get; set; } // Primary key
        public int MovieId { get; set; } // Movie ID from TMDb
        public string Title { get; set; } = string.Empty; // Movie title
        public string UserId { get; set; } = string.Empty; // Associated user ID
        public IdentityUser User { get; set; } = null!; // Navigation property
    }
}
