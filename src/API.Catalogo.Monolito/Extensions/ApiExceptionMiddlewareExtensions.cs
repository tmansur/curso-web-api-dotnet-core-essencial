using API.Catalogo.Models;
using Microsoft.AspNetCore.Diagnostics;
using System.Net;

namespace API.Catalogo.Extensions
{
  public static class ApiExceptionMiddlewareExtensions
  {
    // Método de extensão criado para tratamento global de exceção
    public static void ConfigureExceptionHandler(this IApplicationBuilder app)
    {
      //Configura middleware de tratamento de exceção
      app.UseExceptionHandler(appError => 
      {
        //O que será executado quando uma exceção não tratada for recebida
        appError.Run(async context =>
        {
          context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
          context.Response.ContentType = "application/json";

          var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
          if(contextFeature != null)
          {
            await context.Response.WriteAsync(new ErrorDetails()
            {
              StatusCode = context.Response.StatusCode,
              Message = contextFeature.Error.Message,
              Trace = contextFeature.Error.StackTrace
            }.ToString());
          }
        });
      });
    }
  }
}
