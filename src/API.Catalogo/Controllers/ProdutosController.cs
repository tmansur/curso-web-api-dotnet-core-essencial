using API.Catalogo.Context;
using API.Catalogo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
      var produtos = _context.Produtos.AsNoTracking().ToList(); // Utiliza-se .AsNoTracking() para otimizar consultas que possuem ação apenas de leitura

      if (produtos is null)
      {
        return NotFound("Produtos não encontrados");
      }

      return produtos;
    }


    [HttpGet("{id:int}", Name = "ObterProduto")] //Só aceita parâmetro do tipo int e está nomeado como ObterProduto
    public ActionResult<Produto> Get(int id)
    {
      var produto = _context.Produtos.AsNoTracking().FirstOrDefault(p => p.ProdutoId == id);

      if (produto == null)
      {
        return NotFound($"Produto com id = {id} não encontrado");
      }

      return produto;
    }

    [HttpPost]
    public ActionResult Post(Produto produto) //Não é mais necessário utilizar o atributo [FromBody] para definir que a informação vem via body no request
    {
      // Validaçaõ de ModelState é feita de forma automatica, por isso não é mais necessário o código a seguir
      //if(!ModelState.IsValid)
      //{
      //  return BadRequest(ModelState);
      //}

      if (produto is null)
      {
        return BadRequest();
      }

      _context.Produtos.Add(produto);
      _context.SaveChanges();

      //Retorna 201 e os dados do produto criado, para isso executa o endpoint ObterProduto passando o id como parâmetro
      return new CreatedAtRouteResult("ObterProduto", new { id = produto.ProdutoId }, produto);
    }

    [HttpPut("{id:int}")] //PUT atualiza todos os campos do objeto
    public ActionResult Put(int id, Produto produto)
    {
      if (id != produto.ProdutoId)
      {
        return BadRequest();
      }

      _context.Entry(produto).State = EntityState.Modified;
      _context.SaveChanges();

      return Ok(produto);
    }

    [HttpDelete("{id:int}")]
    public ActionResult Delete(int id)
    {
      var produto = _context.Produtos.FirstOrDefault(p => p.ProdutoId == id);

      if (produto is null) return NotFound("Produto não encontrado");

      _context.Produtos.Remove(produto);
      _context.SaveChanges();

      return Ok(produto);
    }
  }
}
