
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace API.Catalogo.Logging
{
  /// <summary>
  /// Possui os métodos necessários para registrar mensagens de log
  /// </summary>
  public class CustomerLogger : ILogger
  {

    private readonly string _loggerName;
    private readonly CustomerLoggerProviderConfiguration _loggerConfig;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name">Nome de uma classe ou componente</param>
    /// <param name="loggerConfig"></param>
    public CustomerLogger(string name, CustomerLoggerProviderConfiguration loggerConfig)
    {
      _loggerName = name;
      _loggerConfig = loggerConfig;
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
      return null;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
      return logLevel == _loggerConfig.LogLevel;
    }

    /// <summary>
    /// Registra mensagem de log
    /// </summary>
    /// <typeparam name="TState"></typeparam>
    /// <param name="logLevel"></param>
    /// <param name="eventId"></param>
    /// <param name="state"></param>
    /// <param name="exception"></param>
    /// <param name="formatter"></param>
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
      string mensagem = $"{logLevel.ToString()}: {eventId.Id} - {formatter(state, exception)}";

      EscreverTextoNoArquivo(mensagem);
    }

    public void EscreverTextoNoArquivo(string mensagem)
    {
      string path = @"C:\temp\customlog.txt";

      using var streamWrite = new StreamWriter(path, true);
      try
      {
        streamWrite.WriteLine(mensagem);
        streamWrite.Close();
      }
      catch (Exception)
      {
        throw;
      }
    }
  }
}
