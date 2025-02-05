using Microsoft.AspNetCore.Identity;
using System.Text.Json.Serialization;

namespace Backend.Domain.Entities;

public class Comment
{
  public long CommentId { get; set; }
  public long PostId { get; set; }
  public string AuthorId { get; set; }
  public string Content { get; set; }
  public DateTime CreatedAt { get; set; }

  [JsonIgnore]
  public virtual Post Post { get; set; }
}
