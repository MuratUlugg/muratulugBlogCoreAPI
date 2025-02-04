﻿using System;
using System.Collections.Generic;

namespace muratulugBlogCore.API.Models
{
    public partial class Article
    {
        public Article()
        {
            Comment = new HashSet<Comment>();
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public string ContentSummary { get; set; }
        public string ContentMain { get; set; }
        public DateTime PublishDate { get; set; }
        public string Picture { get; set; }
        public int CategoryId { get; set; }
        public int ViewCount { get; set; }

        public Category Category { get; set; }
        public ICollection<Comment> Comment { get; set; }
    }
}
