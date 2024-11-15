using AutoMapper;
using Catalogo.Application.Dtos;
using Catalogo.Domain.Entities;

namespace Catalogo.Application.Mappings
{
  public class DomainToDtoMappingProfile : Profile
  {
    public DomainToDtoMappingProfile()
    {
      CreateMap<Categoria, CategoriaDTO>().ReverseMap();
      CreateMap<Produto, ProdutoDTO>().ReverseMap();
    }
  }
}
