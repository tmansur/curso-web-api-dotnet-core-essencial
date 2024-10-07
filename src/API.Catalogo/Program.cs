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
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers(options =>
{
  options.Filters.Add(typeof(ApiExceptionFilter)); //Adiciona o filtro de exce��o criado como um filtro global
}).AddJsonOptions(options =>
{
  options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles; //Ignora refer�ncias ciclicas detectadas durante a serializa��o
}).AddNewtonsoftJson();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
