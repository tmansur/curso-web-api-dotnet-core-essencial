using API.Catalogo.Context;
using API.Catalogo.DTOs.Mappings;
using API.Catalogo.Extensions;
using API.Catalogo.Filters;
using API.Catalogo.Logging;
using API.Catalogo.Models;
using API.Catalogo.RateLimitOptions;
using API.Catalogo.Repositories;
using API.Catalogo.Services;
using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers(options =>
{
  options.Filters.Add(typeof(ApiExceptionFilter)); //Adiciona o filtro de exceção criado como um filtro global
}).AddJsonOptions(options =>
{
  options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles; //Ignora referências ciclicas detectadas durante a serialização
}).AddNewtonsoftJson();

//var origensComAcessoPermitido = "_origensComAcessoPermitido";
builder.Services.AddCors(options => 
{
  ////Define uma política nomeada de CORS utilizando .AddPolicy
  //options.AddPolicy(name: origensComAcessoPermitido,
  //  policy =>
  //  {
  //    policy.WithOrigins("https://apirequest.io")
  //          .AllowAnyHeader()
  //          .AllowAnyMethod();
  //  });

  //Define uma política padrão de CORS
  options.AddDefaultPolicy(policy =>
    {
      policy.WithOrigins("https://apirequest.io", "https://exemplo.com")
            .AllowAnyHeader()
            .WithMethods("GET");
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
  c.SwaggerDoc("v1", new OpenApiInfo { Title = "API Catalogo - V1", Version = "v1" });
  c.SwaggerDoc("v2", new OpenApiInfo { Title = "API Catalogo - V2", Version = "v2" });
  c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
  {
    Name = "Authorization", //Nome do cabeçalho onde o token é enviado
    Type = SecuritySchemeType.ApiKey,
    Scheme = "Bearer",
    BearerFormat = "JWT",
    In = ParameterLocation.Header,
    Description = "Bearer JWT"
  });
  c.AddSecurityRequirement(new OpenApiSecurityRequirement
  {
    {
      new OpenApiSecurityScheme
      {
        Reference = new OpenApiReference
        {
          Type = ReferenceType.SecurityScheme,
          Id = "Bearer"
        }
      },
      new string[] { }
    }
  });

  var xmlFileName = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
  c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFileName));
});

//Configuração do Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>() //ApplicationUser agora representa os usuários do Identity
  .AddEntityFrameworkStores<AppDbContext>()
  .AddDefaultTokenProviders();

string mySqlConnection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
{
  options.UseMySql(mySqlConnection, ServerVersion.AutoDetect(mySqlConnection));
});

var secretKey = builder.Configuration["Jwt:SecretKey"] ?? throw new ArgumentException("Invalid secret key");
builder.Services.AddAuthentication(options => //Configura a aplicação para utilizar autenticação com JwtBearer
{
  options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
  options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options => //Configura parâmetros para o JwtBearer
{
  options.SaveToken = true;
  options.RequireHttpsMetadata = false; //Não obriga o uso de HTTPS (em produção deveria ser true)
  options.TokenValidationParameters = new TokenValidationParameters()
  {
    ValidateIssuer = true,
    ValidateAudience = true,
    ValidateLifetime = true,
    ValidateIssuerSigningKey = true,
    ClockSkew = TimeSpan.Zero,
    ValidAudience = builder.Configuration["Jwt:ValidAudience"],
    ValidIssuer = builder.Configuration["Jwt:ValidIssuer"],
    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
  };
});

builder.Services.AddAuthorization(options =>
{
  //Cria política de autorização com nome "AdminOnly" que pode ser aplicada nos controllers, nas actions ou em toda aplicação
  options.AddPolicy("AdminOnly", policy => policy.RequireRole("admin")); //Para ter acesso a política AdminOnly o usuário deve ter a role admin
  options.AddPolicy("SuperAdminOnly", policy => policy.RequireRole("admin").RequireClaim("id", "tmansur"));
  options.AddPolicy("UserOnly", policy => policy.RequireRole("user"));
  options.AddPolicy("ExclusiveOnly", policy => policy.RequireAssertion(context => 
    (context.User.HasClaim(claim => claim.Type == "id" && claim.Value == "tmansur") || 
    context.User.IsInRole("super-admin"))));
});

var myRateLimitOptions = new MyRateLimitOptions();
builder.Configuration.GetSection(MyRateLimitOptions.MyRateLimit).Bind(myRateLimitOptions); //Associa os valores obtidos da seção MyRateLimit do arquivo appsettings.json com a instância da classe MyRateLimitOptions
builder.Services.AddRateLimiter(rateLimiterOptions =>
{
  rateLimiterOptions.AddFixedWindowLimiter("fixedWindow", options =>
  {
    options.PermitLimit = myRateLimitOptions.PermitLimit; //Quantidade de requisições permitidas durante a janela de tempo especificado
    options.Window = TimeSpan.FromSeconds(myRateLimitOptions.Window); //Duração da janela de tempo    
    options.QueueLimit = myRateLimitOptions.QueueLimit; //Número máximo de requisições que podem ser enfileiradas 
    options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst; //Ordem de processamento das requisições que estão na fila de espera
  });
  rateLimiterOptions.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

////Configurando limitação global, com isso não é necessário marcar os controllers/endpoints com [EnableRateLimiting]
//builder.Services.AddRateLimiter(options =>
//{
//  options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
//    RateLimitPartition.GetFixedWindowLimiter(
//      partitionKey: httpContext.User.Identity?.Name ?? httpContext.Request.Headers.Host.ToString(),
//      factory: partition => new FixedWindowRateLimiterOptions
//      {
//        AutoReplenishment = myRateLimitOptions.AutoReplenishment,
//        PermitLimit = myRateLimitOptions.PermitLimit,
//        QueueLimit = myRateLimitOptions.QueueLimit,
//        Window = TimeSpan.FromSeconds(myRateLimitOptions.Window)
//      }
//    ));
//  options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
//});

//Configura versionamento da API
builder.Services.AddApiVersioning(options =>
{
  options.DefaultApiVersion = new ApiVersion(1, 0); //Define a versão padrão da API
  options.AssumeDefaultVersionWhenUnspecified = true; //Usa a versão default da API, definda em DefaultApiVersion, se o cliente não fornecer uma versão explicitamente na requisição
  options.ReportApiVersions = true; //Permite informar via headers as versões de API suportadas. Atributos api-supported-versions e api-deprecated-version
  options.ApiVersionReader = ApiVersionReader.Combine(new UrlSegmentApiVersionReader()); //define o versionamento por URL (o valor padrão é QueryStringApiVersionReader)
}).AddApiExplorer(options => //Utilizado para configurar as rotas dos endpoints no Swagger para incluir o versionamento
{
  options.GroupNameFormat = "'v'VVV";
  options.SubstituteApiVersionInUrl = true;
});

builder.Services.AddScoped<ApiLoggingFilter>();
builder.Services.AddScoped<ICategoriaRepository, CategoriaRepository>();
builder.Services.AddScoped<IProdutoRepository, ProdutoRespository>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ITokenService, TokenService>();

//Configura o provedor de customizado
builder.Logging.AddProvider(new CustomerLoggerProvider(new CustomerLoggerProviderConfiguration
{
  LogLevel = LogLevel.Information
}));

builder.Services.AddAutoMapper(typeof(ProdutoDtoMappingProfile));

var app = builder.Build();

// Configure the HTTP request pipeline
// Configuração de fluxo de middlewares

if (app.Environment.IsDevelopment())
{
  app.UseSwagger(); //Middleware do swagger
  //Middleware de interface do swagger
  app.UseSwaggerUI(options =>
  {
    // Configurar os endpoints de cada versão
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
    options.SwaggerEndpoint("/swagger/v2/swagger.json", "API V2");

    //// Definir o Swagger UI para carregar a última versão como padrão (opcional)
    //options.DefaultModelExpandDepth(2);
    //options.DefaultModelRendering(Swashbuckle.AspNetCore.SwaggerUI.ModelRendering.Model);
    //options.DisplayRequestDuration();
    //options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None); // Configura como as docs aparecem
  }); 
  app.ConfigureExceptionHandler();
}

app.UseHttpsRedirection();
app.UseRateLimiter();
//app.UseCors(origensComAcessoPermitido);
app.UseCors();
app.UseAuthorization(); //Middleware responsável pelo processo de autorização

//Middle definido utilizando request delegate (função que receve um objeto de contexto do request HTTP e executa uma lógica para esse request)
//app.Use(async (context, next) =>
//{
//  //adicionar código antes do request
//  await next(context);
//  //adicionar código depois do request
//});

app.MapControllers(); //Middleware responsável pelo roteamento dos endpoints

app.Run(); // Middleware final do pipeline de processamento do request
