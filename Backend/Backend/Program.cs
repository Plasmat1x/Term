namespace Backend;

class Program
{
  public static void Main(string[] args)
  {
    CreateHostBuilder(args).Build().Run();
  }

  public static IHostBuilder CreateHostBuilder(string[] args) =>
      Host.CreateDefaultBuilder(args)
          .ConfigureWebHostDefaults(webBuilder =>
          {
            webBuilder.UseStartup<Startup>();
            webBuilder.ConfigureLogging(opt =>
            {
              opt.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning);
              opt.AddFilter("Microsoft.EntityFrameworkCore", LogLevel.Warning);
            });
          });
}



