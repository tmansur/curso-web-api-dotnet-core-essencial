using API.Catalogo.DTOs;
using API.Catalogo.DTOs.Mappings;
using API.Catalogo.Filters;
using API.Catalogo.Models;
using API.Catalogo.Pagination;
using API.Catalogo.Repositories;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace API.Catalogo.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
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
      var metadata = new
      {
        categorias.TotalCount,
        categorias.PageSize,
        categorias.CurrentPage,
        categorias.TotalPages,
        categorias.HasNext,
        categorias.HasPrevious
      };

      //Retorna informações sobre paginação no header do response
      Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(metadata));

      var categoriasDto = categorias.ConvertToDtoList();

      return Ok(categoriasDto);
    }

    [HttpGet] // rota: api/categorias
    [ServiceFilter(typeof(ApiLoggingFilter))] //Filtro personalizado
    public async Task<ActionResult<IEnumerable<CategoriaDto>>> GetAsync()
    {
      _logger.LogInformation("================ GET api/categoria ================");

      var categorias = await _unitOfWork.CategoriaRepository.GetAllAsync();
      var categoriasDto = categorias.ConvertToDtoList();

      return Ok(categoriasDto);
    }

    [HttpGet("{id:int}", Name = "ObterCategoria")]
    public async Task<ActionResult<CategoriaDto>> GetAsync(int id)
    {
      _logger.LogInformation("================ GET api/categorias/id ================");
      
      var categoria = await _unitOfWork.CategoriaRepository.GetAsync(c => c.CategoriaId == id);
      
      if(categoria == null) return NotFound("Categoria não encontrada");

      var categoriaDto = categoria.ConvertoToDto();

      return Ok(categoriaDto);
    }

    [HttpPost]
    public async Task<ActionResult<CategoriaDto>> PostAsync(CategoriaDto categoriaDto)
    {
      if (categoriaDto is null) return BadRequest();

      var categoria = categoriaDto.ConvertToEntity();

      var newCategoria = await _unitOfWork.CategoriaRepository.CreateAsync(categoria);
      await _unitOfWork.Commit();

      var newCategoriaDto = newCategoria.ConvertoToDto();

      return new CreatedAtRouteResult("ObterCategoria", new { id = newCategoria.CategoriaId }, newCategoria);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<CategoriaDto>> PutAsync(int id, CategoriaDto categoriaDto)
    {
      if(id != categoriaDto.CategoriaId) return BadRequest();

      var categoria = categoriaDto.ConvertToEntity();
      var newCategoria = _unitOfWork.CategoriaRepository.Update(categoria);
      await _unitOfWork.Commit();

      var newCategoriaDto = newCategoria.ConvertoToDto();

      return Ok(newCategoriaDto);
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<CategoriaDto>> DeleteAsync(int id)
    {
      var categoria = await _unitOfWork.CategoriaRepository.GetAsync(c => c.CategoriaId == id);

      if(categoria == null) return NotFound();

      var categoriaDeletada = _unitOfWork.CategoriaRepository.Delete(categoria);
      await _unitOfWork.Commit();

      var categoriaDtoDeletada = categoriaDeletada.ConvertoToDto();

      return Ok(categoriaDtoDeletada);
    }    
  }
}
