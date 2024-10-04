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

    public async Task<PagedList<Categoria>> GetCategoriasFiltroNomeAsync(CategoriasFiltroNome categoriasParams)
    {
      var categorias = (await GetAllAsync()).AsQueryable();

      if(!string.IsNullOrEmpty(categoriasParams.Nome))
      {
        categorias = categorias.Where(c => c.Nome.Contains(categoriasParams.Nome));
      }

      var categoriasFiltradas = PagedList<Categoria>.ToPagedList(
        categorias,
        categoriasParams.PageNumber,
        categoriasParams.PageSize);

      return categoriasFiltradas;
    }
  }
}
