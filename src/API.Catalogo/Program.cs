using API.Catalogo.Context;
using API.Catalogo.Extensions;
using API.Catalogo.Filters;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers().AddJsonOptions(options =>
{
  options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles; //Ignora referências ciclicas detectadas durante a serialização
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

string mySqlConnection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
{
  options.UseMySql(mySqlConnection, ServerVersion.AutoDetect(mySqlConnection));
});

builder.Services.AddScoped<ApiLoggingFilter>();

var app = builder.Build();

// Configure the HTTP request pipeline
// Configuração de fluxo de middlewares

if (app.Environment.IsDevelopment())
{
  app.UseSwagger(); //Middleware do swagger
  app.UseSwaggerUI(); //Middleware de interface do swagger
  app.ConfigureExceptionHandler();
}

app.UseHttpsRedirection();

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
