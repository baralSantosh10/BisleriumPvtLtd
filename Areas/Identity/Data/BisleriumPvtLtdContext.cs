using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BisleriumPvtLtd.Models;
using System.Reflection.Metadata;

namespace BisleriumPvtLtd.Data;

public class BisleriumPvtLtdContext : IdentityDbContext<IdentityUser>
{
    public BisleriumPvtLtdContext(DbContextOptions<BisleriumPvtLtdContext> options)
    : base(options)
    {

    }
    public DbSet<Blog> Blogs { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
    }
}
