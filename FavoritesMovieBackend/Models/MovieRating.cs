using System.Text.Json.Serialization;

namespace FavoritesMovieBackend.Models
{
    public class MovieRating
    {
        [JsonPropertyName("vote_average")]
        public double VoteAverage { get; set; }
    }
}
