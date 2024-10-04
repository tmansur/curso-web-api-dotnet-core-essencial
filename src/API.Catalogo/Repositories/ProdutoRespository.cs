using API.Catalogo.Context;
using API.Catalogo.Models;
using API.Catalogo.Pagination;

namespace API.Catalogo.Repositories
{
  public class ProdutoRespository : Repository<Produto>, IProdutoRepository
  {
    public ProdutoRespository(AppDbContext context) : base(context)
    {
    }

    public async Task<PagedList<Produto>> GetProdutosAsync(ProdutosParameters produtosParams)
    {
      var produtos = (await GetAllAsync()).OrderBy(p => p.ProdutoId).AsQueryable();
      var produtosOrdernados = PagedList<Produto>.ToPagedList(produtos, produtosParams.PageNumber, produtosParams.PageSize);

      return produtosOrdernados;
    }

    public async Task<IEnumerable<Produto>> GetProdutosPorCategoriaAsync(int id)
    {
      return (await GetAllAsync()).Where(c => c.CategoriaId == id);      
    }
  }
}
