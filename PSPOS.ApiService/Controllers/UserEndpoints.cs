// UserEndpoints.cs

using PSPOS.ApiService.Services;
using PSPOS.ServiceDefaults.Models;

namespace PSPOS.ApiService.Controllers;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this WebApplication app)
    {
        app.MapGet("/users", async (UserService UserService) =>
        {
            return await UserService.GetAllUsersAsync();
        });
        
        app.MapPost("/users", async (UserService userService, User user) =>
        {
            return await userService.AddUserAsync(user);
        });
        
    }
}