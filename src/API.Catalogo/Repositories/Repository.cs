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
      return await _context.Set<T>().ToListAsync(); //Set<T> -> método usado para acessar uma coleção/tabela, equivale por exemplo a _context.Produtos
    }    

    public async Task<T?> GetAsync(Expression<Func<T, bool>> predicate)
    {
      return await _context.Set<T>().FirstOrDefaultAsync(predicate);
    }    

    public async Task<T> CreateAsync(T entity)
    {
      await _context.Set<T>().AddAsync(entity);
      await _context.SaveChangesAsync();

      return entity;
    }

    public async Task<T> UpdateAsync(T entity)
    {
      _context.Set<T>().Update(entity); //Recomendado a utilização do .Update quando a entidade já está sendo utilizada pelo contexto do EF
      await _context.SaveChangesAsync();

      return entity;
    }

    public async Task<T> DeleteAsync(T entity)
    {
      _context.Set<T>().Remove(entity);
      await _context.SaveChangesAsync();

      return entity;
    }
  }
}
