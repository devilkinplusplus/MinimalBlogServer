using AspNetCore.Identity.MongoDbCore.Models;
using BlogServer.Endpoints.Author;
using BlogServer.Endpoints.Blogs;
using BlogServer.Helpers.ImageHelper;
using BlogServer.Models;
using BlogServer.Models.Database;
using BlogServer.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<DatabaseSettings>(
    builder.Configuration.GetSection("DatabaseSettings"));

var mongoDbSettings = builder.Configuration.GetSection(nameof(DatabaseSettings)).Get<DatabaseSettings>();
builder.Services.AddIdentity<User, MongoIdentityRole<string>>(opt =>
{
    opt.Password = new PasswordOptions
    {
        RequireDigit = false,
        RequiredLength = 8,
        RequiredUniqueChars = 0,
        RequireNonAlphanumeric = false,
        RequireLowercase = false,
        RequireUppercase = false
    };
})
.AddMongoDbStores<User, MongoIdentityRole<string>, string>(mongoDbSettings.ConnectionString, mongoDbSettings.DatabaseName);

builder.Services.AddScoped<BlogService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<ImageUploadHelper>();


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(opt =>
{
    opt.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = false,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});


builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

_ = app.MapBlogEndpoints();
_ = app.MapAuthorEndpoints();




app.UseAuthentication();
app.UseAuthorization();

app.Run();

