using API.Catalogo.Context;
using API.Catalogo.Models;
using API.Catalogo.Pagination;

namespace API.Catalogo.Repositories
{
  public class CategoriaRepository : Repository<Categoria>, ICategoriaRepository
  {
    public CategoriaRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<PagedList<Categoria>> GetCategoriasAsync(CategoriasParameters categoriasParameters)
    {
      var categorias = (await GetAllAsync()).OrderBy(c => c.CategoriaId).AsQueryable();
      var categoriasOrdernadas = PagedList<Categoria>.ToPagedList(categorias, categoriasParameters.PageNumber, categoriasParameters.PageSize);

      return categoriasOrdernadas;
    }
  }
}
