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

    public async Task<PagedList<Produto>> GetProdutosFiltroAsync(ProdutosFiltroPreco produtosFiltroParams)
    {
      var produtos = (await GetAllAsync()).AsQueryable();

      if (produtosFiltroParams.Preco.HasValue && !string.IsNullOrEmpty(produtosFiltroParams.PrecoCriterio))
      {
        if (produtosFiltroParams.PrecoCriterio.Equals("maior", StringComparison.OrdinalIgnoreCase))
        {
          produtos = produtos.Where(p => p.Preco > produtosFiltroParams.Preco.Value).OrderBy(p => p.Preco);
        }
        else if (produtosFiltroParams.PrecoCriterio.Equals("menor", StringComparison.OrdinalIgnoreCase))
        {
          produtos = produtos.Where(p => p.Preco < produtosFiltroParams.Preco.Value).OrderBy(p => p.Preco);
        }
        else if (produtosFiltroParams.PrecoCriterio.Equals("igual", StringComparison.OrdinalIgnoreCase))
        {
          produtos = produtos.Where(p => p.Preco == produtosFiltroParams.Preco.Value).OrderBy(p => p.Preco);
        }
      }

      var produtosFiltrados = PagedList<Produto>.ToPagedList(
        produtos, 
        produtosFiltroParams.PageNumber,
        produtosFiltroParams.PageSize);
     
      return produtosFiltrados;
    }

    public async Task<IEnumerable<Produto>> GetProdutosPorCategoriaAsync(int id)
    {
      return (await GetAllAsync()).Where(c => c.CategoriaId == id);      
    }
  }
}
