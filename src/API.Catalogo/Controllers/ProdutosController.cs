using API.Catalogo.DTOs;
using API.Catalogo.Models;
using API.Catalogo.Pagination;
using API.Catalogo.Repositories;
using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace API.Catalogo.Controllers
{
  [ApiController]
  [Route("api/v{version:apiVersion}/[controller]")]
  [ApiVersion("1.0")]
  public class ProdutosController : ControllerBase
  {
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ProdutosController(IUnitOfWork unitOfWork, IMapper mapper)
    {
      _unitOfWork = unitOfWork;
      _mapper = mapper;
    }

    /// <summary>
    /// Obtem produto pelo seu identificador
    /// </summary>
    /// <param name="id">Identificador do produto</param>
    /// <returns>Objeto produto</returns>
    [HttpGet("categoria/{id}")]
    public async Task<ActionResult<IEnumerable<ProdutoDto>>> GetProdutosCategoria(int id)
    {
      var produtos = await _unitOfWork.ProdutoRepository.GetProdutosPorCategoriaAsync(id);

      if (produtos is null) return NotFound();
      
      var produtosDto = _mapper.Map<IEnumerable<ProdutoDto>>(produtos);
      return Ok(produtosDto);
    }

    /// <summary>
    /// Obtém lista de produtos
    /// </summary>
    /// <returns>Retorna lista de objeto do tipo Produto</returns>
    // rota: api/produtos
    [HttpGet]
    [Authorize(Policy = "UserOnly")]
    public async Task<ActionResult<IEnumerable<ProdutoDto>>> GetAsync()
    {
      var produtos = await _unitOfWork.ProdutoRepository.GetAllAsync();

      if (produtos is null)
      {
        return NotFound("Produtos não encontrados");
      }

      var produtosDto = _mapper.Map<IEnumerable<ProdutoDto>>(produtos);

      return Ok(produtosDto);
    }

    [HttpGet("pagination")]
    public async Task<ActionResult<IEnumerable<ProdutoDto>>> GetAsync([FromQuery]ProdutosParameters produtosParameters)
    {
      var produtos = await _unitOfWork.ProdutoRepository.GetProdutosAsync(produtosParameters);
      var metadata = new
      {
        produtos.TotalCount,
        produtos.PageSize,
        produtos.CurrentPage,
        produtos.TotalPages,
        produtos.HasNext,
        produtos.HasPrevious
      };

      //Retorna informações sobre paginação no header do response
      Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(metadata));

      var produtosDto = _mapper.Map<IEnumerable<ProdutoDto>>(produtos);

      return Ok(produtosDto);
    }

    [HttpGet("filter/preco/pagination")]
    public async Task<ActionResult<IEnumerable<ProdutoDto>>> GetProdutosFilterPreco([FromQuery] ProdutosFiltroPreco produtosFilterParameters)
    {
      var produtos = await _unitOfWork.ProdutoRepository.GetProdutosFiltroAsync(produtosFilterParameters);
      return ObterProdutos(produtos);
    }
    private ActionResult<IEnumerable<ProdutoDto>> ObterProdutos(PagedList<Produto> produtos)
    {
      var metadata = new
      {
        produtos.TotalCount,
        produtos.PageSize,
        produtos.CurrentPage,
        produtos.TotalPages,
        produtos.HasNext,
        produtos.HasPrevious
      };

      Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(metadata));
      var produtosDto = _mapper.Map<IEnumerable<ProdutoDto>>(produtos);
      
      return Ok(produtosDto);
    }

    // rota: api/produtos/{id}
    [HttpGet("{id:int:min(1)}", Name = "ObterProduto")] //Só aceita parâmetro do tipo int maior que 0 e está nomeado como ObterProduto
    public async Task<ActionResult<ProdutoDto>> GetAsync(int id)
    {
      var produto = await _unitOfWork.ProdutoRepository.GetAsync(p => p.ProdutoId == id);

      if (produto == null)
      {
        return NotFound($"Produto com id = {id} não encontrado");
      }

      var produtoDto = _mapper.Map<ProdutoDto>(produto);

      return Ok(produtoDto);
    }

    // rota: api/produtos
    [HttpPost]
    public async Task<ActionResult<ProdutoDto>> PostAsync(ProdutoDto produtoDto) //Não é mais necessário utilizar o atributo [FromBody] para definir que a informação vem via body no request
    {
      // Validaçaõ de ModelState é feita de forma automatica, por isso não é mais necessário o código a seguir
      //if(!ModelState.IsValid)
      //{
      //  return BadRequest(ModelState);
      //}

      if (produtoDto is null)
      {
        return BadRequest();
      }

      var produto = _mapper.Map<Produto>(produtoDto);

      var produtoNovo = await _unitOfWork.ProdutoRepository.CreateAsync(produto);
      await _unitOfWork.CommitAsync();

      var produtoDtoNovo = _mapper.Map<ProdutoDto>(produtoNovo);

      //Retorna 201 e os dados do produto criado, para isso executa o endpoint ObterProduto passando o id como parâmetro
      return new CreatedAtRouteResult(
        "ObterProduto", 
        new { id = produtoDtoNovo.ProdutoId },
        produtoDtoNovo);
    }

    //Exemplo de body para executar alteração parcial:
    //[{ "path": "/estoque", "op":"replace", "value": 350  }, { "path": "/datacadastro", "op":"replace", "value": "2024-10-01 09:14:00"  }]
    [HttpPatch("{id}/updatepartial")]
    public async Task<ActionResult<ProdutoUpdateResponseDto>> Patch(int id, JsonPatchDocument<ProdutoUpdateRequestDto> produtoDto)
    {
      if (produtoDto is null || id <= 0) return BadRequest();

      var produto = await _unitOfWork.ProdutoRepository.GetAsync(p => p.ProdutoId == id);

      if (produto is null) return NotFound();

      var produtoUpdateRequestDto = _mapper.Map<ProdutoUpdateRequestDto>(produto);

      produtoDto.ApplyTo(produtoUpdateRequestDto, ModelState); //Aplica as alterações parciais ao objeto produtoUpdateRequestDto

      if (!ModelState.IsValid || !TryValidateModel(produtoUpdateRequestDto)) return BadRequest(ModelState);

      _mapper.Map(produtoUpdateRequestDto, produto);

      _unitOfWork.ProdutoRepository.Update(produto);
      await _unitOfWork.CommitAsync();

      return Ok(_mapper.Map<ProdutoUpdateResponseDto>(produto));
    }


    [HttpPut("{id:int}")] //PUT atualiza todos os campos do objeto
    public async Task<ActionResult<ProdutoDto>> PutAsync(int id, ProdutoDto produtoDto)
    {
      if (id != produtoDto.ProdutoId)
      {
        return BadRequest();
      }

      var produto = _mapper.Map<Produto>(produtoDto);

      var produtoAtualizado = _unitOfWork.ProdutoRepository.Update(produto);
      await _unitOfWork.CommitAsync();

      var produtoDtoAtualizado = _mapper.Map<ProdutoDto>(produtoAtualizado);

      return Ok(produtoDtoAtualizado);
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ProdutoDto>> DeleteAsync(int id)
    {
      var produto = await _unitOfWork.ProdutoRepository.GetAsync(p => p.ProdutoId == id);

      if (produto is null) return NotFound("Produto não encontrado");

      var produtoDeletado = _unitOfWork.ProdutoRepository.Delete(produto);
      await _unitOfWork.CommitAsync();

      var produtoDtoDeletado = _mapper.Map<ProdutoDto>(produtoDeletado);

      return Ok(produtoDtoDeletado);
    }
  }
}
