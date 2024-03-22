using Microsoft.AspNetCore.Mvc;
using Shelf.Life.API.Controllers;
using Shelf.Life.Domain.Models;
using Shelf.Life.Domain.Services;
using System.Net;
using System.Text.Json;

namespace Shelf.Life.API.Tests.Controllers;
public class FoodControllerTests
{
    private readonly AutoMocker _autoMocker = new();
    private readonly Fixture _fixture = new();

    private readonly FoodController _subject;
    public FoodControllerTests()
    {
        _subject = _autoMocker.CreateInstance<FoodController>();
    }

    [Fact]
    public async Task WhenGetAll_ThenReturnOkStatusWithAllFoods()
    {
        //Given
        var foods = _fixture.CreateMany<Food>();
        _autoMocker.GetMock<IFoodService>()
            .Setup(x => x.Get())
            .ReturnsAsync(foods);

        //When
        var result = (OkObjectResult)await _subject.GetAll();

        //Then
        result.StatusCode.Should().Be((int)HttpStatusCode.OK);
        result.Value.Should().BeEquivalentTo(foods);
    }

    [Fact]
    public async Task GivenFoodServiceFails_WhenGetAll_ThenReturnInternalServerError()
    {
        //Given
        var exception = _fixture.Create<Exception>();
        _autoMocker.GetMock<IFoodService>()
            .Setup(x => x.Get())
            .ThrowsAsync(exception);

        //When
        var result = (ObjectResult)await _subject.GetAll();

        //Then
        result.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        result.Value.Should().Be($"{nameof(FoodService)}:GET throws unexpected exception: {exception.Message}");
    }

    [Fact]
    public async Task GivenCreateFoodRequest_WhenCreate_ThenReturnNoContentStatus()
    {
        //Given
        var request = _fixture.Create<CreateFoodRequest>();

        //When
        var result = (NoContentResult)await _subject.Create(request);

        //Then
        result.StatusCode.Should().Be((int)HttpStatusCode.NoContent);

        _autoMocker.GetMock<IFoodService>()
            .Verify(x => x.Insert(request), Times.Once);
    }

    [Fact]
    public async Task GivenFoodAlreadyExist_WhenCreate_ThenReturnBadRequestStatus()
    {
        //Given
        var request = _fixture.Create<CreateFoodRequest>();

        var matchingFood = _fixture.Create<Food>();
        _autoMocker.GetMock<IFoodService>()
            .Setup(x => x.FindMatchingFood(request))
            .ReturnsAsync(matchingFood);

        //When
        var result = (BadRequestObjectResult)await _subject.Create(request);

        //Then
        result.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        result.Value.Should().Be($"{nameof(FoodService)}:POST failed. Food with same name already exist, duplicate food: {JsonSerializer.Serialize(matchingFood)}");

        _autoMocker.GetMock<IFoodService>()
            .Verify(x => x.Insert(request), Times.Never);
    }
}
