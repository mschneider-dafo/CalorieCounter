using Microsoft.AspNetCore.Identity;
namespace CalorieCounter.ApiService;

public class User : IdentityUser<int>
{
    public DateTimeOffset CreatedAt{get;init;}
}
