using BlogServer.DTOs;

namespace BlogServer.ResponseParameters
{
    public class LoginResponse
    {
        public Token Token { get; set; }
        public bool Succeeded { get; set; }
        public IEnumerable<string> Errors { get; set; }
    }   
}
