using BlogServer.DTOs;
using BlogServer.Models;
using BlogServer.ResponseParameters;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;

namespace BlogServer.Services
{
    public class UserService
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;
        private readonly SignInManager<User> _signInManager;
        public UserService(UserManager<User> userManager, IConfiguration configuration, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _configuration = configuration;
            _signInManager = signInManager;
        }

        public async Task<UserResponse> CreateUserAsync(NewUserDto user)
        {
            User newUser = new()
            {
                Id = Guid.NewGuid().ToString(),
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.UserName,
                Email = user.Email,
            };
            IdentityResult result = await _userManager.CreateAsync(newUser, user.Password);

            if (result.Succeeded)
                return new() { Succeeded = true };
            return new() { Succeeded = false, Errors = result.Errors.Select(x => x.Description).ToList() };
        }
        public async Task<LoginResponse> LoginUserAsync(string username, string password)
        {
            User? user = await _userManager.FindByNameAsync(username);
            if (user is not null)
            {
                SignInResult result = await _signInManager.CheckPasswordSignInAsync(user, password, false);
                if (result.Succeeded)
                {
                    string token = await CreateAccessToken(user);
                    return new() { Succeeded = true, Token = token };
                }
                return new() { Succeeded = false, Errors = new List<string>() { "Password is incorrect!" } };
            }
            return new() { Succeeded = false, Errors = new List<string>() { "User is not found!" } };
        }
        public async Task<string> CreateAccessToken(User user)
        {
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                        issuer: issuer,
                        audience: audience,
                        signingCredentials: credentials,
                        claims: await AddUserValuesToTokenAsync(user)
                    );

            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(token);
        }
        private async Task<List<Claim>> AddUserValuesToTokenAsync(User user)
        {
            var userRoles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name,user.UserName)
            };

            foreach (string role in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            return claims;
        }
        public async Task<User> GetUserInfoById(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }
        public string GetUserFullNameById(string id)
        {
            User user = _userManager.Users.FirstOrDefault(x => x.Id == id);
            return user.FirstName + " " + user.LastName;
        }

    }
}
