﻿using BlogServer.DTOs;

namespace BlogServer.ResponseParameters
{
    public class BlogListResponse
    {
        public bool Succeeded { get; set; }
        public IEnumerable<BlogDto> Blogs { get; set; }
        public string Error { get; set; }
    }
}
