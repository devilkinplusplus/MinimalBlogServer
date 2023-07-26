using BlogServer.DTOs;
using BlogServer.Helpers.ImageHelper;
using BlogServer.Models;
using BlogServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
                return Results.NotFound(blogs.Error);
            });

            app.MapGet("/blogs/{id}", async (string id, BlogService blogService) =>
            {
                var res = await blogService.GetAsync(id);
                if (res.Succeeded)
                    return Results.Ok(res.Blog);
                return Results.NotFound(res.Error);
            });

            app.MapPost("/blog", [Authorize] async (BlogService blogService, HttpContext httpContext, ImageUploadHelper helper) =>
            {
                if (httpContext.User.Identity?.IsAuthenticated == true)
                {
                    var form = await httpContext.Request.ReadFormAsync();
                    string title = form["title"];
                    string description = form["description"];
                    string category = form["category"];
                    IFormFile photoUrl = form.Files["photoUrl"];


                    var results = await helper.UploadImageAsync(photoUrl);
                    string userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                    Blog blog = new()
                    {
                        Title = title,
                        Description = description,
                        Category = category,
                        PhotoUrl = results.pathName,
                        CreatedDate = DateTime.UtcNow,
                        UserId = userId
                    };
                    await blogService.CreateAsync(blog);
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
