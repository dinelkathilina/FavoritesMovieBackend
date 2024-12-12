using FavoritesMovieBackend.DTOs;
using FavoritesMovieBackend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FavoritesMovieBackend.Endpoints
{
    public static class AuthEndpoints
    {
        public static void mapAuthEndpoints(this IEndpointRouteBuilder app) {

            app.MapPost("/register", async (UserManager<IdentityUser> userManager, RegisterUserDto request) =>
            {
                // Check if the user already exists
                if (await userManager.FindByEmailAsync(request.Email) != null)
                {
                    return Results.BadRequest("User already exists.");
                }

                var user = new IdentityUser { Email = request.Email, UserName = request.Email };
                var result = await userManager.CreateAsync(user, request.Password);

                return result.Succeeded ? Results.Ok("User registered successfully.") : Results.BadRequest(result.Errors);
            });

            app.MapPost("/login", async (SignInManager<IdentityUser> signInManager, LoginUserDto request, IConfiguration configuration) =>
            {
                var result = await signInManager.PasswordSignInAsync(request.Email, request.Password, false, false);
                if (result.Succeeded)
                {
                    var user = await signInManager.UserManager.FindByEmailAsync(request.Email);
                    var token = GenerateJwtToken(user, configuration);
                    return Results.Ok(new { token });
                }
                return Results.Unauthorized();
            });

        }
        private static string GenerateJwtToken(IdentityUser user, IConfiguration configuration)
        {
            var jwtSettings = new JwtSettings();
            configuration.GetSection("Jwt").Bind(jwtSettings);

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(jwtSettings.Key);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.NameIdentifier, user.Id)
        }),
                Expires = DateTime.UtcNow.AddMinutes(30),
                Issuer = jwtSettings.Issuer, // Add Issuer
                Audience = jwtSettings.Audience, // Add Audience
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
