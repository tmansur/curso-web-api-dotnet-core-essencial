using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Catalogo.Filters
{
  /// <summary>
  /// Filtro personalizado sincrono que realiza o log de registro nos métodos actions dos controladores
  /// </summary>
  public class ApiLoggingFilter : IActionFilter
  {
    private readonly ILogger<ApiLoggingFilter> _logger;

    public ApiLoggingFilter(ILogger<ApiLoggingFilter> logger)
    {
      _logger = logger;
    }

    /// <summary>
    /// Executa depois da execução da action do controller
    /// </summary>
    /// <param name="context"></param>
    /// <exception cref="NotImplementedException"></exception>
    public void OnActionExecuted(ActionExecutedContext context)
    {
      _logger.LogInformation("### Executando -> OnActionExecuted");
      _logger.LogInformation("###################################");
      _logger.LogInformation($"{DateTime.UtcNow.ToLongDateString()}");
      _logger.LogInformation($"StatusCode: {context.HttpContext.Response.StatusCode}");
      _logger.LogInformation("###################################");
    }

    /// <summary>
    /// Executa antes da execução da action do controller
    /// </summary>
    /// <param name="context"></param>
    /// <exception cref="NotImplementedException"></exception>
    public void OnActionExecuting(ActionExecutingContext context)
    {
      _logger.LogInformation("### Executando -> OnActionExecuting");
      _logger.LogInformation("###################################");
      _logger.LogInformation($"{DateTime.UtcNow.ToLongDateString()}");
      _logger.LogInformation($"ModelState: {context.ModelState.IsValid}");
      _logger.LogInformation("###################################");
    }
  }
}
