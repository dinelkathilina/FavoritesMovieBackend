Favorites Movie Backend

A simple backend API for managing favorite movies.

Getting Started

Clone the repository using the following command:
gh repo clone dinelkathilina/FavoritesMovieBackend

Open the appsettings.json file and update the following settings:
TMDb.ApiKey: Replace with your own TMDB API key.
Jwt.Audience and Jwt.Issuer: Update the audience URL if you plan to use a different port or domain.
Run the application using the following command:
dotnet run( I f you are using Visual Studio , build and run using Https.

ex: https://localhost:7219

CORS Configuration

The API is configured to allow CORS requests from http://localhost:3000. If you need to allow requests from a different origin, update the MyAllowSpecificOrigins policy in the Program.cs file.

Issue Tracking

If you encounter any issues, please open an issue on the GitHub repository.

Note

Make sure to update the appsettings.json file with your own TMDB API key and audience URL (if necessary) before running the application.
