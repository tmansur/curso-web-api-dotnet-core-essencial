using API.Catalogo.DTOs;
using API.Catalogo.DTOs.Mappings;
using API.Catalogo.Filters;
using API.Catalogo.Models;
using API.Catalogo.Pagination;
using API.Catalogo.Repositories;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Newtonsoft.Json;

namespace API.Catalogo.Controllers
{
  [ApiController]
  [Route("api/v{version:apiVersion}/[controller]")]
  [ApiVersion("1.0")]
  [EnableRateLimiting("fixedWindow")] //Aplica a limitação de taxa a todos endpoints  
  public class CategoriasController : ControllerBase
  {
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CategoriasController> _logger;

    public CategoriasController(IUnitOfWork unitOfWork, ILogger<CategoriasController> logger)
    {
      _unitOfWork = unitOfWork;
      _logger = logger;
    }



    //[HttpGet("produtos")] // rota: api/categorias/produtos
    //public async Task<ActionResult<IEnumerable<Categoria>>> GetCategoriasProdutosAsync()
    //{
    //  _logger.LogInformation("================ GET api/categorias/produtos ================");

    //  return await _context.Categorias.Include(p => p.Produtos).AsNoTracking().ToListAsync();
    //}

    [HttpGet("pagination")]
    public async Task<ActionResult<IEnumerable<CategoriaDto>>> GetAsync([FromQuery] CategoriasParameters categoriasParameters)
    {
      var categorias = await _unitOfWork.CategoriaRepository.GetCategoriasAsync(categoriasParameters);
      return ObterCategorias(categorias);
    }

    [HttpGet("filter/nome/pagination")]
    public async Task<ActionResult<IEnumerable<CategoriaDto>>> GetCategoriasFiltradasAsync([FromQuery] CategoriasFiltroNome categoriasFiltro)
    {
      var categoriasFiltradas = await _unitOfWork.CategoriaRepository.GetCategoriasFiltroNomeAsync(categoriasFiltro);
      return ObterCategorias(categoriasFiltradas);
    }

    private ActionResult<IEnumerable<CategoriaDto>> ObterCategorias(PagedList<Categoria> categoriasFiltradas)
    {
      var metadata = new
      {
        categoriasFiltradas.TotalCount,
        categoriasFiltradas.PageSize,
        categoriasFiltradas.CurrentPage,
        categoriasFiltradas.TotalPages,
        categoriasFiltradas.HasNext,
        categoriasFiltradas.HasPrevious
      };

      //Retorna informações sobre paginação no header do response
      Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(metadata));

      var categoriasDto = categoriasFiltradas.ConvertToDtoList();

      return Ok(categoriasDto);
    }

    /// <summary>
    /// Obtem lista de objetos Categoria
    /// </summary>
    /// <returns>Lista de objetos do tipo Categoria</returns>
    [HttpGet] // rota: api/categorias
    [ServiceFilter(typeof(ApiLoggingFilter))] //Filtro personalizado
    [DisableRateLimiting] //Desabilita a aplicação do rate limite configurado no controller
    public async Task<ActionResult<IEnumerable<CategoriaDto>>> GetAsync()
    {
      _logger.LogInformation("================ GET api/categoria ================");

      var categorias = await _unitOfWork.CategoriaRepository.GetAllAsync();
      var categoriasDto = categorias.ConvertToDtoList();

      return Ok(categoriasDto);
    }

    /// <summary>
    /// Obtem uma Categoria pelo seu Id
    /// </summary>
    /// <param name="id">Id da categoria</param>
    /// <returns>Objeto do tipo categoria</returns>
    [HttpGet("{id:int}", Name = "ObterCategoria")]
    public async Task<ActionResult<CategoriaDto>> GetAsync(int id)
    {
      _logger.LogInformation("================ GET api/categorias/id ================");
      
      var categoria = await _unitOfWork.CategoriaRepository.GetAsync(c => c.CategoriaId == id);
      
      if(categoria == null) return NotFound("Categoria não encontrada");

      var categoriaDto = categoria.ConvertoToDto();

      return Ok(categoriaDto);
    }

    /// <summary>
    /// Inclui uma nova categoria
    /// </summary>
    /// <remarks>
    /// Exemplo de request:
    /// 
    ///   POST api/categorias
    ///   {
    ///     "categoriaId": 1,
    ///     "nome": "categoria1",
    ///     "imagemUrl": "http://teste.com.br/imagem1.jpg"
    ///   }
    /// </remarks>
    /// <param name="categoriaDto">Objeto Categoria</param>
    /// <returns>O objeto do tipo categoria que foi incluido</returns>
    /// <remarks>Retorna um objeto categoria cadastro a partir do dto enviado</remarks>
    [HttpPost]
    public async Task<ActionResult<CategoriaDto>> PostAsync(CategoriaDto categoriaDto)
    {
      if (categoriaDto is null) return BadRequest();

      var categoria = categoriaDto.ConvertToEntity();

      var newCategoria = await _unitOfWork.CategoriaRepository.CreateAsync(categoria);
      await _unitOfWork.CommitAsync();

      var newCategoriaDto = newCategoria.ConvertoToDto();

      return new CreatedAtRouteResult("ObterCategoria", new { id = newCategoria.CategoriaId }, newCategoria);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<CategoriaDto>> PutAsync(int id, CategoriaDto categoriaDto)
    {
      if(id != categoriaDto.CategoriaId) return BadRequest();

      var categoria = categoriaDto.ConvertToEntity();
      var newCategoria = _unitOfWork.CategoriaRepository.Update(categoria);
      await _unitOfWork.CommitAsync();

      var newCategoriaDto = newCategoria.ConvertoToDto();

      return Ok(newCategoriaDto);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<CategoriaDto>> DeleteAsync(int id)
    {
      var categoria = await _unitOfWork.CategoriaRepository.GetAsync(c => c.CategoriaId == id);

      if(categoria == null) return NotFound();

      var categoriaDeletada = _unitOfWork.CategoriaRepository.Delete(categoria);
      await _unitOfWork.CommitAsync();

      var categoriaDtoDeletada = categoriaDeletada.ConvertoToDto();

      return Ok(categoriaDtoDeletada);
    }    
  }
}
