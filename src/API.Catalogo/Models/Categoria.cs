using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace API.Catalogo.Models
{
  /// <summary>
  /// Classe que representa o domínio de Categoria
  /// É uma classe anêmica pois define apenas propriedades
  /// </summary>
  public class Categoria
  {
    public Categoria()
    {
      Produtos = new Collection<Produto>(); //É uma BOA PRÁTICA inicializar as coleções definidas na classe
    }

    [Key]
    public int CategoriaId { get; set; }

    [Required]
    [StringLength(80)]
    public string? Nome { get; set; }

    [Required]
    [StringLength(300)]
    public string? ImagemUrl { get; set; }

    //Uma categoria pode ter N produtos -> relacionamento de um-para-muitos entre as tabelas Categorias e Produtos 
    public ICollection<Produto> Produtos { get; set; }
  }
}
