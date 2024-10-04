using API.Catalogo.Models;
using AutoMapper;

namespace API.Catalogo.DTOs.Mappings
{
  public class ProdutoDtoMappingProfile : Profile
  {
    public ProdutoDtoMappingProfile()
    {
      CreateMap<Produto, ProdutoDto>().ReverseMap();
      CreateMap<Produto, ProdutoUpdateRequestDto>().ReverseMap();
      CreateMap<Produto, ProdutoUpdateResponseDto>().ReverseMap();
    }
  }
}
