using API.Catalogo.Controllers;
using API.Catalogo.DTOs;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace ApiCatalogoTests.UnitTests
{
  //IClassFixture permite a configuração da classe de teste GetProdutoTests como uma instância compartilhada da classe ProdutosControllerTest
  public class GetProdutoTests : IClassFixture<ProdutosControllerTest>
  {
    private readonly ProdutosController _controller;

    public GetProdutoTests(ProdutosControllerTest controller)
    {
      _controller = new ProdutosController(controller._repository, controller._mapper);
    }

    [Fact]
    public async Task GetProdutoById_Return_OkResult()
    {
      //Arrange
      var productId = 2;

      //Act
      var data = await _controller.GetAsync(productId);

      //Assert (xUnit)
      var result = Assert.IsType<OkObjectResult>(data.Result);
      Assert.Equal(200, result.StatusCode);

      //Assert (FluentAssertions)
      data.Result.Should().BeOfType<OkObjectResult>().
        Which.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task GetProdutoById_Return_NotFound()
    {
      //Arrange
      var productId = 999;

      //Act
      var data = await _controller.GetAsync(productId);

      //Assert
      data.Result.Should().BeOfType<NotFoundObjectResult>()
        .Which.StatusCode.Should().Be(404);
    }

    //[Fact]
    //public async Task GetProdutoById_Return_BadRequest()
    //{
    //  //Arrange
    //  var productId = -1;

    //  //Act
    //  var data = await _controller.GetAsync(productId);

    //  //Assert
    //  data.Result.Should().BeOfType<BadRequestObjectResult>()
    //    .Which.StatusCode.Should().Be(400);
    //}

    [Fact]
    public async Task GetProdutos_Return_ListOfProdutoDto()
    {
      //Act
      var data = await _controller.GetAsync();

      //Assert
      data.Result.Should().BeOfType<OkObjectResult>()
        .Which.Value.Should().BeAssignableTo<IEnumerable<ProdutoDto>>()
        .And.NotBeNull();
    }
  }
}
