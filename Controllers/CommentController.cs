using BisleriumPvtLtd.Data;
using BisleriumPvtLtd.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

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

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var comment = new Comment
            {
                BlogId = blogId,
                UserId = user,
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

            var user = await _userManager.GetUserAsync(User);
            if (user != comment.UserId)
            {
                return Unauthorized();
            }

            comment.Content = newContent;
            _context.Update(comment);
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

            var user = await _userManager.GetUserAsync(User);
            if (user != comment.UserId)
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
        public async Task<IActionResult> VoteComment(int Id, bool isUpvote)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Find the comment by ID
            var comment = await _context.Comments.FindAsync(Id);
            if (comment == null)
            {
                // If comment is not found, return NotFound or handle the error appropriately
                return NotFound();
            }

            // Check if the user has already voted on this comment
            var existingVote = await _context.CommentVotes.FirstOrDefaultAsync(c => c.CommentId == Id && c.UserId == userId);

            if (existingVote != null)
            {
                // Update the existing vote
                existingVote.IsUpvote = isUpvote;
            }
            else
            {
                // Create a new vote
                var vote = new CommentVote { CommentId = Id, UserId = userId, IsUpvote = isUpvote };
                _context.CommentVotes.Add(vote);
            }

            await _context.SaveChangesAsync();

            // Update the upvotes and downvotes count in the Comment table
            var totalUpvotes = await _context.CommentVotes.CountAsync(v => v.CommentId == Id && v.IsUpvote);
            var totalDownvotes = await _context.CommentVotes.CountAsync(v => v.CommentId == Id && !v.IsUpvote);

            comment.Upvotes = totalUpvotes;
            comment.Downvotes = totalDownvotes;

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

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var reply = new Comment
            {
               
                BlogId = parentComment.BlogId,
                UserId = user,
                Content = content,
                CreatedAt = DateTime.Now
            };

            _context.Comments.Add(reply);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }

        public async Task<bool> CanEditComment(int commentId)
        {
            var comment = await _context.Comments.FindAsync(commentId);
            if (comment == null)
            {
                return false;
            }

            var user = await _userManager.GetUserAsync(User);
            return user != null && user == comment.UserId;
        }

        public async Task<int> CountVotes(int commentId, bool isUpvote)
        {
            return await _context.CommentVotes.Where(v => v.CommentId == commentId && v.IsUpvote == isUpvote).CountAsync();
        }
    }
}
