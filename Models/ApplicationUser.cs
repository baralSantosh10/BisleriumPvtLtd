using Microsoft.AspNetCore.Identity;

namespace BisleriumPvtLtd.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string UserName { get; set; }
       
    }
}