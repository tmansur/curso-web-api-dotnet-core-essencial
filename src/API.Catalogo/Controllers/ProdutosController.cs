using API.Catalogo.Context;
using API.Catalogo.Models;
using API.Catalogo.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Catalogo.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class ProdutosController : ControllerBase
  {
    private readonly IProdutoRepository _repository;

    public ProdutosController(IProdutoRepository repository)
    {
      _repository = repository;
    }

    // rota: api/produtos
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Produto>>> GetAsync()
    {
      var produtos = _repository.GetProdutosAsync().ToList(); //Ao fazer o .ToList() a consulta no banco de dados será executada

      if (produtos is null)
      {
        return NotFound("Produtos não encontrados");
      }

      return Ok(produtos);
    }

    //// rota: api/produtos/primeiro
    //[HttpGet("primeiro")] //modifica a rota para evitar duplicidade de rota com o endpoint que retorna todos os produtos
    //public async Task<ActionResult<Produto>> GetPrimeiroProdutoAsync()
    //{
    //  var primeiroProduto = await _context.Produtos.AsNoTracking().FirstOrDefaultAsync();

    //  if (primeiroProduto is null)
    //  {
    //    return NotFound("Produto não encontrado");
    //  }

    //  return primeiroProduto;
    //}

    // rota: api/produtos/{id}
    [HttpGet("{id:int:min(1)}", Name = "ObterProduto")] //Só aceita parâmetro do tipo int maior que 0 e está nomeado como ObterProduto
    public async Task<ActionResult<Produto>> GetAsync(int id)
    {
      var produto = await _repository.GetProdutoAsync(id);

      if (produto == null)
      {
        return NotFound($"Produto com id = {id} não encontrado");
      }

      return Ok(produto);
    }

    // rota: api/produtos
    [HttpPost]
    public async Task<ActionResult> PostAsync(Produto produto) //Não é mais necessário utilizar o atributo [FromBody] para definir que a informação vem via body no request
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

      var novoProduto = await _repository.CreateAsync(produto);

      //Retorna 201 e os dados do produto criado, para isso executa o endpoint ObterProduto passando o id como parâmetro
      return new CreatedAtRouteResult(
        "ObterProduto", 
        new { id = novoProduto.ProdutoId }, 
        novoProduto);
    }

    [HttpPut("{id:int}")] //PUT atualiza todos os campos do objeto
    public async Task<ActionResult> PutAsync(int id, Produto produto)
    {
      if (id != produto.ProdutoId)
      {
        return BadRequest();
      }

      var result = await _repository.UpdateAsync(produto);

      if (!result) return StatusCode(500, $"Falha ao atualizar o produto de id = {id}");

      return Ok(produto);
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteAsync(int id)
    {
      var produto = await _repository.GetProdutoAsync(id);

      if (produto is null) return NotFound("Produto não encontrado");

      var result = await _repository.DeleteAsync(id);

      if (!result) return StatusCode(500, $"Falha ao deletar o produto de id = {id}");

      return Ok($"Produto de id = {id} excluído");
    }
  }
}
