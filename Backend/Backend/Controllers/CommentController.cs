using Backend.Domain;
using Backend.Domain.Entities;
using Backend.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Backend.Controllers;

[Route("api/comments")]
[ApiController]
public class CommentController : ControllerBase
{
  private readonly AppDbContext p_context;
  private readonly ILogger<PostController> p_logger;
  private readonly UserManager<IdentityUser> p_userManager;

  public CommentController(AppDbContext context, UserManager<IdentityUser> userManager, ILogger<PostController> logger)
  {
    p_context = context;
    p_logger = logger;
    p_userManager = userManager;
  }

  [Authorize]
  [HttpPost("{postId}")]
  public async Task<IActionResult> AddComment(long postId, [FromBody] CommentModel newComment, CancellationToken ct)
  {
    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    if (userId == null) return Unauthorized();
    var post = await p_context.Posts.FirstOrDefaultAsync(x => x.PostId == postId, ct);
    if (post == null) return NotFound("Post not found");
    var comment = new Comment
    {
      PostId = postId,
      AuthorId = userId,
      Content = newComment.Content,
      CreatedAt = DateTime.UtcNow,
    };
    p_context.Comments.Add(comment);
    await p_context.SaveChangesAsync(ct);
    p_logger.LogInformation($"Comment added:{comment.CommentId} to Post:{comment.PostId} by User:{comment.AuthorId}");

    var result = new
    {
      comment.PostId,
      comment.AuthorId,
      comment.Content,
      CreatedAt = DateTime.UtcNow,
      Author = p_userManager.Users.Where(y => y.Id == comment.AuthorId).Select(y => y.UserName).FirstOrDefault(),
    };

    return Ok(result);

  }

  [Authorize]
  [HttpPut("{commentId}")]
  public async Task<IActionResult> EditComment(long commentId, [FromBody] CommentModel model, CancellationToken ct)
  {
    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    if (userId == null) return Unauthorized();
    var comment = await p_context.Comments.FirstOrDefaultAsync(x => x.CommentId == commentId, ct);
    if (comment == null) return NotFound("Comment not found");
    if (comment.AuthorId != userId && !User.IsInRole("Moderator") && !User.IsInRole("Admin"))
      return Forbid();
    comment.Content = model.Content;
    await p_context.SaveChangesAsync(ct);
    p_logger.LogInformation($"Comment edited: {comment.CommentId} by User:{userId}");
    return Ok(comment);
  }

  [Authorize]
  [HttpDelete("{commentId}")]
  public async Task<IActionResult> DeleteComment(long commentId, CancellationToken ct)
  {
    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    if (userId == null) return Unauthorized();
    var comment = await p_context.Comments.FirstOrDefaultAsync(x => x.CommentId == commentId, ct);
    if (comment == null) return NotFound("Comment not found");
    if (comment.AuthorId != userId && !User.IsInRole("Moderator") && !User.IsInRole("Admin"))
      return Forbid();
    p_context.Comments.Remove(comment);
    await p_context.SaveChangesAsync(ct);
    p_logger.LogInformation($"Comment deleted: {comment.CommentId} by User:{userId}");
    return NoContent();
  }


  [HttpGet("{postId}")]
  public async Task<IActionResult> GetComments(long postId, CancellationToken ct, [FromQuery] int page = 1, [FromQuery] int size = 10)
  {
    var post = await p_context.Posts.FindAsync(new object[] { postId }, ct);
    if (post == null) return NotFound("Post not found");

    var comments = await p_context
      .Comments
      .Where(x => x.PostId == postId)
      .OrderByDescending(x => x.CreatedAt)
      .Skip((page-1)*size)
      .Take(size)
      .Select(x => new
      {
        x.PostId,
        x.CreatedAt,
        x.Content,
        x.CommentId,
        x.AuthorId,
        Author = p_userManager.Users.Where(y => y.Id == x.AuthorId).Select(y => y.UserName).FirstOrDefault(),
      })
      .ToListAsync(ct);

    p_logger.LogInformation($"Comments getted at Post:{postId}");
    return Ok(comments);
  }
}
