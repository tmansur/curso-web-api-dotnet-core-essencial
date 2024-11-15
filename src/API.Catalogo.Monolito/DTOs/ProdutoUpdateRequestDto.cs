using System.ComponentModel.DataAnnotations;

namespace API.Catalogo.DTOs
{
  public class ProdutoUpdateRequestDto : IValidatableObject
  {
    [Range(1,9999, ErrorMessage="O estoque deve estar entre 1 e 9999")]
    public float Estoque { get; set; }
    public DateTime DataCadastro { get; set; }

    //Validação customizada para o atributo DataCadastro
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) 
    {
      if(DataCadastro.Date <= DateTime.UtcNow.Date)
      {
        yield return new ValidationResult(
          "A data deve ser maior que a data atual",
          new[] { nameof(DataCadastro) });
      }
    }
  }
}
