using API.Catalogo.Context;
using API.Catalogo.Models;

namespace API.Catalogo.Repositories
{
  public class ProdutoRespository : Repository<Produto>, IProdutoRepository
  {
    public ProdutoRespository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Produto>> GetProdutosPorCategoriaAsync(int id)
    {
      return (await GetAllAsync()).Where(c => c.CategoriaId == id);      
    }
  }
}
