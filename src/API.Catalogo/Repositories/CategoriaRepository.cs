using API.Catalogo.Context;
using API.Catalogo.Models;

namespace API.Catalogo.Repositories
{
  public class CategoriaRepository : Repository<Categoria>, ICategoriaRepository
  {
    public CategoriaRepository(AppDbContext context) : base(context)
    {
    }   
  }
}
