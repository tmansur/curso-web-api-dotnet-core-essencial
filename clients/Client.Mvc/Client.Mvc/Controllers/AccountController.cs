using Client.Mvc.Models;
using Client.Mvc.Services;
using Microsoft.AspNetCore.Mvc;

namespace Client.Mvc.Controllers
{
  public class AccountController : Controller
  {
    private readonly IAutenticacaoService _autenticacaoService;

    public AccountController(IAutenticacaoService autenticacaoService)
    {
      _autenticacaoService = autenticacaoService;
    }

    [HttpGet]
    public ActionResult Login()
    {
      return View();
    }

    [HttpPost]
    public async Task<ActionResult> Login(UsuarioViewModel model)
    {
      //verifica se o modelo é válido
      if (!ModelState.IsValid)
      {
        ModelState.AddModelError(string.Empty, "Login Inválido....");
        return View(model);
      }
      //verifica as credenciais do usuário e retorna um valor
      var result = await _autenticacaoService.AutenticaUsuario(model);


      if (result is null)
      {
        ModelState.AddModelError(string.Empty, "Login Inválido....");
        return View(model);
      }

      //Armazena o token no cookie
      Response.Cookies.Append("X-Access-Token", result.Token, new CookieOptions() //Adiciona o cookie na coleção de cookies do Response identificando-no como X-Acccess-Token
      {
        Secure = true, //Protege o cookie durante o transporte, garantindo que sempre será enviado via HTTPs
        HttpOnly = true, //Necessário para impedir ataques Cross Site Scripting, onde scripts do lado do client tentam acessar o cookie
        SameSite = SameSiteMode.Strict //Evita falsificação de solicitação entre sites (Cross-Site Forgery)
      });

      return Redirect("/");
    }
  }
}
