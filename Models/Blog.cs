using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BisleriumPvtLtd.Models
{
    public class Blog
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; }

        public string Body { get; set; }
        public string ImageUrl { get; set; }
        public string UserId { get; set; }
        public int? Upvotes { get; set; }
        public int? Downvotes { get; set; }

        // Navigation properties
        public IdentityUser User { get; set; }
        public virtual ICollection<Vote> Votes { get; set; }
        public ICollection<Comment> Comments { get; set; }

        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Constructor to initialize collections
        public Blog()
        {
            Votes = new List<Vote>();
            Comments = new List<Comment>();
        }
    }

    public class AddedBlogModel
    {
        public List<Blog> Blogs { get; set; }
    }
}
