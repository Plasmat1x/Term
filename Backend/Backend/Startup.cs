using Backend.Domain;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Backend;

public class Startup
{
  public IConfiguration Configuration { get; }
  public Startup(IConfiguration configuration) => Configuration = configuration;
  public void ConfigureServices(IServiceCollection services)
  {
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen();

    services.AddDbContext<AppDbContext>(x => x.UseSqlServer(Configuration["ConnectionStrings:RemoteDB"]));

    services.AddIdentity<IdentityUser, IdentityRole>(opts =>
    {
      opts.User.RequireUniqueEmail = true;
      opts.Password.RequiredLength = 6;
      opts.Password.RequireNonAlphanumeric = false;
      opts.Password.RequireDigit = false;
      opts.Password.RequireLowercase = false;
      opts.Password.RequireUppercase = false;
    })
      .AddEntityFrameworkStores<AppDbContext>()
      .AddDefaultTokenProviders();

    services.AddAuthentication(opt =>
    {
      opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
      opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
      opt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
      .AddJwtBearer(opt =>
      {
        opt.SaveToken = true;
        opt.RequireHttpsMetadata = false;
        opt.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
        {
          ValidateIssuer = true,
          ValidIssuer = Configuration["JWT:Issuer"],
          ValidateAudience = true,
          ValidAudience = Configuration["JWT:Audience"],
          ValidateLifetime = true,
          IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:Secret"])),
          ValidateIssuerSigningKey = true
        };
      });

    services.AddAuthorization(options =>
    {
      options.AddPolicy("AdminArea", policy => { policy.RequireRole("admin"); });
    });

    services.AddCors(options =>
    {
      options.AddPolicy("AllowFrontend", builder =>
      {
        var origins = Configuration.GetSection("FrontendOrigins").Get<string[]>();
        builder
          .WithOrigins(origins)
          .AllowAnyHeader()
          .AllowAnyMethod()
          .AllowCredentials();
      });
    });

    services.AddControllers();
  }

  public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
  {
    if (env.IsDevelopment())
    {
      app.UseSwagger();
      app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseRouting();

    app.UseCors("AllowFrontend");
    app.UseAuthentication();
    app.UseAuthorization();

    app.UseEndpoints(endpoints =>
    {
      endpoints.MapControllers();
    });
  }
}
