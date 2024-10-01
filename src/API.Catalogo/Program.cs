using API.Catalogo.Context;
using API.Catalogo.Extensions;
using API.Catalogo.Filters;
using API.Catalogo.Logging;
using API.Catalogo.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers(options =>
{
  options.Filters.Add(typeof(ApiExceptionFilter)); //Adiciona o filtro de exce��o criado como um filtro global
})
.AddJsonOptions(options =>
{
  options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles; //Ignora refer�ncias ciclicas detectadas durante a serializa��o
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

string mySqlConnection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
{
  options.UseMySql(mySqlConnection, ServerVersion.AutoDetect(mySqlConnection));
});

builder.Services.AddScoped<ApiLoggingFilter>();
builder.Services.AddScoped<ICategoriaRepository, CategoriaRepository>();
builder.Services.AddScoped<IProdutoRepository, ProdutoRespository>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

//Configura o provedor de customizado
builder.Logging.AddProvider(new CustomerLoggerProvider(new CustomerLoggerProviderConfiguration
{
  LogLevel = LogLevel.Information
}));

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
