using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace API.Catalogo.Controllers
{
  [Route("api/v{version:apiVersion}/teste")]
  [ApiController]
  [ApiVersion("1.0", Deprecated = true)]
  //[ApiExplorerSettings(IgnoreApi = true)] //Endpoints do controller não são exibidos no swagger
  public class TesteV1Controller : ControllerBase
  {
    [HttpGet]
    public string GetVersion()
    {
      return "TesteV1 - GET - Api Version 1.0";
    }
  }
}
