namespace API.Catalogo.Models
{
  /// <summary>
  /// Classe que representa o domínio de Categoria
  /// É uma classe anêmica pois define apenas propriedades
  /// </summary>
  public class Categoria
  {
    public int CategoriaId { get; set; }
    public string? Nome { get; set; }
    public string? ImagemUrl { get; set; }
  }
}
