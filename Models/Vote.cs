using BisleriumPvtLtd.Models;
using Microsoft.AspNetCore.Identity;

public class Vote
{
    public int Id { get; set; }
    public int BlogId { get; set; }
    public string UserId { get; set; }
    
    public bool IsUpvote { get; set; }
    public virtual Blog Blog { get; set; }
    public virtual IdentityUser User { get; set; }


}

public enum VoteType
{
    Upvote,
    Downvote
}
