namespace API.Catalogo.Models
{
  public class Produto
  {
    public int ProdutoId { get; set; }
    public string? Nome { get; set; }
    public string? Descricao { get; set; }
    public decimal Preco { get; set; }
    public string? ImagemUrl { get; set; }
    public float Estoque { get; set; }
    public DateTime DataCadastro { get; set; }

    //Relacionamento entre Produtos e Categorias (um produto pode ter apenas uma categoria relacionada)
    public int CategoriaId {  get; set; }
    public Categoria? Categoria { get; set; }
  }
}
