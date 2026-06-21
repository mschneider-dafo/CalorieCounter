using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using CalorieCounter.ApiService;

public class CalorieWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder){
        builder.UseSetting("Jwt:Key", "your-test-jwt-key-must-be-at-least-32-characters-long");
        builder.UseSetting("Jwt:Issuer", "CalorieCounter");
        builder.UseSetting("Jwt:Audience", "CalorieCounterApi");

        builder.ConfigureServices(services =>{
                var descriptors = services.Where(db =>
                        db.ServiceType.FullName?.Contains("CalorieDbContext") ?? false
                        ).ToList();

                foreach(var desc in descriptors)
                    services.Remove(desc);

                var conn = new SqliteConnection("DataSource=:memory:");
                conn.Open();
                services.AddDbContext<CalorieDbContext>(options => options.UseSqlite(conn));
                }
                );


        builder.UseEnvironment("Development");
    }
}

