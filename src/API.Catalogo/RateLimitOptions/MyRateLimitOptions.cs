namespace API.Catalogo.RateLimitOptions
{
  public class MyRateLimitOptions
  {
    public const string MyRateLimit = "MyRateLimit";
    public int PermitLimit { get; set; } = 2;
    public int Window { get; set; } = 5;
    public int QueueLimit { get; set; } = 2;
    public bool AutoReplenishment { get; set; } = false;
  }
}
