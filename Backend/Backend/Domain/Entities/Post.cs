namespace Backend.Domain.Entities;

public class Post
{
  public long PostId { get; set; }
  public string Title { get; set; }
  public string Content { get; set; }
  public string AuthorId { get; set; }
  public DateTime CreatedAt { get; set; }
  public DateTime? UpdatedAt { get; set; }

  public virtual List<Comment> Comments { get; set; } = new();
}
