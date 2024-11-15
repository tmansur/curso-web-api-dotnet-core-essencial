using API.Catalogo.Models;

namespace API.Catalogo.DTOs.Mappings
{
  public static class CategoriaDtoMappingExtensions
  {
    public static Categoria? ConvertToEntity(this CategoriaDto categoriaDto)
    {
      if(categoriaDto == null) return null;

      return new Categoria()
      {
        CategoriaId = categoriaDto.CategoriaId,
        Nome = categoriaDto.Nome,
        ImagemUrl = categoriaDto.ImagemUrl
      };
    }

    public static CategoriaDto? ConvertoToDto(this Categoria categoria)
    {
      if (categoria is null) return null;

      return new CategoriaDto()
      {
        CategoriaId = categoria.CategoriaId,
        Nome = categoria.Nome,
        ImagemUrl = categoria.ImagemUrl
      };
    }

    public static IEnumerable<CategoriaDto> ConvertToDtoList(this IEnumerable<Categoria> categorias)
    {
      if (categorias is null || !categorias.Any()) return new List<CategoriaDto>();

      return categorias.Select(ConvertoToDto).ToList();
    }
  }
}
