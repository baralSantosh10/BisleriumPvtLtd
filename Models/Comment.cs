using System;
using Microsoft.AspNetCore.Identity;

namespace BisleriumPvtLtd.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public int BlogId { get; set; }
 
        public string UserId { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }

        public int Upvotes { get; set; }
        public int Downvotes { get; set; }

        public bool IsUpvote { get; set; }
        public Blog Blog { get; set; }
        public IdentityUser User { get; set; }
    }
}
