using Microsoft.AspNetCore.Identity;

namespace BisleriumPvtLtd.Models
{
   
        public class Comment
        {
            public int Id { get; set; }
            public string Content { get; set; }
            public DateTime CreatedAt { get; set; }

            // Foreign Key properties
            public int BlogId { get; set; }
            public string UserId { get; set; }

            // Navigation properties
            public virtual Blog Blog { get; set; }
            public IdentityUser User { get; set; }
        }

    
}
