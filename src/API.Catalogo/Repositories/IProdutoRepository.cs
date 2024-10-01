using API.Catalogo.Models;

namespace API.Catalogo.Repositories
{
  public interface IProdutoRepository
  {
    IQueryable<Produto> GetProdutosAsync();
    Task<Produto> GetProdutoAsync(int id);
    Task<Produto> CreateAsync(Produto produto);
    Task<bool> UpdateAsync(Produto produto);
    Task<bool> DeleteAsync(int id);
  }
}
