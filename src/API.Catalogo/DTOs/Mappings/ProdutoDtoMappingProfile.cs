using API.Catalogo.Models;
using AutoMapper;

namespace API.Catalogo.DTOs.Mappings
{
  public class ProdutoDtoMappingProfile : Profile
  {
    public ProdutoDtoMappingProfile()
    {
      CreateMap<Produto, ProdutoDto>().ReverseMap();
      CreateMap<Categoria, CategoriaDto>().ReverseMap();
    }
  }
}
