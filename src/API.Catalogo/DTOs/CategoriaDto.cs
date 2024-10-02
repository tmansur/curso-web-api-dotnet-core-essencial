using System.ComponentModel.DataAnnotations;

namespace API.Catalogo.DTOs
{
  public class CategoriaDto
  {
    public int CategoriaId { get; set; }
    public string? Nome { get; set; }
    public string? ImagemUrl { get; set; }
  }
}
