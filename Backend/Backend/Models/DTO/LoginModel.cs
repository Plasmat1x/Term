using System.ComponentModel.DataAnnotations;

namespace Backend.Models.DTO;

public class LoginModel
{
  [Required]
  [Display(Name = "UserName")]
  public string UserName { get; set; }

  [Required]
  [UIHint("password")]
  [Display(Name = "Password")]
  public string Password { get; set; }
}

