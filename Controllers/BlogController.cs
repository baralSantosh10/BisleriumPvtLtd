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
public class BlogController : Controller
{
    private readonly BisleriumPvtLtdContext _context;
    private readonly IWebHostEnvironment _environment;

    public BlogController(BisleriumPvtLtdContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;

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

    [HttpPost]
    [ValidateAntiForgeryToken]
     
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




    [HttpGet]
    public IActionResult PreviewBlog(int id)
    {
        var blog = _context.Blogs.FirstOrDefault(b => b.Id == id);
        if (blog == null)
        {
            return NotFound();
        }

        return View(blog);
    }

    [HttpGet]
    public IActionResult ListBlogs()
    {
        var blogs = _context.Blogs.ToList();
        return View(blogs);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteBlog(int id)
    {
        var blog = await _context.Blogs.FindAsync(id);
        if (blog == null)
        {
            return NotFound();
        }

        if (!string.IsNullOrEmpty(blog.ImageUrl))
        {
            string filePath = Path.Combine(_environment.WebRootPath, "images", blog.ImageUrl);
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }

        _context.Blogs.Remove(blog);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [HttpPost]
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Vote(int blogId, bool isUpvote)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        // Update or insert the vote
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

        // Calculate upvote and downvote counts
        var upvotes = await _context.Votes.Where(v => v.BlogId == blogId && v.IsUpvote).CountAsync();
        var downvotes = await _context.Votes.Where(v => v.BlogId == blogId && !v.IsUpvote).CountAsync();

        // Return partial view with updated vote counts
        return RedirectToAction("Index", "Home");
    
}







[HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize] 
    public async Task<IActionResult> AddComment(int blogId, string content)
    {
        var blog = await _context.Blogs.Include(b => b.Comments).FirstOrDefaultAsync(b => b.Id == blogId);
        if (blog == null)
        {
            return NotFound();
        }

        var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            return Unauthorized();
        }

        var comment = new Comment
        {
            BlogId = blogId,
            UserId = userId,
            Content = content,
            CreatedAt = DateTime.Now

        };

        blog.Comments.Add(comment);
        await _context.SaveChangesAsync();

        return PartialView("_CommentPartial", comment);
    }

}

