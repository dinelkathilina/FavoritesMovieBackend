# Favorites Movie Backend

A simple backend API for managing favorite movies, built with .NET 8 Minimal API, SQLite, Entity Framework Core, and TMDB API. Authentication and authorization are handled using ASP.NET Core Identity and JWT.

## Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQLite](https://www.sqlite.org/download.html)

### Installation

1. **Clone the repository:**
    ```bash
    gh repo clone dinelkathilina/FavoritesMovieBackend
    ```

2. **Configure the application:**
    - Open the `appsettings.json` file and update the following settings:
      ```json
      {
        "TMDb": {
          "ApiKey": "YOUR_TMDB_API_KEY"
        },
        "Jwt": {
          "Audience": "YOUR_AUDIENCE_URL",
          "Issuer": "YOUR_ISSUER_URL"
        }
      }
      ```

3. **Run the application:**
    ```bash
    dotnet run
    ```
    - If you are using Visual Studio, build and run using HTTPS, e.g., `https://localhost:7219`.

## Authentication and Authorization

The application uses ASP.NET Core Identity for user management and JWT for securing API endpoints. Ensure you have configured the `Jwt` settings in `appsettings.json` correctly.

## CORS Configuration

The API is configured to allow CORS requests from `http://localhost:3000`. To allow requests from a different origin, update the `MyAllowSpecificOrigins` policy in the `Program.cs` file.

## Issue Tracking

If you encounter any issues, please [open an issue](https://github.com/dinelkathilina/FavoritesMovieBackend/issues) on the GitHub repository.

## Note

Make sure to update the `appsettings.json` file with your own TMDB API key and audience URL (if necessary) before running the application.
