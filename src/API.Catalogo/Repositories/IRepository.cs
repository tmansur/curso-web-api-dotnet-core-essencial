using System.Linq.Expressions;

namespace API.Catalogo.Repositories
{
  public interface IRepository<T>
  {
    Task<IEnumerable<T>> GetAllAsync();
    //T Get(int id); //Função mais restrita pois só aceita um parâmetro específico (id do tipo int)

    //Expression => representa expressões de funções lambda
    //Func<T,bool> => delegate (função que pode ser passada como argumento)
    //predicate => critério utilizado para filtrar a função lambda
    //Método get que aceita como argumento uma expressão lambda que recebe um objeto do tipo T e retorna um booleano com base no predicate
    Task<T?> GetAsync(Expression<Func<T, bool>> predicate);
    Task<T> CreateAsync(T entity);
    Task<T> UpdateAsync(T entity);
    Task<T> DeleteAsync(T entity);
  }
}
