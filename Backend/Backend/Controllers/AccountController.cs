using Backend.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Backend.Controllers;
[Route("api/account")]
[ApiController]
public class AccountController : ControllerBase
{
  private readonly UserManager<IdentityUser> p_userManager;
  private readonly RoleManager<IdentityRole> p_roleManager;
  private readonly IConfiguration p_configuration;
  private readonly ILogger<AccountController> p_logger;

  public AccountController(UserManager<IdentityUser> userMgr, RoleManager<IdentityRole> roleMgr, ILogger<AccountController> logger, IConfiguration configuration)
  {
    p_userManager=userMgr;
    p_roleManager=roleMgr;
    p_configuration=configuration;
    p_logger=logger;
  }

  [HttpPost("login")]
  [AllowAnonymous]
  public async Task<IActionResult> Login([FromBody] LoginModel model, CancellationToken ct)
  {
    if (!ModelState.IsValid)
      return BadRequest(ModelState);

    var user = await p_userManager.FindByNameAsync(model.UserName);
    if (user == null || !await p_userManager.CheckPasswordAsync(user, model.Password))
      return Unauthorized("Invalid username or password");

    var role = await p_userManager.GetRolesAsync(user);

    var token = GenerateJwtToken(user);

    return Ok(new { token, name = user.UserName, role = role.FirstOrDefault() });
  }

  [HttpPost("register")]
  [AllowAnonymous]
  public async Task<IActionResult> Register([FromBody] RegisterModel model, CancellationToken ct)
  {
    if (!ModelState.IsValid)
      return BadRequest(ModelState);

    var user = new IdentityUser { UserName = model.UserName, Email = model.Email };
    var result = await p_userManager.CreateAsync(user, model.Password);
    if (!result.Succeeded)
      return BadRequest(result.Errors);

    if (!await p_roleManager.RoleExistsAsync("User"))
      await p_roleManager.CreateAsync(new IdentityRole("User"));
    await p_userManager.AddToRoleAsync(user, "User");

    return Ok("User registred successfully");
  }

  [Authorize(Roles = "Admin")]
  [HttpPost("create-role")]
  public async Task<IActionResult> CreateRole([FromBody] string roleName)
  {
    if (string.IsNullOrWhiteSpace(roleName)) return BadRequest("Invalid role name");

    if (!await p_roleManager.RoleExistsAsync(roleName))
    {
      await p_roleManager.CreateAsync(new IdentityRole(roleName));
      return Ok($"Role `{roleName}` created successfully");
    }

    return BadRequest("Role alredy exists");
  }

  [Authorize]
  [HttpPost("logout")]
  public IActionResult Logout()
  {
    return Ok("Logged out");
  }

  private string GenerateJwtToken(IdentityUser user)
  {
    var claims = new[] {
      new Claim(JwtRegisteredClaimNames.Sub, user.Id),
      new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
    };

    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(p_configuration["Jwt:Secret"]));
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var token = new JwtSecurityToken(
      issuer: p_configuration["Jwt:Issuer"],
      audience: p_configuration["Jwt:Audience"],
      claims: claims,
      expires: DateTime.UtcNow.AddHours(2),
      signingCredentials: creds
      );

    return new JwtSecurityTokenHandler().WriteToken(token);
  }
}

