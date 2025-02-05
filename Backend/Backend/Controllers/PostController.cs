using Backend.Domain;
using Backend.Domain.Entities;
using Backend.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Backend.Controllers;
[Route("api/posts")]
[ApiController]
public class PostController : ControllerBase
{
  private readonly AppDbContext p_context;
  private readonly ILogger<PostController> p_logger;
  private readonly UserManager<IdentityUser> p_userManager;

  public PostController(AppDbContext context, UserManager<IdentityUser> userManager, ILogger<PostController> logger)
  {
    p_context = context;
    p_logger = logger;
    p_userManager = userManager;
  }

  [HttpGet("all")]
  public async Task<IActionResult> GetAllPosts(CancellationToken ct)
  {
    var posts = await p_context.Posts.OrderByDescending(x => x.CreatedAt).ToListAsync(ct);
    p_logger.LogInformation("Get all posts");
    return Ok(posts);
  }

  [HttpGet("{id}")]
  public async Task<IActionResult> GetPostById(long id, CancellationToken ct)
  {
    var post = await p_context.Posts.FirstOrDefaultAsync(x => x.PostId == id, ct);
    if (post == null)
      return NotFound("Post not found");

    var user = await p_userManager.FindByIdAsync(post.AuthorId);

    var res = new
    {
      post.PostId,
      post.Title,
      post.Content,
      post.CreatedAt,
      post.UpdatedAt,
      Author = user?.UserName,
    };

    p_logger.LogInformation($"Get post at Id:{id}");
    return Ok(res);
  }

  [Authorize]
  [HttpPost("create")]
  public async Task<IActionResult> CreatePost([FromBody] PostModel newPost, CancellationToken ct)
  {
    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    if (userId == null) return Unauthorized();
    var post = new Post
    {
      Content = newPost.Content,
      CreatedAt = DateTime.UtcNow,
      Title = newPost.Title,
      AuthorId = userId,
    };
    p_context.Posts.Add(post);
    await p_context.SaveChangesAsync(ct);
    p_logger.LogInformation($"Post created Title: {post.Title} Author: {post.AuthorId}");
    return CreatedAtAction(nameof(GetPostById), new { id = post.PostId }, post);
  }

  [Authorize("User, Moderator, Admin")]
  [HttpPut("{id}")]
  public async Task<IActionResult> EditPost(long id, [FromBody] PostModel model, CancellationToken ct)
  {
    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    var post = await p_context.Posts.FirstOrDefaultAsync(x => x.PostId == id, ct);
    if (post == null) return NotFound("Post not found");
    if (post.AuthorId != userId && !User.IsInRole("Moderator") && !User.IsInRole("Admin"))
      return Forbid();
    post.Title = model.Title;
    post.Content = model.Content;
    post.UpdatedAt = DateTime.UtcNow;
    await p_context.SaveChangesAsync(ct);
    p_logger.LogInformation($"Post updated Id:{post.PostId} by UserId:{userId}");
    return Ok(post);
  }

  [Authorize]
  [HttpDelete("{id}")]
  public async Task<IActionResult> DeletePost(long id, CancellationToken ct)
  {
    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    if (userId == null) return Unauthorized();
    var post = await p_context.Posts.FirstOrDefaultAsync(x => x.PostId == id, ct);
    if (post == null) return NotFound("Post not found");
    if (!User.IsInRole("Moderator") && !User.IsInRole("Admin"))
      return Forbid();
    p_context.Posts.Remove(post);
    await p_context.SaveChangesAsync(ct);
    p_logger.LogInformation($"Post deleted:{post.PostId} by User:{User.FindFirstValue(ClaimTypes.NameIdentifier)}");
    return Ok("Post deleted");
  }
}
