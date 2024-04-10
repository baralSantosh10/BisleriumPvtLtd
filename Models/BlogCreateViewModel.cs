using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace BisleriumPvtLtd.Models
{
    public class BlogCreateViewModel
    {
        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; }
        public string Body { get; set; }
        public IFormFile Image { get; set; }
    }
}
