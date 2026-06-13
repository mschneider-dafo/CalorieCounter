namespace CalorieCounter.ApiService;

public record RegisterRequest(string Username, string Email, string Password);

public record LoginRequest(string Username, string Password);

public record AuthOkResponse(string Token, DateTimeOffset ExpiresAt){
    internal AuthOkResponse((string Token, DateTime ExpiresAt) arg):this(arg.Token, new DateTimeOffset(arg.ExpiresAt))
    {
    }

}
