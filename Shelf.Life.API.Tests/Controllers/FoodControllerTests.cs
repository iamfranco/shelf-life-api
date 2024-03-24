using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shelf.Life.API.Controllers;
using Shelf.Life.Domain.Models;
using Shelf.Life.Domain.Models.Requests;
using Shelf.Life.Domain.Stores;

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
    public void WhenGetAll_ThenReturnOkStatusWithAllFoods()
    {
        //Given
        var foods = _fixture.CreateMany<Food>();
        _autoMocker.GetMock<IFoodStore>()
            .Setup(x => x.Get())
            .Returns(foods);

        //When
        var result = (OkObjectResult)_subject.GetAll();

        //Then
        result.StatusCode.Should().Be(StatusCodes.Status200OK);
        result.Value.Should().BeEquivalentTo(foods);
    }

    [Fact]
    public async Task GivenCreateFoodRequest_WhenCreate_ThenReturnOkStatus()
    {
        //Given
        var request = _fixture.Create<CreateOrUpdateFoodRequest>();

        var createdFood = _fixture.Create<Food>();
        _autoMocker.GetMock<IFoodStore>()
            .Setup(x => x.Insert(request))
            .ReturnsAsync(createdFood);

        //When
        var result = (OkObjectResult)await _subject.Create(request);

        //Then
        result.StatusCode.Should().Be(StatusCodes.Status200OK);
        result.Value.Should().Be(createdFood);
    }

    [Fact]
    public void GivenMatchingFoodsExist_WhenGetByPartialName_ThenReturnOkStatusWithMatchingFoods()
    {
        //Given
        var partialName = _fixture.Create<string>();

        var matchingFoods = _fixture.CreateMany<Food>();
        _autoMocker.GetMock<IFoodStore>()
            .Setup(x => x.QueryByPartialName(partialName))
            .Returns(matchingFoods);

        //When
        var result = (OkObjectResult)_subject.GetByPartialName(partialName);

        //Then
        result.StatusCode.Should().Be(StatusCodes.Status200OK);
        result.Value.Should().BeEquivalentTo(matchingFoods);
    }

    [Fact]
    public void GivenMatchingFoodExists_WhenGet_ThenReturnsOkStatusAndMatchingFood()
    {
        //Given
        var id = _fixture.Create<int>();
        var matchingFood = _fixture.Create<Food>();
        _autoMocker.GetMock<IFoodStore>()
            .Setup(x => x.FindById(id))
            .Returns(matchingFood);

        //When
        var result = (OkObjectResult)_subject.Get(id);

        //Then
        result.StatusCode.Should().Be(StatusCodes.Status200OK);
        result.Value.Should().Be(matchingFood);
    }

    [Fact]
    public async Task WhenUpdate_ThenReturnsOkStatusAndUpdatedFood()
    {
        //Given
        var id = _fixture.Create<int>();
        var request = _fixture.Create<CreateOrUpdateFoodRequest>();

        var updatedFood = _fixture.Create<Food>();
        _autoMocker.GetMock<IFoodStore>()
            .Setup(x => x.Update(id, request))
            .ReturnsAsync(updatedFood);

        //When
        var result = (OkObjectResult)await _subject.Update(id, request);

        //Then
        result.StatusCode.Should().Be(StatusCodes.Status200OK);
        result.Value.Should().Be(updatedFood);
    }

    [Fact]
    public async Task WhenDelete_ThenReturnsNoContentAndDeletesFood()
    {
        //Given
        var id = _fixture.Create<int>();

        //When
        var result = (NoContentResult)await _subject.Delete(id);

        //Then
        result.StatusCode.Should().Be(StatusCodes.Status204NoContent);

        _autoMocker.GetMock<IFoodStore>()
            .Verify(x => x.Delete(id), Times.Once);
    }
}
