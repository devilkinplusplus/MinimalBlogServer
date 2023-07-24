using BlogServer.Helpers.ImageHelper;
using BlogServer.Models;
using BlogServer.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace BlogServer.Endpoints.Blogs
{
    public static class BlogsEndpoints
    {
        public static WebApplication MapBlogEndpoints(this WebApplication app)
        {
            app.MapGet("/blogs", async (BlogService blogService) =>
            {
                var blogs = await blogService.GetAsync();
                if(blogs.Succeeded)
                    return Results.Ok(blogs.Blogs);
                return Results.Ok(blogs.Error);
            });

            app.MapGet("/blogs/{id}", async (string id, BlogService blogService) =>
            {
                var blog = await blogService.GetAsync(id);
                return blog is null ? Results.NotFound("Not found") :
                                      Results.Ok(blog);
            });

            app.MapPost("/blogs", [Authorize] async (Blog blog, BlogService blogService, HttpContext httpContext) =>
            {
                if (httpContext.User.Identity?.IsAuthenticated == true)
                {
                    string userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                    await blogService.CreateAsync(blog, userId);
                    return Results.Created("Created", blog);
                }
                return Results.Unauthorized();
            });

            app.MapPut("/blogs/{id}", [Authorize] async (string id, Blog blog, BlogService blogService, HttpContext httpContext) =>
            {
                if (httpContext.User.Identity?.IsAuthenticated == true)
                {
                    await blogService.UpdateAsync(id, blog);
                    return Results.NoContent();
                }
                return Results.Unauthorized();
            });

            app.MapDelete("/blogs/{id}", [Authorize] async (string id, BlogService blogService, HttpContext httpContext) =>
            {
                if (httpContext.User.Identity?.IsAuthenticated == true)
                {
                    await blogService.RemoveAsync(id);
                    return Results.NoContent();
                }
                return Results.Unauthorized();
            });


            app.MapPost("/blog/upload",[Authorize] async (HttpContext context, BlogService blogService, ImageUploadHelper helper) =>
            {
                if (context.User.Identity?.IsAuthenticated == true)
                {
                    var request = await context.Request.ReadFormAsync();
                    var file = request.Files.GetFile("File");
                    var blogId = request["BlogId"];
                    var values = await helper.UploadImageAsync(file);
                    await blogService.WriteImageAsync(values.pathName, blogId);
                    return Results.Ok();
                }
                return Results.Unauthorized();   
            });


            return app;
        }
    }
}
