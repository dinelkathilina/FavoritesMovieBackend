using System.Text.Json.Serialization;

namespace FavoritesMovieBackend.Models
{
    public class MovieSearchResponse
    {
        // [JsonPropertyName("results")]
        public List<MovieSearchResult> results { get; set; }

        [JsonPropertyName("total_pages")]
        public int TotalPages { get; set; }

        [JsonPropertyName("total_results")]
        public int TotalResults { get; set; }
    }
}
