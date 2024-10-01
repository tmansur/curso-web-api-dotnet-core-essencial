using API.Catalogo.Context;
using API.Catalogo.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Catalogo.Repositories
{
  public class CategoriaRepository : ICategoriaRepository
  {
    private readonly AppDbContext _context;

    public CategoriaRepository(AppDbContext context)
    {
      _context = context;
    }

    public async Task<IEnumerable<Categoria>> GetCategoriasAsync()
    {
      return await _context.Categorias.AsNoTracking().ToListAsync();
    }

    public async Task<Categoria> GetCategoriaAsync(int id)
    {
      return await _context.Categorias.AsNoTracking().FirstOrDefaultAsync(c => c.CategoriaId == id);
    }   

    public async Task<Categoria> CreateAsync(Categoria categoria)
    {
      if(categoria is null) throw new ArgumentNullException(nameof(categoria));

      await _context.Categorias.AddAsync(categoria);
      await _context.SaveChangesAsync();

      return categoria;
    }

    public async Task<Categoria> UpdateAsync(Categoria categoria)
    {
      if (categoria is null) throw new ArgumentNullException(nameof(categoria));

      _context.Entry(categoria).State = EntityState.Modified;
      await _context.SaveChangesAsync();

      return categoria;
    }

    public async Task<Categoria> DeleteAsync(int id)
    {
      var categoria = await _context.Categorias.FindAsync(id);

      if (categoria is null) throw new ArgumentNullException(nameof(categoria));

      _context.Categorias.Remove(categoria);
      await _context.SaveChangesAsync();

      return categoria;
    }       
  }
}
