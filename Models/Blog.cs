using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;

namespace BisleriumPvtLtd.Models
{
    public class Blog
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        
        public string? Title { get; set; }
        public string? Body { get; set; }
        public string? ImageUrl { get; set; }
        public string? UserId { get; set; }
       
        public  IdentityUser User { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
