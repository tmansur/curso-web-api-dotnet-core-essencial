using System.ComponentModel.DataAnnotations;

namespace API.Catalogo.DTOs
{
  public class ProdutoDto
  {
    public int ProdutoId { get; set; }
    [Required]
    [StringLength(80)]
    public string? Nome { get; set; }
    [Required]
    [StringLength(300)]
    public string? Descricao { get; set; }
    public decimal Preco { get; set; }
    [Required]
    [StringLength(30)]
    public string? ImagemUrl { get; set; }
    public int CategoriaId { get; set; }
  }
}
