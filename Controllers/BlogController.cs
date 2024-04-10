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
public class BlogController : Controller
{
    private readonly BisleriumPvtLtdContext _context;
    private readonly IWebHostEnvironment _environment;

    public BlogController(BisleriumPvtLtdContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;

    }

    [HttpGet]
    public IActionResult AddBlog()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize] // Ensure the user is authenticated
    public async Task<IActionResult> AddBlog(BlogCreateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        // Retrieve user ID
        var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        // Check if user ID exists
        if (userId == null)
        {
            return RedirectToAction("Login", "Account"); // Redirect to login page if user is not authenticated
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
            UserId = userId, // Assign user ID to the blog
            Title = model.Title,
            Body = model.Body,
            ImageUrl = imageUrl,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        _context.Blogs.Add(blog);
        await _context.SaveChangesAsync();

        // Redirect to the list of blogs after adding a new blog
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
}

