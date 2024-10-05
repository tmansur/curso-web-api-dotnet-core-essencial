using Microsoft.AspNetCore.Identity;

namespace API.Catalogo.Models
{
  //Extende a classe IdentityUser, que representa o usuário do Identity, para incluir 2 colunas na tabela AspNetUsers
  public class ApplicationUser : IdentityUser
  {
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; }
  }
}
