using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace API.Catalogo.Services
{
  public interface ITokenService
  {
    /// <summary>
    /// Gera o access token
    /// </summary>
    /// <param name="claims"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    JwtSecurityToken GenerateAccessToken(IEnumerable<Claim> claims, IConfiguration configuration);

    /// <summary>
    /// Gera o refresh token
    /// </summary>
    /// <returns></returns>
    string GenerateRefreshToken();

    /// <summary>
    /// Extrai as informações das claims do token expirado
    /// </summary>
    /// <param name="token"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    ClaimsPrincipal GetClaimsPrincipalFromExpiredToken(string token, IConfiguration configuration);
  }
}
