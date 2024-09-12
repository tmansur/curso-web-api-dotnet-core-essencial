using API.Catalogo.Context;
using API.Catalogo.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Catalogo.Controllers
{
  [ApiController]
  [Route("[controller]")]
  public class ProdutosController : ControllerBase
  {
    private readonly AppDbContext _context;

    public ProdutosController(AppDbContext context)
    {
      _context = context;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Produto>> Get()
    {
      var produtos = _context.Produtos.ToList();

      if(produtos is null)
      {
        return NotFound("Produtos não encontrados");
      }

      return produtos;
    }


    [HttpGet("{id:int}", Name="ObterProduto")] //Só aceita parâmetro do tipo int e está nomeado como ObterProduto
    public ActionResult<Produto> Get(int id)
    {
      var produto = _context.Produtos.FirstOrDefault(p => p.ProdutoId == id);

      if(produto == null)
      {
        return NotFound($"Produto com id = {id} não encontrado");
      }

      return produto;
    }    
  }
}
