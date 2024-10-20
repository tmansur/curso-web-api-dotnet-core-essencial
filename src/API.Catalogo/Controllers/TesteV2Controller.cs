using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace API.Catalogo.Controllers
{
  [Route("api/v{version:apiVersion}/teste")]
  [ApiController]
  [ApiVersion("2.0")]
  //[ApiExplorerSettings(IgnoreApi = true)] //Endpoints do controller não são exibidos no swagger
  public class TesteV2Controller : ControllerBase
  {
    [HttpGet]
    public string GetVersion()
    {
      return "TesteV2 - GET - Api Version 2.0";
    }
  }
}
