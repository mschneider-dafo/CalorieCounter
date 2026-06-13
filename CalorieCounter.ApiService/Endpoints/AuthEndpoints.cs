using Microsoft.AspNetCore.Identity;
namespace CalorieCounter.ApiService;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        app.MapPost("/api/register", 
                async(RegisterRequest request, UserManager<User> userManager, JwtService serv) =>{
                    User user = new User(){
                        UserName = request.Username,
                        Email = request.Email,
                    };

                    var res = await userManager.CreateAsync(user,request.Password);

                    if(res.Succeeded){
                        var tok = serv.GenerateToken(user);
                        return Results.Ok(new AuthOkResponse(tok));
                    }

                    return Results.BadRequest(res.Errors);

                });

        app.MapPost("/api/login",
                async(LoginRequest request, UserManager<User> userManager, JwtService serv) =>{
                    var user = await userManager.FindByNameAsync(request.Username);

                    if(user is null)
                        return Results.BadRequest("Username not found");

                    var res = await userManager.CheckPasswordAsync(user,request.Password);

                    if(!res)
                        return Results.BadRequest("Password not correct");


                    var tok = serv.GenerateToken(user);

                    return Results.Ok(new AuthOkResponse(tok));
                });
    }
}
