using System.Collections.Concurrent;

namespace API.Catalogo.Logging
{
  /// <summary>
  /// Responsável por criar instâncias de loggers personalizados
  /// </summary>
  public class CustomerLoggerProvider : ILoggerProvider
  {
    private readonly CustomerLoggerProviderConfiguration _loggerConfig;
    //Dicionário de logs onde a chave é o nome da categoria e o valor é uma instância de CustomerLogger
    private readonly ConcurrentDictionary<string, CustomerLogger> _loggers = new();

    public CustomerLoggerProvider(CustomerLoggerProviderConfiguration loggerConfig)
    {
      _loggerConfig = loggerConfig;
    }

    /// <summary>
    /// Cria logger para a categoria 
    /// </summary>
    /// <param name="categoryName">Nome de uma classe ou componente</param>
    /// <returns></returns>
    public ILogger CreateLogger(string categoryName)
    {
      return _loggers.GetOrAdd(categoryName, name => new CustomerLogger(name, _loggerConfig));
    }

    public void Dispose()
    {
      _loggers.Clear();
    }
  }
}
