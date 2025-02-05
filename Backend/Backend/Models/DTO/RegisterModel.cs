using System.ComponentModel.DataAnnotations;

namespace Backend.Models.DTO;

public class RegisterModel
{
  [Required]
  public string UserName { get; set; }
  [Required]
  public string Email { get; set; }
  [Required]
  public string Password { get; set; }
}
