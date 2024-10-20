using API.Catalogo.Context;
using API.Catalogo.DTOs.Mappings;
using API.Catalogo.Extensions;
using API.Catalogo.Filters;
using API.Catalogo.Logging;
using API.Catalogo.Models;
using API.Catalogo.Repositories;
using API.Catalogo.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers(options =>
{
  options.Filters.Add(typeof(ApiExceptionFilter)); //Adiciona o filtro de exce��o criado como um filtro global
}).AddJsonOptions(options =>
{
  options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles; //Ignora refer�ncias ciclicas detectadas durante a serializa��o
}).AddNewtonsoftJson();

//var origensComAcessoPermitido = "_origensComAcessoPermitido";
builder.Services.AddCors(options => 
{
  ////Define uma pol�tica nomeada de CORS utilizando .AddPolicy
  //options.AddPolicy(name: origensComAcessoPermitido,
  //  policy =>
  //  {
  //    policy.WithOrigins("https://apirequest.io")
  //          .AllowAnyHeader()
  //          .AllowAnyMethod();
  //  });

  //Define uma pol�tica padr�o de CORS
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
  c.SwaggerDoc("v1", new OpenApiInfo { Title = "API Catalogo", Version = "v1" });
  c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
  {
    Name = "Authorization", //Nome do cabe�alho onde o token � enviado
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
});

//Configura��o do Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>() //ApplicationUser agora representa os usu�rios do Identity
  .AddEntityFrameworkStores<AppDbContext>()
  .AddDefaultTokenProviders();

string mySqlConnection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
{
  options.UseMySql(mySqlConnection, ServerVersion.AutoDetect(mySqlConnection));
});

var secretKey = builder.Configuration["Jwt:SecretKey"] ?? throw new ArgumentException("Invalid secret key");
builder.Services.AddAuthentication(options => //Configura a aplica��o para utilizar autentica��o com JwtBearer
{
  options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
  options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options => //Configura par�metros para o JwtBearer
{
  options.SaveToken = true;
  options.RequireHttpsMetadata = false; //N�o obriga o uso de HTTPS (em produ��o deveria ser true)
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
  //Cria pol�tica de autoriza��o com nome "AdminOnly" que pode ser aplicada nos controllers, nas actions ou em toda aplica��o
  options.AddPolicy("AdminOnly", policy => policy.RequireRole("admin")); //Para ter acesso a pol�tica AdminOnly o usu�rio deve ter a role admin
  options.AddPolicy("SuperAdminOnly", policy => policy.RequireRole("admin").RequireClaim("id", "tmansur"));
  options.AddPolicy("UserOnly", policy => policy.RequireRole("user"));
  options.AddPolicy("ExclusiveOnly", policy => policy.RequireAssertion(context => 
    (context.User.HasClaim(claim => claim.Type == "id" && claim.Value == "tmansur") || 
    context.User.IsInRole("super-admin"))));
});

builder.Services.AddRateLimiter(rateLimiterOptions =>
{
  rateLimiterOptions.AddFixedWindowLimiter("fixedWindow", options =>
  {
    options.PermitLimit = 1; //Quantidade de requisi��es permitidas durante a janela de tempo especificado
    options.Window = TimeSpan.FromSeconds(5); //Dura��o da janela de tempo    
    options.QueueLimit = 2; //N�mero m�ximo de requisi��es que podem ser enfileiradas 
    options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst; //Ordem de processamento das requisi��es que est�o na fila de espera
  });
  rateLimiterOptions.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

//Configurando limita��o global, com isso n�o � necess�rio marcar os controllers/endpoints com [EnableRateLimiting]
//builder.Services.AddRateLimiter(options =>
//{
//  options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext => 
//    RateLimitPartition.GetFixedWindowLimiter(
//      partitionKey: httpContext.User.Identity?.Name ?? httpContext.Request.Headers.Host.ToString(),
//      factory: partition => new FixedWindowRateLimiterOptions
//      {
//        AutoReplenishment = true,
//        PermitLimit = 2,
//        QueueLimit = 0,
//        Window = TimeSpan.FromSeconds(5)
//      }
//    ));
//  options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
//});

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
// Configura��o de fluxo de middlewares

if (app.Environment.IsDevelopment())
{
  app.UseSwagger(); //Middleware do swagger
  app.UseSwaggerUI(); //Middleware de interface do swagger
  app.ConfigureExceptionHandler();
}

app.UseHttpsRedirection();
app.UseRateLimiter();
//app.UseCors(origensComAcessoPermitido);
app.UseCors();
app.UseAuthorization(); //Middleware respons�vel pelo processo de autoriza��o

//Middle definido utilizando request delegate (fun��o que receve um objeto de contexto do request HTTP e executa uma l�gica para esse request)
//app.Use(async (context, next) =>
//{
//  //adicionar c�digo antes do request
//  await next(context);
//  //adicionar c�digo depois do request
//});

app.MapControllers(); //Middleware respons�vel pelo roteamento dos endpoints

app.Run(); // Middleware final do pipeline de processamento do request
