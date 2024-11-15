using API.Catalogo.Models;
using API.Catalogo.Pagination;

namespace API.Catalogo.Repositories
{
  public interface ICategoriaRepository : IRepository<Categoria>
  {
    Task<PagedList<Categoria>> GetCategoriasAsync(CategoriasParameters categoriasParameters);
    Task<PagedList<Categoria>> GetCategoriasFiltroNomeAsync(CategoriasFiltroNome categoriasFiltroNome);
  }
}
