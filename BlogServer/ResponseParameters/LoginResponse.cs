namespace BlogServer.ResponseParameters
{
    public class LoginResponse
    {
        public string Token { get; set; }
        public bool Succeeded { get; set; }
        public IEnumerable<string> Errors { get; set; }
    }   
}
