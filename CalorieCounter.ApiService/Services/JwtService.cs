using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CalorieCounter.ApiService;

public class JwtService
{
    private readonly string _key;
    private readonly string _iss;
    private readonly string _aud;

    public JwtService(IConfiguration config){
        _key = config["Jwt:Key"] ?? throw new InvalidOperationException("JWT KEY NOT FOUND");
        _iss = config["Jwt:Issuer"] ?? "";
        _aud = config["Jwt:Audience"] ?? "";
    }

    public (string Token, DateTime expiresAt) GenerateToken(User user)
    {
        var expiry = DateTime.UtcNow.AddDays(7);

        Claim[] claims = [ new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                           new Claim(ClaimTypes.Name, user.UserName ?? "")
                         ];

        var symKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));
        var creds = new SigningCredentials(symKey,SecurityAlgorithms.HmacSha256);
        
        var token = new JwtSecurityToken(issuer: _iss,
                                         audience:_aud,
                                         claims: claims,
                                         expires: expiry,
                                         signingCredentials: creds);

        return (new JwtSecurityTokenHandler().WriteToken(token), expiry);
    }

}
