using API.Catalogo.Context;
using API.Catalogo.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Catalogo.Repositories
{
  public class ProdutoRespository : IProdutoRepository
  {
    private readonly AppDbContext _context;

    public ProdutoRespository(AppDbContext context)
    {
      _context = context;
    }

    public IQueryable<Produto> GetProdutosAsync()
    {
      //Retorna uma consulta IQueryable que representa a seleção de todos produtos
      //Não executa a consulta no banco de dados nesse momento => Lazy Loading
      //Isso permite que sejam adicionados filtros, clausulas, ordenações, etc, antes da execução da consulta
      //Minimiza a quantidade de dados transferidos do bd para a aplicação
      //Exemplo de utilização: fazer páginação dos dados retornado
      return _context.Produtos;
    }

    public async Task<Produto> GetProdutoAsync(int id)
    {
      var produto = await _context.Produtos.AsNoTracking().FirstOrDefaultAsync(p => p.ProdutoId == id);

      if (produto is null) throw new ArgumentNullException(nameof(produto));

      return produto;
    }
    
    public async Task<Produto> CreateAsync(Produto produto)
    {
      if (produto is null) throw new ArgumentNullException(nameof(produto));

      await _context.Produtos.AddAsync(produto);
      await _context.SaveChangesAsync();

      return produto;
    }

    public async Task<bool> UpdateAsync(Produto produto)
    {
      if (produto is null) throw new ArgumentNullException(nameof(produto));

      if(_context.Produtos.Any(p => p.ProdutoId == produto.ProdutoId))
      {
        _context.Produtos.Update(produto); //Recomendado a utilização do .Update quando a entidade já está sendo utilizada pelo contexto do EF
        await _context.SaveChangesAsync();

        return true;
      }

      return false;
    }

    public async Task<bool> DeleteAsync(int id)
    {
      var produto = await _context.Produtos.FindAsync(id);

      if(produto is not null)
      {
        _context.Produtos.Remove(produto);
        await _context.SaveChangesAsync();

        return true;
      }

      return false;
    }    
  }
}
