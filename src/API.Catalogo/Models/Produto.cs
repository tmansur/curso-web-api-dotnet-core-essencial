using API.Catalogo.Validations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace API.Catalogo.Models
{
  public class Produto
  {
    [Key]
    public int ProdutoId { get; set; }

    [Required]
    [StringLength(80, ErrorMessage="O nome deve ter no máximo {1} caracteres")]
    [PrimeiraLetraMaiuscula] //Atributo customizado
    public string? Nome { get; set; }

    [Required]
    [StringLength(300, ErrorMessage="A descrição deve ter no máximo {1} caracteres")]
    public string? Descricao { get; set; }

    [Required]
    [Column(TypeName="decimal(10,2)")]
    [Range(1, 10000, ErrorMessage="O preço deve estar entre {1} e {2}")]
    public decimal Preco { get; set; }

    [Required]
    [StringLength(3000)]
    public string? ImagemUrl { get; set; }
    
    public float Estoque { get; set; }
    
    public DateTime DataCadastro { get; set; }

    //Relacionamento entre Produtos e Categorias (um produto pode ter apenas uma categoria relacionada)
    public int CategoriaId {  get; set; }

    [JsonIgnore] //Ignora a propriedade na serialização e na desserialização
    public Categoria? Categoria { get; set; }

    // Por padrão, todas propriedades definidas como públicas são serializadas, o que pode gerar informações desnecessárias no request/response
    // Para evitar isso pode-se ingnorar propriedades:
    // 1) Ignorando propriedades individuais: [JsonIgnore] ou [JsonIgnore(Condition=JsonIgnoreCondition.Always)] 
    // 2) Ignorando todas as propriedades: Incluir na configuração do JSON na classe program .DefaultIgnoreCondition = JsonIgnoreCondition.<condição>)
  }
}
