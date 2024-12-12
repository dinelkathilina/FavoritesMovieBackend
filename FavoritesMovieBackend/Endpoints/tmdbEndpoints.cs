using FavoritesMovieBackend.TMDBService;
using FavoritesMovieBackend.DTOs;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
namespace FavoritesMovieBackend.Endpoints
{
    public static class tmdbEndpoints
    {
        public static void mapTMDBEndpoints(this IEndpointRouteBuilder app) {
            app.MapGet("/movies/search", [Authorize] async (TMDbService tmdbService, string query) =>
            {
                if (string.IsNullOrWhiteSpace(query))
                {
                    return Results.BadRequest("Query parameter is required.");
                }

                var result = await tmdbService.SearchMovieAsync(query);

                if (result.Count == 0)
                {
                    return Results.NotFound("No movies found.");
                }

                return Results.Ok(result); // Return the first 5 movie search results
            });




            app.MapGet("/movie/{id}", [Authorize] async (TMDbService tmdbService, int id) =>
            {
                var movieDetails = await tmdbService.GetMovieDetailsAsync(id);

                if (movieDetails == null)
                {
                    return Results.NotFound("Movie not found.");
                }

                return Results.Ok(movieDetails); // Return detailed movie information
            });

            app.MapPost("/favorites/add", [Authorize] async (int movieId, ClaimsPrincipal user, TMDbService tmdbService) =>
            {


                var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);  // This should give you the userId

                try
                {
                    await tmdbService.AddFavoriteMovieAsync(movieId, userId);
                    return Results.Ok("Movie added to favorites successfully.");
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(ex.Message);
                }
            });

            app.MapGet("/movie/rating", async (int movieId, TMDbService tmdbService) =>
            {
                
                var rating = await tmdbService.GetMovieRatingAsync(movieId);
                return Results.Ok(rating);
            });
            app.MapGet("/favorites/isFavorite", [Authorize] async (int movieId, ClaimsPrincipal user, TMDbService tmdbService) =>
            {
                var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Results.BadRequest("User not found.");
                }

                var isFavorite = await tmdbService.IsFavoriteAsync(movieId, userId);
                return Results.Ok(new { isFavorite });
            });


            app.MapGet("/movie/cast", [Authorize] async (int movieId, TMDbService tmdbService) =>
            {
                
                var rating = await tmdbService.GetMovieCastNamesAsync(movieId);
                return Results.Ok(rating);
            });
            app.MapDelete("/favorites/remove", [Authorize] async (int movieId, ClaimsPrincipal user, TMDbService tmdbService) =>
            {
                var userId = user.FindFirstValue(ClaimTypes.NameIdentifier); // Get the authenticated user's ID

                if (string.IsNullOrEmpty(userId))
                {
                    return Results.Unauthorized();
                }

                try
                {
                    await tmdbService.RemoveFavoriteMovieAsync(movieId, userId);
                    return Results.Ok("Movie removed from favorites successfully.");
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(ex.Message);
                }
            });


            app.MapGet("/favorites", [Authorize] async (ClaimsPrincipal user, TMDbService tmdbService) =>
            {
                var userId = user.FindFirstValue(ClaimTypes.NameIdentifier); // Get the authenticated user's ID

                if (string.IsNullOrEmpty(userId))
                {
                    return Results.Unauthorized();
                }

                try
                {
                    var favoriteMovies = await tmdbService.GetFavoriteMoviesAsync(userId);

                    if (favoriteMovies.Count == 0)
                    {
                        return Results.NotFound("No favorite movies found.");
                    }

                    return Results.Ok(favoriteMovies); // Return the list of favorite movies
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(ex.Message);
                }
            });

            app.MapGet("/movie/details/{movieId}", [Authorize] async (int movieId, TMDbService tmdbService) =>
            {
                try
                {
                    // Call the service to get genres and overview
                    var (genres, overview) = await tmdbService.GetMovieGenreAndOverviewAsync(movieId);

                    // Check if data is available
                    if (genres.Count == 0 && string.IsNullOrEmpty(overview))
                    {
                        return Results.NotFound("No details found for the specified movie.");
                    }

                    // Return genres and overview as a JSON response
                    return Results.Ok(new
                    {
                        MovieId = movieId,
                        Genres = genres,
                        Overview = overview
                    });
                }
                catch (Exception ex)
                {
                    // Return a bad request with the exception message
                    return Results.BadRequest(new { Error = ex.Message });
                }
            });

        }
    }
}
