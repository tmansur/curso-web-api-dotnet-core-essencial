using API.Catalogo.Context;
using API.Catalogo.Filters;
using API.Catalogo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Catalogo.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class CategoriasController : ControllerBase
  {
    private readonly AppDbContext _context;
    private readonly ILogger _logger;

    public CategoriasController(AppDbContext context, ILogger<CategoriasController> logger)
    {
      _context = context;
      _logger = logger;
    }

    [HttpGet("produtos")] // rota: api/categorias/produtos
    public async Task<ActionResult<IEnumerable<Categoria>>> GetCategoriasProdutosAsync()
    {
      _logger.LogInformation("================ GET api/categorias/produtos ================");

      return await _context.Categorias.Include(p => p.Produtos).AsNoTracking().ToListAsync();
    }

    [HttpGet] // rota: api/categorias
    [ServiceFilter(typeof(ApiLoggingFilter))] //Filtro personalizado
    public async Task<ActionResult<IEnumerable<Categoria>>> GetAsync()
    {
      _logger.LogInformation("================ GET api/categoria ================");

      return await _context.Categorias.AsNoTracking().ToListAsync();
    }

    [HttpGet("{id:int}", Name = "ObterCategoria")]
    public async Task<ActionResult<Categoria>> GetAsync(int id)
    {
      _logger.LogInformation("================ GET api/categorias/id ================");
      var categoria = await _context.Categorias.AsNoTracking().FirstOrDefaultAsync(c => c.CategoriaId == id);

      
      if(categoria == null) return NotFound("Categoria não encontrada");

      return categoria;
    }

    [HttpPost]
    public async Task<ActionResult> PostAsync(Categoria categoria)
    {
      if (categoria is null) return BadRequest();

      await _context.Categorias.AddAsync(categoria);
      await _context.SaveChangesAsync();

      return new CreatedAtRouteResult("ObterCategoria", new { id = categoria.CategoriaId }, categoria);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> PutAsync(int id, Categoria categoria)
    {
      if(id != categoria.CategoriaId) return BadRequest();

      _context.Entry(categoria).State = EntityState.Modified;
      await _context.SaveChangesAsync();

      return Ok(categoria);
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<Categoria>> DeleteAsync(int id)
    {
      var categoria = await _context.Categorias.FirstOrDefaultAsync(c => c.CategoriaId == id);

      if(categoria == null) return NotFound();

      _context.Categorias.Remove(categoria);
      await _context.SaveChangesAsync();

      return Ok(categoria);
    }
  }
}
