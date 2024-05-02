using BisleriumPvtLtd.Data;
using BisleriumPvtLtd.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BisleriumPvtLtd.Controllers
{
    public class CommentController : Controller
    {
        private readonly BisleriumPvtLtdContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CommentController(BisleriumPvtLtdContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> AddComment(int blogId, string content)
        {
            var blog = await _context.Blogs.FindAsync(blogId);
            if (blog == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
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

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> EditComment(int commentId, string newContent)
        {
            var comment = await _context.Comments.FindAsync(commentId);
            if (comment == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            if (userId != comment.UserId)
            {
                return Unauthorized();
            }

            comment.Content = newContent;
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteComment(int commentId)
        {
            var comment = await _context.Comments.FindAsync(commentId);
            if (comment == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            if (userId != comment.UserId)
            {
                return Unauthorized();
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> UpvoteComment(int commentId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var existingVote = await _context.Comments.FirstOrDefaultAsync(c => c.Id == commentId && c.UserId == userId && c.UserId != null);


            if (existingVote != null)
            {
                existingVote.IsUpvote = true;
            }
            else
            {
                var vote = new Comment { Id = commentId, UserId = userId, IsUpvote = true };
                _context.Comments.Add(vote);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DownvoteComment(int commentId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var existingVote = await _context.Comments.FirstOrDefaultAsync(c => c.Id == commentId && c.UserId == userId && c.UserId != null);


            if (existingVote != null)
            {
                existingVote.IsUpvote = false;
            }
            else
            {
                var vote = new Comment { Id = commentId, UserId = userId, IsUpvote = false };
                _context.Comments.Add(vote);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> AddReply(int parentCommentId, string content)
        {
            var parentComment = await _context.Comments.FindAsync(parentCommentId);
            if (parentComment == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return Unauthorized();
            }

            var reply = new Comment
            {
                BlogId = parentComment.BlogId,
               
                UserId = userId,
                Content = content,
                CreatedAt = DateTime.Now
            };

            _context.Comments.Add(reply);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> EditReply(int replyId, string newContent)
        {
            var reply = await _context.Comments.FindAsync(replyId);
            if (reply == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            if (userId != reply.UserId)
            {
                return Unauthorized();
            }

            reply.Content = newContent;
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteReply(int replyId)
        {
            var reply = await _context.Comments.FindAsync(replyId);
            if (reply == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            if (userId != reply.UserId)
            {
                return Unauthorized();
            }

            _context.Comments.Remove(reply);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }

    }
}
