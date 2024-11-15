namespace API.Catalogo.Logging
{
  /// <summary>
  /// Classe de configuração do provedor de log personalizado
  /// </summary>
  public class CustomerLoggerProviderConfiguration
  {
    /// <summary>
    /// Nível mínimo de log a ser registrado
    /// </summary>
    public LogLevel LogLevel { get; set; } = LogLevel.Warning;

    /// <summary>
    /// Id do evento de log
    /// </summary>
    public int EventId { get; set; } = 0;
  }
}
