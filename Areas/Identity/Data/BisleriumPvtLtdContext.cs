using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using BisleriumPvtLtd.Models;
using Microsoft.AspNetCore.Identity;

namespace BisleriumPvtLtd.Data
{
    public class BisleriumPvtLtdContext : IdentityDbContext<IdentityUser>
    {
        public BisleriumPvtLtdContext(DbContextOptions<BisleriumPvtLtdContext> options)
            : base(options)
        {

        }

        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Vote> Votes { get; set; }
        public DbSet<Comment> Comments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

           
        }

    }
}
