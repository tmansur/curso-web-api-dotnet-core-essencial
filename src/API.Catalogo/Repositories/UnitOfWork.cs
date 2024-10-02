
using API.Catalogo.Context;

namespace API.Catalogo.Repositories
{
  public class UnitOfWork : IUnitOfWork
  {
    private IProdutoRepository? _produtoRepository;
    private ICategoriaRepository? _categoriaRepository;
    public AppDbContext _context;    

    public UnitOfWork(AppDbContext context)
    {
      _context = context;
    }

    public IProdutoRepository ProdutoRepository
    {
      get
      {
        return _produtoRepository = _produtoRepository ?? new ProdutoRespository(_context);
      }
    }

    public ICategoriaRepository CategoriaRepository
    {
      get
      {
        return _categoriaRepository = _categoriaRepository ?? new CategoriaRepository(_context);
      }
    }

    public async Task Commit()
    {
      await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
      _context.Dispose();
    }
  }
}
