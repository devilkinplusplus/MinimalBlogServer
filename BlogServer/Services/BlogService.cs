using BlogServer.DTOs;
using BlogServer.Models;
using BlogServer.Models.Database;
using BlogServer.ResponseParameters;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BlogServer.Services
{
    public class BlogService
    {
        private readonly IMongoCollection<Blog> _blogsCollection;
        private readonly UserService _userService;
        public BlogService(IOptions<DatabaseSettings> databaseSettings, UserService userService)
        {
            var mongoClient = new MongoClient(databaseSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(databaseSettings.Value.DatabaseName);
            _blogsCollection = mongoDatabase.GetCollection<Blog>(databaseSettings.Value.BlogsCollectionName);
            _userService = userService;
        }

        public async Task<BlogListResponse> GetAsync()
        {
            IEnumerable<BlogListDto> blogList = _blogsCollection.Find(_ => true).ToList().Select(x => new BlogListDto
            {
                Id = x.Id,
                Title = x.Title,
                Description = x.Description,
                Category = x.Category,
                Author = _userService.GetUserFullNameById(x.UserId),
                PhotoUrl = x.PhotoUrl
            }).ToList();
            if (!blogList.Any())
                return new() { Succeeded = false, Error = "There are no blogs" };

            return new() { Succeeded = true, Blogs = blogList };
        }

        public async Task<Blog?> GetAsync(string id) =>
        await _blogsCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(Blog newBlog, string userId)
        {
            newBlog.UserId = userId;
            await _blogsCollection.InsertOneAsync(newBlog);
        }
        public async Task UpdateAsync(string id, Blog updatedBlog)
        {
            await _blogsCollection.UpdateOneAsync(Builders<Blog>.Filter.Eq("Id", id),
                        Builders<Blog>.Update.Set("Title", updatedBlog.Title).Set("Description", updatedBlog.Description).Set("Category", updatedBlog.Category));
        }
        public async Task RemoveAsync(string id) =>
            await _blogsCollection.DeleteOneAsync(x => x.Id == id);

        public async Task WriteImageAsync(string pathName, string blogId)
        {
            await _blogsCollection.UpdateOneAsync(Builders<Blog>.Filter.Eq("Id", blogId), Builders<Blog>.Update.Set("PhotoUrl", pathName));
        }

        public async Task<BlogListResponse> GetAuthorBlogs(string userId)
        {
            var blogs = await _blogsCollection.FindAsync(Builders<Blog>.Filter.Eq("UserId", userId));
            IEnumerable<BlogListDto> blogList = blogs.ToList().Select(x => new BlogListDto
            {
                Id = x.Id,
                Title = x.Title,
                Description = x.Description,
                Category = x.Category,
                Author = x.UserId,
                PhotoUrl = x.PhotoUrl
            }).ToList();

            if (!blogList.Any())
                return new() { Error = "You have no blogs yet", Succeeded = false };
            return new() { Succeeded = true, Blogs = blogList };
        }
    }
}
