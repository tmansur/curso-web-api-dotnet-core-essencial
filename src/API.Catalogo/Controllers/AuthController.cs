using API.Catalogo.DTOs;
using API.Catalogo.Models;
using API.Catalogo.Services;
using Microsoft.AspNetCore.Authorization;
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
    private readonly ILogger<AuthController> _logger;

    public AuthController(ITokenService tokenService,
                          UserManager<ApplicationUser> userManager,
                          RoleManager<IdentityRole> roleManager,
                          IConfiguration configuration,
                          ILogger<AuthController> logger)
    {
      _tokenService = tokenService;
      _userManager = userManager;
      _roleManager = roleManager;
      _configuration = configuration;
      _logger = logger;
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
          new("id", user.UserName!)
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

    /// <summary>
    /// Gera um novo refresh token
    /// </summary>
    /// <param name="tokenDto"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
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

    /// <summary>
    /// Revoga o refresh token de um usuário
    /// </summary>
    /// <param name="username"></param>
    /// <returns></returns>    
    [HttpPost]
    [Authorize(Policy = "ExclusiveOnly")]
    [Route("revoke/{username}")]
    public async Task<IActionResult> Revoke(string username)
    {
      var user = await _userManager.FindByNameAsync(username);

      if (user == null) return BadRequest("Invalid username");

      user.RefreshToken = null;

      await _userManager.UpdateAsync(user);

      return NoContent();
    }

    /// <summary>
    /// Cria uma nova role
    /// </summary>
    /// <param name="roleName"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("create-role")]
    [Authorize(Policy = "SuperAdminOnly")]
    public async Task<IActionResult> CreateRole(string roleName)
    {
      var roleExist = await _roleManager.RoleExistsAsync(roleName);

      if(!roleExist)
      {
        var roleResult = await _roleManager.CreateAsync(new IdentityRole(roleName)); //Criar a role na tabela AspNetRoles

        if(roleResult.Succeeded)
        {
          _logger.LogInformation(1, "Roles Added");

          return StatusCode(StatusCodes.Status200OK, 
            new ResponseDto { Status = "Success", Message = $"Role {roleName} added successfully" });
        }

        _logger.LogInformation(2, "Error");

        return StatusCode(StatusCodes.Status400BadRequest, 
          new ResponseDto { Status = "Error", Message = $"Issue adding the new {roleName} role" });
      }
      return StatusCode(StatusCodes.Status400BadRequest, 
        new ResponseDto { Status = "Error", Message = "Role already exist" });
    }

    /// <summary>
    /// Adiciona role para usuário
    /// </summary>
    /// <param name="email"></param>
    /// <param name="roleName"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("add-user-to-role")]
    [Authorize(Policy = "SuperAdminOnly")]
    public async Task<IActionResult> AddUserToRole(string email, string roleName)
    {
      var user = await _userManager.FindByEmailAsync(email);

      if(user != null)
      {
        var result = await _userManager.AddToRoleAsync(user, roleName); //Identity: AspNetUserRoles

        if(result.Succeeded)
        {
          _logger.LogInformation(1, $"User {user.Email} added to the {roleName} role");

          return StatusCode(StatusCodes.Status200OK,
            new ResponseDto { Status = "Success", Message = $"User {user.Email} added to the {roleName} role" });
        }

        _logger.LogInformation(1, $"Error: Unable to add user {user.Email} to the {roleName} role");

        return StatusCode(StatusCodes.Status400BadRequest,
            new ResponseDto { Status = "Error", Message = $"Error: Unable to add user {user.Email} to the {roleName} role" });
      }

      return BadRequest(new { error = "Unable to find user" });
    }
  }
}
