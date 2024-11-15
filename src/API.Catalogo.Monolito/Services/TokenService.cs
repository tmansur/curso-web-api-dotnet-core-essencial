using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace API.Catalogo.Services
{
  public class TokenService : ITokenService
  {
    public JwtSecurityToken GenerateAccessToken(IEnumerable<Claim> claims, IConfiguration configuration)
    {
      var key = configuration.GetSection("Jwt").GetValue<string>("SecretKey") ?? throw new InvalidOperationException("Invalid secret key");
      var privateKey = Encoding.UTF8.GetBytes(key); //Converte chave secreta para um array de bytes

      //Cria as credenciais para assinar o token que será gerado
      //SymmetricSecurityKey e SigningCredentials são utilizadas para configurar a chave de assinatura necessária para verficiar a autenticidade do token JWT
      var signingCredentials = new SigningCredentials(
        new SymmetricSecurityKey(privateKey),
        SecurityAlgorithms.HmacSha256Signature);

      var tokenDescription = new SecurityTokenDescriptor
      {
        Subject = new ClaimsIdentity(claims),
        Expires = DateTime.UtcNow.AddMinutes(configuration.GetSection("Jwt").GetValue<double>("TokenValidityInMinutes")),
        Audience = configuration.GetSection("Jwt").GetValue<string>("ValidAudience"),
        Issuer = configuration.GetSection("Jwt").GetValue<string>("ValidIssuer"),
        SigningCredentials = signingCredentials
      };

      var tokenHandler = new JwtSecurityTokenHandler();
      var token = tokenHandler.CreateJwtSecurityToken(tokenDescription);

      return token;
    }

    public string GenerateRefreshToken()
    {
      var secureRandomBytes = new byte[128];

      using var randomNumberGenerator = RandomNumberGenerator.Create();
      randomNumberGenerator.GetBytes(secureRandomBytes);

      var refreshToken = Convert.ToBase64String(secureRandomBytes);

      return refreshToken;
    }

    public ClaimsPrincipal GetClaimsPrincipalFromExpiredToken(string token, IConfiguration configuration)
    {
      var secretKey = configuration["Jwt:SecretKey"] ?? throw new InvalidOperationException("Invalid key");

      var tokenValidationParameters = new TokenValidationParameters
      {
        ValidateAudience = false,
        ValidateIssuer = false,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ValidateLifetime = false
      };

      var tokenHandler = new JwtSecurityTokenHandler();
      var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

      if(securityToken is not JwtSecurityToken jwtSecurityToken || 
        !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
      {
        throw new SecurityTokenException("Invalid token");
      }

      return principal;
    }
  }
}
