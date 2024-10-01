using API.Catalogo.Context;
using API.Catalogo.Filters;
using API.Catalogo.Models;
using API.Catalogo.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Catalogo.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class CategoriasController : ControllerBase
  {    
    private readonly ILogger _logger;
    private readonly ICategoriaRepository _repository;

    public CategoriasController(ILogger<CategoriasController> logger, ICategoriaRepository repository)
    {
      _logger = logger;
      _repository = repository;
    }

    //[HttpGet("produtos")] // rota: api/categorias/produtos
    //public async Task<ActionResult<IEnumerable<Categoria>>> GetCategoriasProdutosAsync()
    //{
    //  _logger.LogInformation("================ GET api/categorias/produtos ================");

    //  return await _context.Categorias.Include(p => p.Produtos).AsNoTracking().ToListAsync();
    //}

    [HttpGet] // rota: api/categorias
    [ServiceFilter(typeof(ApiLoggingFilter))] //Filtro personalizado
    public async Task<ActionResult<IEnumerable<Categoria>>> GetAsync()
    {
      _logger.LogInformation("================ GET api/categoria ================");

      var categorias = await _repository.GetCategoriasAsync();
      return Ok(categorias);
    }

    [HttpGet("{id:int}", Name = "ObterCategoria")]
    public async Task<ActionResult<Categoria>> GetAsync(int id)
    {
      _logger.LogInformation("================ GET api/categorias/id ================");
      var categoria = await _repository.GetCategoriaAsync(id);
      
      if(categoria == null) return NotFound("Categoria não encontrada");

      return Ok(categoria);
    }

    [HttpPost]
    public async Task<ActionResult> PostAsync(Categoria categoria)
    {
      if (categoria is null) return BadRequest();

      var newCategoria = await _repository.CreateAsync(categoria);
    
      return new CreatedAtRouteResult("ObterCategoria", new { id = newCategoria.CategoriaId }, newCategoria);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> PutAsync(int id, Categoria categoria)
    {
      if(id != categoria.CategoriaId) return BadRequest();

      var newCategoria = await _repository.UpdateAsync(categoria);

      return Ok(newCategoria);
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<Categoria>> DeleteAsync(int id)
    {
      var categoria = await _repository.GetCategoriaAsync(id);

      if(categoria == null) return NotFound();

      var categoriaDeletada = await _repository.DeleteAsync(id);

      return Ok(categoriaDeletada);
    }
  }
}
