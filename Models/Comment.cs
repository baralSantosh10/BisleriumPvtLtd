using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace BisleriumPvtLtd.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }

   

        [Required]
        public int BlogId { get; set; }

        [Required]
        public IdentityUser UserId { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Content { get; set; }
        public int Upvotes { get; set; }
        public int Downvotes { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        public virtual ICollection<CommentVote> Votes { get; set; }
    }

    public class CommentVote
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CommentId { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public bool IsUpvote { get; set; }

        public virtual Comment Comment { get; set; }
    }
}
