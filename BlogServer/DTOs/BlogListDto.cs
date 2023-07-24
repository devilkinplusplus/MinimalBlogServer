namespace BlogServer.DTOs
{
    public class BlogListDto
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string? PhotoUrl { get; set; }
        public string Author { get; set; }
    }
}
