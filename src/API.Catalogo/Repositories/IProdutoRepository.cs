using API.Catalogo.Models;
using API.Catalogo.Pagination;

namespace API.Catalogo.Repositories
{
  public interface IProdutoRepository : IRepository<Produto>
  {
    Task<PagedList<Produto>> GetProdutosAsync(ProdutosParameters produtosParams);
    Task<IEnumerable<Produto>> GetProdutosPorCategoriaAsync(int id);
  }
}
