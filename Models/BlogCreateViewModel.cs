using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace BisleriumPvtLtd.Models
{
    public class BlogCreateViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public IFormFile Image { get; set; }

    }
}
