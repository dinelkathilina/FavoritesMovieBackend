using FavoritesMovieBackend.Data;
using FavoritesMovieBackend.Models;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace FavoritesMovieBackend.TMDBService
{
    public class TMDbService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly ApplicationDbContext _dbContext;

        public TMDbService(HttpClient httpClient, string apiKey, ApplicationDbContext dbContext)
        {
            _httpClient = httpClient;
            _apiKey = apiKey;
            _dbContext = dbContext;
        }

        // Method to search a movie by title
        public async Task<List<MovieSearchResult>> SearchMovieAsync(string query)
        {
            var url = $"https://api.themoviedb.org/3/search/movie?api_key={_apiKey}&query={query}";
            var response = await _httpClient.GetStringAsync(url);



            // Deserialize the response into a MovieSearchResponse object
            var movieSearchResponse = JsonSerializer.Deserialize<MovieSearchResponse>(response);


            if (movieSearchResponse?.results.Any() == true)
            {
                return movieSearchResponse.results.Take(15).ToList();
            }


            return new List<MovieSearchResult>(); // Return an empty list
        }




        // Method to get movie details by its ID
        public async Task<MovieSearchResult> GetMovieDetailsAsync(int movieId)
        {
            var url = $"movie/{movieId}?api_key={_apiKey}";
            var response = await _httpClient.GetStringAsync(url);

            // Deserialize the JSON response to a single MovieSearchResult
            var movieDetails = JsonSerializer.Deserialize<MovieSearchResult>(response);
            return movieDetails;
        }

        //Add Favorite Movie method
        public async Task AddFavoriteMovieAsync(int movieID, string userID)
        {
            var movieDetails = await GetMovieDetailsAsync(movieID);

            // Check if the user ID is valid
            if (string.IsNullOrEmpty(userID))
            {
                throw new ArgumentException("User ID cannot be null or empty.", nameof(userID));
            }

            // Check if the movie is already in the user's favorites
            var existingFavorite = await _dbContext.FavoriteMovies
                .FirstOrDefaultAsync(fm => fm.MovieId == movieID && fm.UserId == userID);

            if (existingFavorite != null)
            {
                throw new InvalidOperationException("The movie is already in the user's favorites.");
            }


            if (movieDetails == null)
            {
                throw new InvalidOperationException("The movie details could not be fetched.");
            }

            // Create a new FavoriteMovie entity
            var favoriteMovie = new FavoriteMovie
            {
                MovieId = movieDetails.Id,
                Title = movieDetails.Title,
                UserId = userID
            };

            // Add to the database
            _dbContext.FavoriteMovies.Add(favoriteMovie);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<MovieRating> GetMovieRatingAsync(int movieId)
        {
            var url = $"https://api.themoviedb.org/3/movie/{movieId}?api_key={_apiKey}";
            var response = await _httpClient.GetStringAsync(url);

            // Deserialize the response to get only the vote_average
            var movieRating = JsonSerializer.Deserialize<MovieRating>(response);

            return movieRating;
        }
        public async Task<List<string>> GetMovieCastNamesAsync(int movieId)
        {
            // URL for getting movie cast from TMDb API
            var url = $"https://api.themoviedb.org/3/movie/{movieId}/credits?api_key={_apiKey}";
            var response = await _httpClient.GetStringAsync(url);

            // Deserialize the response to get the cast
            var movieCastResponse = JsonSerializer.Deserialize<MovieCastResponse>(response);

            if (movieCastResponse?.cast != null)
            {
                // Extract and return the names of the cast members
                return movieCastResponse.cast.Select(c => c.name).Take(5).ToList();
            }

            return new List<string>(); // Return an empty list if no cast is found
        }
        public async Task<List<FavoriteMovie>> GetFavoriteMoviesAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentException("User ID cannot be null or empty.", nameof(userId));
            }

            return await _dbContext.FavoriteMovies
                .Where(fm => fm.UserId == userId)
                .ToListAsync();
        }
        public async Task<bool> IsFavoriteAsync(int movieId, string userId)
        {
            return await _dbContext.FavoriteMovies
                .AnyAsync(fm => fm.MovieId == movieId && fm.UserId == userId);
        }


        public async Task RemoveFavoriteMovieAsync(int movieId, string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentException("User ID cannot be null or empty.", nameof(userId));
            }

            // Find the favorite movie in the database
            var favoriteMovie = await _dbContext.FavoriteMovies
                .FirstOrDefaultAsync(fm => fm.MovieId == movieId && fm.UserId == userId);

            if (favoriteMovie == null)
            {
                throw new InvalidOperationException("The movie is not in the user's favorites.");
            }

            // Remove the favorite movie
            _dbContext.FavoriteMovies.Remove(favoriteMovie);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<(List<string> Genres, string Overview)> GetMovieGenreAndOverviewAsync(int movieId)
        {
            // URL for getting movie details from TMDb API
            var url = $"https://api.themoviedb.org/3/movie/{movieId}?api_key={_apiKey}";
            var response = await _httpClient.GetStringAsync(url);

            // Deserialize the response to MovieGenreOverview
            var movieGenreOverview = JsonSerializer.Deserialize<MovieGenreOverview>(response);

            // Ensure response is not null
            if (movieGenreOverview == null)
            {
                return (new List<string>(), null); // Return empty genres and null overview if response is invalid
            }

            // Extract genres and overview
            var genres = movieGenreOverview.genres?.Select(g => g.name).Take(5).ToList() ?? new List<string>();
            var overview = movieGenreOverview.overview;

            return (genres, overview); // Return both genres and overview
        }

    }
}
