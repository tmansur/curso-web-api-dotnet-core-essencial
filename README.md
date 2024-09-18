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
