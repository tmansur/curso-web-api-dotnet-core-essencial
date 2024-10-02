using API.Catalogo.Models;
using API.Catalogo.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace API.Catalogo.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class ProdutosController : ControllerBase
  {
    private readonly IUnitOfWork _unitOfWork;

    public ProdutosController(IUnitOfWork unitOfWork)
    {
      _unitOfWork = unitOfWork;
    }

    [HttpGet("categoria/{id}")]
    public async Task<ActionResult<IEnumerable<Produto>>> GetProdutosCategoria(int id)
    {
      var produto = await _unitOfWork.ProdutoRepository.GetProdutosPorCategoriaAsync(id);

      if (produto is null) return NotFound();

      return Ok(produto);
    }

    // rota: api/produtos
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Produto>>> GetAsync()
    {
      var produtos = await _unitOfWork.ProdutoRepository.GetAllAsync();

      if (produtos is null)
      {
        return NotFound("Produtos não encontrados");
      }

      return Ok(produtos);
    }

    // rota: api/produtos/{id}
    [HttpGet("{id:int:min(1)}", Name = "ObterProduto")] //Só aceita parâmetro do tipo int maior que 0 e está nomeado como ObterProduto
    public async Task<ActionResult<Produto>> GetAsync(int id)
    {
      var produto = await _unitOfWork.ProdutoRepository.GetAsync(p => p.ProdutoId == id);

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

      var novoProduto = await _unitOfWork.ProdutoRepository.CreateAsync(produto);
      await _unitOfWork.Commit();

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

      var produtoAtualizado = _unitOfWork.ProdutoRepository.Update(produto);
      await _unitOfWork.Commit();

      return Ok(produtoAtualizado);
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteAsync(int id)
    {
      var produto = await _unitOfWork.ProdutoRepository.GetAsync(p => p.ProdutoId == id);

      if (produto is null) return NotFound("Produto não encontrado");

      var produtoDeletado = _unitOfWork.ProdutoRepository.Delete(produto);
      await _unitOfWork.Commit();

      return Ok(produtoDeletado);
    }
  }
}
