using API.Catalogo.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Catalogo.Context
{
  /// <summary>
  /// Classe de contexto responsável por realizar a comunicação entre as entidades de domínio (models) e o banco de dados relacional
  /// </summary>
  public class AppDbContext : DbContext
  {
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }

    public DbSet<Categoria>? Categorias { get; set; }
    public DbSet<Produto>? Produtos { get; set; }
  }
}
