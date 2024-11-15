using API.Catalogo.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace API.Catalogo.Repositories
{
  public class Repository<T> : IRepository<T> where T : class //restringe que o tipo T só pode ser uma class
  {
    protected readonly AppDbContext _context;

    public Repository(AppDbContext context)
    {
      _context = context;
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
      return await _context.Set<T>().AsNoTracking().ToListAsync(); //Set<T> -> método usado para acessar uma coleção/tabela, equivale por exemplo a _context.Produtos
    }    

    public async Task<T?> GetAsync(Expression<Func<T, bool>> predicate)
    {
      return await _context.Set<T>().AsNoTracking().FirstOrDefaultAsync(predicate);
    }    

    public async Task<T> CreateAsync(T entity)
    {
      await _context.Set<T>().AddAsync(entity);

      return entity;
    }

    public T Update(T entity)
    {
      _context.Set<T>().Update(entity); //Recomendado a utilização do .Update quando a entidade já está sendo utilizada pelo contexto do EF

      return entity;
    }

    public T Delete(T entity)
    {
      _context.Set<T>().Remove(entity);

      return entity;
    }
  }
}
