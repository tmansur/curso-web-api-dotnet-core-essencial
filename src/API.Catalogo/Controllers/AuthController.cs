using API.Catalogo.DTOs;
using API.Catalogo.Models;
using API.Catalogo.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace API.Catalogo.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class AuthController : ControllerBase
  {
    private readonly ITokenService _tokenService;
    private readonly UserManager<ApplicationUser> _userManager; //Administração de usuários no Identity
    private readonly RoleManager<IdentityRole> _roleManager; //Administração de roles no Identity
    private readonly IConfiguration _configuration;

    public AuthController(ITokenService tokenService, 
                          UserManager<ApplicationUser> userManager, 
                          RoleManager<IdentityRole> roleManager, 
                          IConfiguration configuration)
    {
      _tokenService = tokenService;
      _userManager = userManager;
      _roleManager = roleManager;
      _configuration = configuration;
    }

    /// <summary>
    /// Permite que usuário faça autenticação
    /// </summary>
    /// <param name="loginDto"></param>
    /// <returns>token e refresh token</returns>
    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login(LoginDto loginDto)
    {
      var user = await _userManager.FindByNameAsync(loginDto.Username!);

      if(user is not null && await _userManager.CheckPasswordAsync(user, loginDto.Password!))
      {
        var userRoles = await _userManager.GetRolesAsync(user); //Busca perfis do usuário

        var authClaims = new List<Claim>
        {
          new(ClaimTypes.Name, user.UserName!),
          new(ClaimTypes.Email, user.Email!),
          new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        foreach(var userRole in userRoles)
        {
          authClaims.Add(new Claim(ClaimTypes.Role, userRole)); //Inclui os perfis do usuário no token
        }

        var token = _tokenService.GenerateAccessToken(authClaims, _configuration); //Gera token
        var refreshToken = _tokenService.GenerateRefreshToken(); //Gerar refresh token

        //Converte o valor de validade em minutos do token para int e o atribui a variável refreshTokenValidityInMinutes
        _ = int.TryParse(_configuration["Jwt:RefreshTokenValidityInMinutes"], out int refreshTokenValidityInMinutes);

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(refreshTokenValidityInMinutes);

        await _userManager.UpdateAsync(user); //Atualiza tabela AspNetUser

        return Ok(new
        {
          Token = new JwtSecurityTokenHandler().WriteToken(token),
          RefreshToken = refreshToken,
          Expiration = token.ValidTo
        });
      }

      return Unauthorized();
    }

    /// <summary>
    /// Registra novo usuário
    /// </summary>
    /// <param name="registerDto"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register(RegisterDto registerDto)
    {
      var userExists = await _userManager.FindByNameAsync(registerDto.Username!);

      if(userExists is not null)
      {
        return StatusCode(StatusCodes.Status500InternalServerError,
          new ResponseDto { Status = "Error", Message = "User already exists!" });
      }

      //Cria nova instancia de ApplicationUser com as informações enviadas
      ApplicationUser user = new()
      {
        Email = registerDto.Email,
        SecurityStamp = Guid.NewGuid().ToString(),
        UserName = registerDto.Username
      };

      //Cria o novo usuário
      var result = await _userManager.CreateAsync(user, registerDto.Password!);

      if(!result.Succeeded)
      {
        return StatusCode(StatusCodes.Status500InternalServerError,
          new ResponseDto { Status = "Error", Message = "User creation failed." });
      }

      return Ok(new ResponseDto { Status = "Success", Message = "User created successfully!" });
    }

    //RefreshToken -> gera um novo refresh token
    [HttpPost]
    [Route("refresh-token")]
    public async Task<IActionResult> RefreshToken(TokenDto tokenDto)
    {
      if(tokenDto is null)
      {
        return BadRequest("Invalid client request");
      }

      string? accessToken = tokenDto.AccessToken ?? throw new ArgumentNullException(nameof(tokenDto));
      string? refreshToken = tokenDto.RefreshToken ?? throw new ArgumentNullException(nameof(tokenDto));

      //Obtem as claims presentes no token expirado
      var claims = _tokenService.GetClaimsPrincipalFromExpiredToken(accessToken!, _configuration);

      if(claims is null)
      {
        return BadRequest("Invalid access token/refresh token");
      }

      string username = claims.Identity.Name;
      var user = await _userManager.FindByNameAsync(username!);

      if(user is null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
      {
        return BadRequest("Invalid access token/refresh token");
      }

      var newAccessToken = _tokenService.GenerateAccessToken(claims.Claims.ToList(), _configuration);
      var newRefreshToken = _tokenService.GenerateRefreshToken();

      user.RefreshToken = newRefreshToken;

      await _userManager.UpdateAsync(user);

      return new ObjectResult(new
      {
        accessToken = new JwtSecurityTokenHandler().WriteToken(newAccessToken),
        refreshToken = newRefreshToken
      });
    }

    //Revoke -> revogar um refresh token
  }
}
