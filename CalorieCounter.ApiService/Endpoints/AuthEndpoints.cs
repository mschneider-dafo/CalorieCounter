using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
namespace CalorieCounter.ApiService;

using AuthResult = Results<Ok<AuthOkResponse>, ValidationProblem>;

public static class AuthEndpoints
{
   public static void MapAuthEndpoints(this WebApplication app)
   {
      app.MapPost("/api/register", Register).WithName("Register").WithSummary("Registers a new User, email and username must be unique");
      app.MapPost("/api/login", Login).WithName("Login").WithSummary("Login with already registered User");
   }

   private static async Task<AuthResult> Register(RegisterRequest request, UserManager<User> userManager, JwtService serv)
   {
      User user = new User()
      {
         UserName = request.Username,
         Email = request.Email,
         CreatedAt = DateTimeOffset.UtcNow
      };

      var res = await userManager.CreateAsync(user, request.Password);

      if (res.Succeeded)
      {
         var tok = serv.GenerateToken(user);
         return TypedResults.Ok(new AuthOkResponse(tok));
      }

      return TypedResults.ValidationProblem(res.Errors.GroupBy(x=> x.Code).ToDictionary(e => e.Key, e => e.Select(x=> x.Description).ToArray()));
   }

   private static async Task<AuthResult> Login(LoginRequest request, UserManager<User> userManager, JwtService serv)
   {
      var user = await userManager.FindByNameAsync(request.Username);

      if (user is null || !await userManager.CheckPasswordAsync(user, request.Password))
         return TypedResults.ValidationProblem([new KeyValuePair<string, string[]>("Credentials", ["Wrong username or password"])]);

      var tok = serv.GenerateToken(user);

      return TypedResults.Ok(new AuthOkResponse(tok));
   }

}
