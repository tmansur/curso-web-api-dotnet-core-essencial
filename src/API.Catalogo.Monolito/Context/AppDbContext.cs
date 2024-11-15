using API.Catalogo.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace API.Catalogo.Context
{
  /// <summary>
  /// Classe de contexto responsável por realizar a comunicação entre as entidades de domínio (models) e o banco de dados relacional
  /// </summary>
  public class AppDbContext : IdentityDbContext<ApplicationUser>
  {
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }

    public DbSet<Categoria>? Categorias { get; set; }
    public DbSet<Produto>? Produtos { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
      base.OnModelCreating(builder); //Executa o construtor da classe base 
    }
  }
}
