using API.Catalogo.Context;
using API.Catalogo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Catalogo.Controllers
{
  [ApiController]
  [Route("[controller]")]
  public class CategoriasController : ControllerBase
  {
    private readonly AppDbContext _context;

    public CategoriasController(AppDbContext context)
    {
      _context = context;
    }

    [HttpGet("produtos")] // rota: /categorias/produtos
    public ActionResult<IEnumerable<Categoria>> GetCategoriasProdutos()
    {
      return _context.Categorias.Include(p => p.Produtos).AsNoTracking().ToList();
    }

    [HttpGet] // rota: /categorias
    public ActionResult<IEnumerable<Categoria>> Get()
    {
      return _context.Categorias.AsNoTracking().ToList();
    }

    [HttpGet("{id:int}", Name = "ObterCategoria")]
    public ActionResult<Categoria> Get(int id)
    {
      var categoria = _context.Categorias.AsNoTracking().FirstOrDefault(c => c.CategoriaId == id);
      
      if(categoria == null) return NotFound("Categoria não encontrada");

      return categoria;
    }

    [HttpPost]
    public ActionResult Post(Categoria categoria)
    {
      if (categoria is null) return BadRequest();

      _context.Categorias.Add(categoria);
      _context.SaveChanges();

      return new CreatedAtRouteResult("ObterCategoria", new { id = categoria.CategoriaId }, categoria);
    }

    [HttpPut("{id:int}")]
    public ActionResult Put(int id, Categoria categoria)
    {
      if(id != categoria.CategoriaId) return BadRequest();

      _context.Entry(categoria).State = EntityState.Modified;
      _context.SaveChanges();

      return Ok(categoria);
    }

    [HttpDelete("{id:int}")]
    public ActionResult<Categoria> Delete(int id)
    {
      var categoria = _context.Categorias.FirstOrDefault(c => c.CategoriaId == id);

      if(categoria == null) return NotFound();

      _context.Categorias.Remove(categoria);
      _context.SaveChanges();

      return Ok(categoria);
    }
  }
}
