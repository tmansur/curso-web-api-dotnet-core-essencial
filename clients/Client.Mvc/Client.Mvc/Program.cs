using Client.Mvc.Services;
using System.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient("CategoriasApi", configureClient =>
{
  configureClient.BaseAddress = new Uri(builder.Configuration["ServiceUri:CategoriasApi"]);
});
builder.Services.AddHttpClient("ProdutosApi", configureClient =>
{
  configureClient.BaseAddress = new Uri(builder.Configuration["ServiceUri:ProdutosApi"]);
});
//Como o endere�o base � o mesmo de CategoriasApi, n�o precisava incluir um novo HttpClient, isso est� sendo feito para demonstrar como deve ser configurado
//caso as APIs sejam diferentes
builder.Services.AddHttpClient("AutenticaApi", configureClient =>
{
  configureClient.BaseAddress = new Uri(builder.Configuration["ServiceUri:AutenticaApi"]);
  // O c�digo a seguir n�o � necess�rio, foi incluido apenas para mostrar como definir cabe�alho padr�o
  configureClient.DefaultRequestHeaders.Accept.Clear(); //Limpa todos cabe�alhos padr�es
  configureClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json")); //Inclui cabe�alho padr�o para o request
});

builder.Services.AddScoped<ICategoriaService, CategoriaService>();
builder.Services.AddScoped<IProdutoService, ProdutoService>();
builder.Services.AddScoped<IAutenticacaoService, AutenticacaoService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
  app.UseExceptionHandler("/Home/Error");
  // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
  app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
