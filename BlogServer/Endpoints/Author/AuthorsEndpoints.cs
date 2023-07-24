using BlogServer.DTOs;
using BlogServer.ResponseParameters;
using BlogServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace BlogServer.Endpoints.Author
{
    public static class AuthorsEndpoints
    {
        public static WebApplication MapAuthorEndpoints(this WebApplication app)
        {
            app.MapPost("/user", [AllowAnonymous] async (NewUserDto user, UserService userService) =>
            {
                UserResponse res = await userService.CreateUserAsync(user);
                if (res.Succeeded)
                    return Results.Ok();
                return Results.Ok(res.Errors);
            });

            app.MapPost("/user/login", [AllowAnonymous] async (LoginDto model, UserService userService) =>
            {
                LoginResponse response = await userService.LoginUserAsync(model.Username, model.Password);
                if (response.Succeeded)
                    return Results.Ok(response.Token);
                return Results.Ok(response.Errors);
            });

            app.MapGet("/user/blogs", [Authorize] async (HttpContext context, BlogService blogService) =>
            {
                if (context.User.Identity?.IsAuthenticated == true)
                {
                    string userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
                    BlogListResponse res = await blogService.GetAuthorBlogs(userId);
                    if(res.Succeeded)
                        return Results.Ok(res.Blogs);
                    return Results.Ok(res.Error);
                }

                return Results.Unauthorized();
            });

            return app;
        }
    }
}
