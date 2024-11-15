using API.Catalogo.Context;
using API.Catalogo.DTOs.Mappings;
using API.Catalogo.Repositories;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace ApiCatalogoTests.UnitTests
{
  public class ProdutosControllerTest
  {
    public IUnitOfWork _repository;
    public IMapper _mapper;

    public static DbContextOptions<AppDbContext> dbContextOptions { get; }
    public static string connectionString = "Server=localhost;DataBase=apicatalogodb;Uid=root;Pwd=root";   

    /// <summary>
    /// Construtor estático que será executado apenas uma vez quando a classe for carregada pela primeira vez
    /// Apesar do teste utilizar a infra para execução, o mais comum é mockar o acesso a infra
    /// </summary>
    static ProdutosControllerTest()
    {
      dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
        .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
        .Options;
    }

    public ProdutosControllerTest()
    {
      var config = new MapperConfiguration(configure =>
      {
        configure.AddProfile(new ProdutoDtoMappingProfile());
      });

      _mapper = config.CreateMapper();
      var context = new AppDbContext(dbContextOptions);
      _repository = new UnitOfWork(context);
    }
  }
}
