using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Catalogo.Filters
{
  /// <summary>
  /// Filtro para manipu~lação de exceções não tratadas
  /// </summary>
  public class ApiExceptionFilter : IExceptionFilter
  {
    private readonly ILogger<ApiExceptionFilter> _logger;

    public ApiExceptionFilter(ILogger<ApiExceptionFilter> logger)
    {
      _logger = logger;
    }

    public void OnException(ExceptionContext context)
    {
      _logger.LogError(context.Exception, "Ocorreu uma exceção não tratada: Status code 500");

      context.Result = new ObjectResult("Ocorreu um problema ao tratar sua solicitação: Status Code 500")
      {
        StatusCode = StatusCodes.Status500InternalServerError
      };
    }
  }
}
