# Curso Web API ASP .NET Core Essencial (.NET 8)

Link: https://ambevtech.udemy.com/course/curso-web-api-asp-net-core-essencial

Plataforma: Udemy

Instrutor: Jose Carlos Macoratti

## Plugins VSCode

[C# Dev Kit](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csdevkit)

[IntelliCode for C# Dev Kit](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.vscodeintellicode-csharp)

---------------------------------------------------------------------

## Conceitos gerais

### API - Application Programming Interface
Interface para comunicação entre aplicativos.

### Web Services

### SOAP
Protocolo para comunicação entre aplicativos a partir de envio de documento XML.

### REST - Representational State Transfer
Estilo de arquitetura para desenvolvimento de serviços web utilizando o protocolo HTTP e seus métodos.

### RESTfull

### HTTP

### Hsts

### JSON

### Níveis de maturidade de Richardson

Modelo que classifica as APIs em 4 níveis de maturidade:

- Nível 0: POX - Plain Old XML. Utilização de URI única e não utilização dos verbos HTTP
- Nível 1: Recursos baseados em múltiplas URI mas não utiliza os verbos corretamente.
- Nível 2: Verbos HTTP e código de status.
- Nível 3: HATEOAS

### Injeção de dependência (DI)

DI é um **Padrão** utilizado para implementar o **princípio** de Inversão de Controle (IoC), reduzindo desse forma o acoplamento entre os objetos. Conceito diretamente relacionado ao **Princípio** da Inversão de Dependência (DIP).

~~~ csharp
//Cliente.cs
public class Cliente
{
  private readonly IPedido _pedido;

  public Client(IPedido pedido) //Container de injeção de dependência fica responsável por fornecer a instância de pedido
  {
    _pedido = pedido
  }

  public List<Pedido> ObterPedidos()
  {
    return pedido.GetPedidos();
  }
}

//program.cs
services.AddTransient<IPedido, Pedido>(); //Configura a injeção de dependência no container DI.
~~~

> [!NOTE]
> A interface `IServiceCollection` é utilizada para registrar os serviços no container de injeção de dependência nativo do .NET (que é definido pela biblioteca `Microsoft.Extensions.DependencyInjection`).
> Outros container disponíveis para .NET: Simple Injector, Autofac, Ninject, Spring.NET, Unity, Castle Windsor. 


#### Tempo de vida do serviço (Lifetimes Service)

1) Transient
Os serviços são instânciados cada vez que for solicitado, ou seja, se o construtor onde a classe é injetada for chamado X vezes, serão criadas X instância dessa classe.

2) Scoped
Os serviços são instânciados em cada solicitação/request. Se durante um request uma mesma classe estiver sendo injetada em diversos lugares, será instânciada apenas uma vez e esse objeto será utilizado em todas esses lugares. Se for realizado um segundo request, uma nova instância será criada.

3) Singleton
O serviço é instânciado apenas uma vez durante a vida útil do aplicativo.

### Classes Anêmicas

São classes que possuem apenas propriedades definidas, não possuem comportamentos.

~~~ Csharp
//Categoria.cs
public class Categoria
{
  public int CategoriaId { get; set; }
  public string? Nome { get; set; }
  public string? ImagemUrl { get; set; }
}
~~~

---------------------------------------------------------------------

## Web API

- .NET 8
- MySQL
- Entity Framework (Code First):
  - Microsoft.EntityFrameworkCore.Design
  - Microsoft.EntityFrameworkCOre.Tools (**instalar via linha de comando no diretório API.Contalogo**)
    - Instalação: `dotnet tool install --global dotnet-ef`
    - Atualização (caso esteja em versão antiga): `dotnet tool update --global dotnet-ef`
    - Verificando se a ferramenta está instalada: `dotnet ef`   
- Pomelo
  - Pomelo.EntityFrameworkCore.Mysql 
- Criar uma Web API para catálogo de produtos e categorias
- Caminho base dos endpoints: /v1/api/{recurso-substantivo}
- Padrão repositório
- Arquitetura em camadas: 
  - Presentation - Angular, Views, Mobile
  - Service - Classes de serviços 
  - Business Logic/Application Core - Repositórios, domínios, serviços
  - Data Access/Persistence - EF Core, MySQL
 
### Classe de contexto do EF Core

DbContext representa uma sessão com o banco de dados, sendo uma ponte entre as entidades de domínios (models) e o banco.

### Migrations

Permite atualizar de forma incremental o esquema de banco de dados de acordo com as atualizações das classes do modelo de domínio (models) definido no código.

Comandos do dotnet para criação/aplicação de Migrations utilizando o **dotnet ef** (necessário estar dentro do diretório onde está o projeto):
- Criar novo script: `dotnet ef migrations add "nome"`
- Remover um script já criado: `dotnet ef migrations remove "nome"`
- Aplicar alterações no bando de dados: `dotnet ef database update`

> [!NOTE]
> Para aplicar o Migrations via Package Manager Console do VS, é necessário instalar o pacote **Microsoft.EntityFrameworkCoreTools via Nuget** e utilizar os seguintes comandos:
> - add-migration "nome"
> - remove-migration "nome"
> - update-database

### Data Annotations 

Atributos que podem ser aplicados a classes e seus membros para fornecer metadados sobre como esses recursos devem ser tratados.

Namespaces:
- System.ComponentModel.DataAnnotations
- System.ComponentModel.DataAnnotations.Schema

Utilização:
- Validação de dados
  - Required
  - RegularExpression
  - StringLength
  - Range
  - CreditCard
  - Url
  - Phone
  - Compare
- Formatação e exibição de dados
- Geração de código
- Especificar relacionamento entre as entidades
- Sobrescrever as convenções do EF Core:
  - **Key** - identifica a propriedade como chave primária
  - **Table("nome")** - define o nome da tabela para a qual a classe será mapeada
  - **Column** - define a coluna na tabela para a qual a propriedade será mapeada
  - **DataType** - associa um tipo de dados a uma propriedade
  - **ForeignKey** - define a propriedade que será chave estrangeira
  - **NotMapped** - não faz mapeamento da propriedade
  - **StringLength** - define o tamanho mínimo e máximo para o tipo
  - **MaxLength**
  - **Required** - define o campo como obrigatório (NOT NULL) 

### Tratamento de erros

Para realizar o tratamento de erros de forma personalizada no ambiente de produção, pode-se utilizar o middleware **UseExceptionHandler**.

~~~CSharp
//program.cs
if(!app.Environment.IsDevelopment())
{
  app.UserExceptionHandle("/Error");
}
~~~

### Roteamento

Middlewares que podem ser utilizados para habilitar explicitamente o roteamento (não são necessário em um projeto WebAPI):
- UseRouting()
- USeEndpoints()

### Padrões de roteamento

Definindo rota padrão com atributo na classe controller: `[Route("api/[controller]")]` // rota: /api/produtos

Incrementando a rota padrão: `HttpGet("primeiro")` // rota: /api/produtos/primeiro

Incluindo parâmetros na rota padrão: 
- `HttpGet("{id}")` // rota: /api/produtos/{id}
- `HttpGet("{id}/{param2}")` // rota: /api/produtos/{id}/{param2]
- `HttpGet("{id}/{param2=Teste}")` // rota: /api/produtos/{id}/Teste

Ignorando a rota padrão: `HttpGet("/primeiro")` // rota: /primeiro

#### Restrições de rota

Restringir os valores aceito nos parâmetros da rota:

- `[HttpGet("{id:int:min(1)}")]` //Aceita int maior que 0
- `[HttpGet("{valor:alpha}")]` //Aceita valor alfanumérico
- `[HttpGet("{valor:alpha:length(5)}")]` //Aceita valor alfanumérico de tamanho 5

### Model Binding


#### Valores de formulário (corpo do request)

Valores passados no corpo de um request para endpoints de verbo PUT, PATCH e/ou POST

~~~CSharp
[HttpPut("{id}")]
public ActionResult Put(int id, [FromBody] Produto produto)
~~~

#### Rotas

api/produtos/**4**

~~~CSharp
[Httpget("{id}")]
public ActionResult<Produtos> Get(int id)
~~~

#### Query Strings

api/produtos/4?**nome=Suco&ativo=true**

~~~CSharp
[Httpget("{id}")]
public ActionResult<Produtos> Get(int id, string nome, bool ativo)
~~~

#### Atributos para definir Model Binding
- BindRequired
  - Obriga que o parâmetro seja passado para ocorrer o binding
  - Configuração feita na action do controller
  - Exemplo: `public ActionResult<Produtos> Get([BindingRequired]int id)`
- BindNever
  - Não vincular a informação ao parâmetro
  - Configuração feita na classe do modelo
  - Exemplo: [BindNever] public string ImagemUrl {get; set;}

#### Atributos que indicam a fonte de dados dos parâmetros
- FromForm
- FromRoute: recebe os dados passados na rota do endpoint.
- FromQuery: recebe os dados via querystring.
- FromHeader
- FromBody
- FromServices: recebe os dados via container de injeção de dependência de forma direta, sem a necessidade de usar a injeção de dependência no construtor.

### Validação Personalizada

#### 1-Criar atributos customizados

- Tem como objetivo validar uma propriedade.
- Pode ser utilizada em diversos modelos e propriedades.
- Criar uma classe que herda de `ValidationAttribute` e sobrescrever o método `IsValid` para realizar a validação customizada.
- O método `IsValid` retorna o campo estático `ValidationResul.Success` em caso de sucesso na validação ou uma nova instância da classe `ValidationResult` com a mensagem de erro.  

#### 2-Implementar IValidatableObject no modelo

- Pode acessar todas as propriedades do modelo e realizar uma validação mais complexa.
- Não pode ser reutilizada em outros modelos.

### Modelo de configuração

appsettings.json
appsettings.**Development**.json
appsettings.**Staging**.json
appsettings.**Production**.json

Utilizar variável de ambiente ASPNETCORE_ENVIRONMENT para definir qual arquivo de configuração utilizar.

Para ler os arquivos de configuração utilizamos o serviço disponível pela interface `IConfiguration`.

~~~CSharp
//xpto.cs
private readonly IConfiguration _configuration;

public Xpto(IConfiguration config)
{
  _configuration = config;
}

var valor1 = _configuration["chave"];
var valor2 = _configuration["seção:chave"];
~~~

Para ler os dados do arquivo de configuração a partir da classe `program.cs` utilizamos a instância de **builder** que possui uma propriedade **Configuration**:

~~~CSharp
//program.cs
var valor1 = builder.Configuration["chave1"];
var valor2 = builder.Configuration["secao1:chave2"];
var conexão = builder.Configuration.GetConnectionString("DefaultConnection"); //Retorna valor da chave "DefaultConnection" existente na sessão "ConnectionString"
~~~

### Middleware - Tratamento global de exceções

Utilizar o middleware `UseExceptionHandle`.

### Filtros

São atributos anexados às classes ou métodos dos controladores que injetam lógica extra ao processamento de uma requisição. Permitem executar código personalizado antes ou depois de executar uma action.

Tipos de filtro:
- Authorization - primeiro filtro a ser executado, determina se usuário tem autorização para executar o request.
- Resource - executado antes do **model binding** ocorrer.
- Action - executa código imediatamente antes e depois do método action do controller.
- Exception - utilizado para manipular exceções antes do corpo da response ser escrito.
- Result - executa código antes ou depois da execução dos resultados das actions do controller.

Tipos de implementação
- Síncrona:
  - IActionFilter
  - onActionExecuting e onActionExecuted
- Assíncrona:
  - IAsyncActionFilter
  - onActionExecutingAsync

Escopo de adição do filtro:
- Método action
- Classe do controller
- Globalmente (será aplicado a todos controladores e actions)

Ordem de execução dos filtros: global, classe, método.

### Logging

Microsoft.Extensions.Logging

Principais interfaces:
- ILoggingFactory
- ILoggingProvider
- ILogger

#### Log customizado
....

---------------------------------------------------------------------

## Padrão Repository

Lógica de negócio separada da lógica de acesso a dados (repository).

Duas abordagens:
- Repository específico: cria interface para uma entidade específica.
- Repository genérico: cria interface genérica contendo o contrato que define métodos genéricos.

---------------------------------------------------------------------

## Padrão Unit of Work

A injeção de cada repositório em cada controlador cria múltiplas instâncias do contexto DbContext, o que pode causar problema de concorrência. O padrão Unit of Work pode ser utilizado para resolver essa situação pois gerencia as transações e garante que todas as operações no banco de dados sejam consistentes (são executadas de forma atômica).

O padrão Unit of Work é um padrão de design de software que centraliza as transações e repositórios em um único ponto. Tem como objetivo garantir que as transações sejam uniformes entre todos os repositórios.

Unit of Work:
- Gerenciar as transações
- Ordenar o CRUD no banco de dados
- Impedir a concorrência
- Usar somente uma instância do contexto por requisição

Nesse padrão os repositórios são responsáveis por realizar operaões de leituras e escrita no banco de dados, mas não podem chamar o SaveChanges, que fica sobe a responsabilidade do padrão Unit of Work.

---------------------------------------------------------------------

## Transferência de objetos entre camadas

### DTO - Data Transfer Object

Não deve-se retornar entidades de domínio a partir dos endpoints da API.

DTO é um conteiner de dados utilizado para transportar os dados entre camadas de uma aplicação.

Podemos ter DTOs diferentes para entrada e saída de dados. Exemplo> CategoriaRequestDto e CategoriaResponseDto

> [!IMPORTANT]
> O conceito de DTO e ViewModel são diferentes em relação ao contexto em que são utilizados. DTO é utilizado na transferência de dados entre camadas, já o ViewModel é utilizado na camada de apresentação para estruturar dados que serão exibidos em uma pagina ou View (aplicações MVC).

> [!IMPORTANT]
> DTOs não devem conter lógica de negócio

O mapeamento entre DTO e entidade pode ser feito de forma manual ou utilizando uma lib como por exemplo a AutoMapper.  

### AutoMapper

Pacote:
- AutoMapper

Realiza o mapeamento entre objetos que representa entidades e objetos que representam dto de forma automática.

---------------------------------------------------------------------

## Atualização parcial de entidade com JSON Patch

HttpPatch - atualiza um recurso de forma parcial (body apenas com informações que serão atualizadas).

Utilizar padrão definido na RFC 6902 - (Json Patch)[https://jsonpatch.com/]:
- Define a estrutura do corpo do request HTTP para indicar as alterações que devem ser feitas no recurso.
- Operações possíveis: add, remove, **replace**, move copy e test.
- Propriedades das operações:
  - **op**: tipo de operação a ser realizada (lista anterior). A operação **replace** é utilizado para atualizar o recurso.
  - **patch**: caminho da propriedade do objeto que será atualizado. Ex: /estoque
  - **value**: novo valor da propriedade

JsonPatch na API Web do .NET: https://learn.microsoft.com/pt-br/aspnet/core/web-api/jsonpatch?view=aspnetcore-8.0

Pacotes necessários:
- Microsoft.AspNetCore.JsonPatch
- Microsoft.AspNetCore.Mvc.NewtonsoftJson
Utilizando JsonPatch no controlador:
- Atributo `HttpPatch`.
- Parâmetro `JsonPatchDocument<T>`.
- Aplicar alterações chamando o método `ApplyTo(Object)`.
- Exemplo:

~~~CSharp
//TesteController.cs

[HttpPatch]
public IActionResult PatchTeste(JsonPatchDocumento<TipoTeste> patchDoc)
{
  //...
  patchDoc.ApplyTo(TipoTeste, ModelState);
  //...
}
~~~



