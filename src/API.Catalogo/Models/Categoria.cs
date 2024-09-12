using System.Collections.ObjectModel;

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

    public int CategoriaId { get; set; }

    public string? Nome { get; set; }

    public string? ImagemUrl { get; set; }

    //Uma categoria pode ter N produtos -> relacionamento de um-para-muitos entre as tabelas Categorias e Produtos 
    public ICollection<Produto> Produtos { get; set; }
  }
}
