using Microsoft.AspNetCore.Mvc;
using BisleriumPvtLtd.Models;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using BisleriumPvtLtd.Data;
using Microsoft.AspNetCore.Http;
using System.Reflection.Metadata;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using Microsoft.AspNetCore.Identity;
public class BlogController : Controller
{
    private readonly BisleriumPvtLtdContext _context;
    private readonly IWebHostEnvironment _environment;
    private readonly SignInManager<IdentityUser> _signInManager;

    public BlogController(BisleriumPvtLtdContext context, IWebHostEnvironment environment , SignInManager<IdentityUser> signInManager)
    {
        _context = context;
        _environment = environment;
        _signInManager = signInManager;
    }
    public async Task<IActionResult> Index()
    {
        var blogVoteCounts = _context.Votes
    .GroupBy(v => v.Id)
    .Select(group => new {
        Id = group.Key,
        Upvotes = group.Count(v => v.IsUpvote),
        Downvotes = group.Count(v => !v.IsUpvote)
    })
    .ToList();

        return View(blogVoteCounts);
    }

    [HttpGet]
    [Authorize]
    public IActionResult AddBlog()
    {
        return View();
    }
    public async Task<IActionResult> AddBlog(BlogCreateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }


        var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;


        if (userId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        string imageUrl = null;
        if (model.Image != null && model.Image.Length > 0)
        {
            string uploadsFolder = Path.Combine(_environment.WebRootPath, "images");
            string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(model.Image.FileName);
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await model.Image.CopyToAsync(fileStream);
            }

            imageUrl = "/images/" + uniqueFileName;
        }

        Blog blog = new Blog
        {
            UserId = userId,
            Title = model.Title,
            Body = model.Body,
            ImageUrl = imageUrl,

            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        _context.Blogs.Add(blog);
        await _context.SaveChangesAsync();


        return RedirectToAction("Index", "Home");
    }



    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkAsDeleted(int id)
    {
        var blog = await _context.Blogs.FindAsync(id);
        if (blog == null)
        {
            return NotFound();
        }

        blog.IsDeleted = true; // Mark the blog as deleted
        await _context.SaveChangesAsync();

        return RedirectToAction("Index", "Home"); // Redirect back to the index page
    }


    public IActionResult AddedBlog()
    {
        if (_signInManager.IsSignedIn(User))
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var blogs = _context.Blogs.Where(b => b.UserId == userId && !b.IsDeleted).ToList();
            var model = new AddedBlogModel { Blogs = blogs };
            return View(model);
        }
        else
        {
            // Redirect to login page or handle unauthorized access
            return RedirectToAction("Login", "Account");
        }
    }










    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Vote(int blogId, bool isUpvote)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var existingVote = await _context.Votes.FirstOrDefaultAsync(v => v.BlogId == blogId && v.UserId == userId);

        if (existingVote != null)
        {
            existingVote.IsUpvote = isUpvote;
        }
        else
        {
            var vote = new Vote { BlogId = blogId, UserId = userId, IsUpvote = isUpvote };
            _context.Votes.Add(vote);
        }

        await _context.SaveChangesAsync();
        var blog = await _context.Blogs.Include(b => b.Votes).FirstOrDefaultAsync(b => b.Id == blogId);
        var totalUpvotes = blog.Votes.Count(v => v.IsUpvote);
        var totalDownvotes = blog.Votes.Count(v => !v.IsUpvote);
        
        blog.Upvotes = (int)totalUpvotes;
        blog.Downvotes = (int)totalDownvotes;

        await _context.SaveChangesAsync();

        return RedirectToAction("Index", "Home");
    }
    public async Task<IActionResult> Edit(int id)
{
    var blog = await _context.Blogs.FindAsync(id);
    if (blog == null)
    {
        return NotFound();
    }

    var model = new BlogCreateViewModel
    {
        Id = blog.Id,
        Title = blog.Title,
        Body = blog.Body
        // Include other properties as needed
    };

    return View(model);
}

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, BlogCreateViewModel model)
    {
        if (id != model.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var blog = await _context.Blogs.FindAsync(id);
        if (blog == null)
        {
            return NotFound();
        }

        // Update blog details
        blog.Title = model.Title;
        blog.Body = model.Body;

        // Check if a new image is uploaded
        if (model.Image != null && model.Image.Length > 0)
        {
            // Delete the existing image file
            if (!string.IsNullOrEmpty(blog.ImageUrl))
            {
                string existingFilePath = Path.Combine(_environment.WebRootPath, blog.ImageUrl.TrimStart('/'));
                if (System.IO.File.Exists(existingFilePath))
                {
                    System.IO.File.Delete(existingFilePath);
                }
            }

            // Save the new image file
            string newUniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(model.Image.FileName);
            string newFilePath = Path.Combine(_environment.WebRootPath, "images", newUniqueFileName);

            using (var fileStream = new FileStream(newFilePath, FileMode.Create))
            {
                await model.Image.CopyToAsync(fileStream);
            }

            blog.ImageUrl = "/images/" + newUniqueFileName;
        }

        // Save changes to the database
        _context.Update(blog);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index", "Home");
    }



    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var blog = await _context.Blogs.FindAsync(id);
        if (blog == null)
        {
            return NotFound();
        }

        blog.IsDeleted = true;
        await _context.SaveChangesAsync();

        return RedirectToAction("Index", "Blog");
    }




}




