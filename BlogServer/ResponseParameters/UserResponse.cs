namespace BlogServer.ResponseParameters
{
    public class UserResponse
    {
        public bool Succeeded { get; set; }
        public IEnumerable<string> Errors { get; set; }
    }
}
