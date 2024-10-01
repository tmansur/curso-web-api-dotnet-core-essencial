using API.Catalogo.Models;

namespace API.Catalogo.Repositories
{
  public interface IProdutoRepository : IRepository<Produto>
  {
    Task<IEnumerable<Produto>> GetProdutosPorCategoriaAsync(int id);
  }
}
