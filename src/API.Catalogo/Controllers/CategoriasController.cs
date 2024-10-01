using API.Catalogo.Filters;
using API.Catalogo.Models;
using API.Catalogo.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace API.Catalogo.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class CategoriasController : ControllerBase
  {
    private readonly IRepository<Categoria> _repository;
    private readonly ILogger<CategoriasController> _logger;

    public CategoriasController(IRepository<Categoria> repository, ILogger<CategoriasController> logger)
    {
      _repository = repository;
      _logger = logger;
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

      var categorias = await _repository.GetAllAsync();
      return Ok(categorias);
    }

    [HttpGet("{id:int}", Name = "ObterCategoria")]
    public async Task<ActionResult<Categoria>> GetAsync(int id)
    {
      _logger.LogInformation("================ GET api/categorias/id ================");
      var categoria = await _repository.GetAsync(c => c.CategoriaId == id);
      
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
      var categoria = await _repository.GetAsync(c => c.CategoriaId == id);

      if(categoria == null) return NotFound();

      var categoriaDeletada = await _repository.DeleteAsync(categoria);

      return Ok(categoriaDeletada);
    }
  }
}
