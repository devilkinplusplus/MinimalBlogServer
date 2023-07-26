using BlogServer.DTOs;

namespace BlogServer.ResponseParameters
{
    public class BlogOneResponse
    {
        public bool Succeeded { get; set; }
        public BlogDto Blog { get; set; }
        public string Error { get; set; }
    }
}
