using Client.Mvc.Models;
using System.Text.Json;
using System.Text;

namespace Client.Mvc.Services
{
  public class AutenticacaoService : IAutenticacaoService
  {
    private readonly IHttpClientFactory _clientFactory;
    const string apiEndpointAutentica = "/api/autoriza/login/";
    private readonly JsonSerializerOptions _options;
    private TokenViewModel tokenUsuario;

    public AutenticacaoService(IHttpClientFactory clientFactory)
    {
      _clientFactory = clientFactory;
      _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    }

    public async Task<TokenViewModel> AutenticaUsuario(UsuarioViewModel usuarioVM)
    {
      var client = _clientFactory.CreateClient("AutenticaApi");
      var usuario = JsonSerializer.Serialize(usuarioVM);
      StringContent content = new(usuario, Encoding.UTF8, "application/json");

      using (var response = await client.PostAsync(apiEndpointAutentica, content))
      {
        if (response.IsSuccessStatusCode)
        {
          var apiResponse = await response.Content.ReadAsStreamAsync();
          tokenUsuario = await JsonSerializer.DeserializeAsync<TokenViewModel>(apiResponse, _options);
        }
        else
        {
          return null;
        }
      }
      return tokenUsuario;
    }
  }
}
