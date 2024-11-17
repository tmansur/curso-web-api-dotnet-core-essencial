using Client.Mvc.Models;

namespace Client.Mvc.Services
{
  public interface IAutenticacaoService
  {
    Task<TokenViewModel> AutenticaUsuario(UsuarioViewModel usuarioVM);
  }
}
