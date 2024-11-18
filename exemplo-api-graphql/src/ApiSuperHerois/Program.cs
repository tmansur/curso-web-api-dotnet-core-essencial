using ApiSuperHerois.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("Sqlite");
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlite(connectionString));

//Configura o GraphQL
builder.Services.AddGraphQLServer().AddQueryType<Query>()
                                   .AddProjections() //Habilita projeções, o que permite definir campos específicos no resultado da consulta
                                   .AddFiltering() //Habilita filtro nas consultas
                                   .AddSorting(); //Habilita ordenação nas consultas

var app = builder.Build();

CreateDatabase(app);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapGraphQL("/graphql"); //Mapea o endpoint /graphql

app.Run();

static void CreateDatabase(WebApplication app)
{
    var serviceScope = app.Services.CreateScope();
    var dataContext = serviceScope.ServiceProvider.GetService<AppDbContext>();
    dataContext?.Database.EnsureCreated(); //Cria o banco de dados quando a aplicação é executada
}