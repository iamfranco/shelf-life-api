using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shelf.Life.API.Controllers;
using Shelf.Life.API.Validators;
using Shelf.Life.API.Validators.Models;
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
    public async Task GivenCreateFoodRequest_WhenCreate_ThenReturnNoContentStatus()
    {
        //Given
        var request = _fixture.Create<CreateOrUpdateFoodRequest>();

        //When
        var result = (NoContentResult)await _subject.Create(request);

        //Then
        result.StatusCode.Should().Be(StatusCodes.Status204NoContent);

        _autoMocker.GetMock<IFoodStore>()
            .Verify(x => x.Insert(request), Times.Once);
    }

    [Fact]
    public async Task GivenValidatorThrowsBadRequestException_WhenCreate_ThenExceptionThrown()
    {
        //Given
        var request = _fixture.Create<CreateOrUpdateFoodRequest>();

        var exception = _fixture.Create<BadRequestException>();
        _autoMocker.GetMock<IFoodValidator>()
            .Setup(x => x.ThrowIfFoodExists(request.Name))
            .Throws(exception);

        //When Then
        var act = () => _subject.Create(request);
        await act.Should().ThrowAsync<BadRequestException>()
            .WithMessage(exception.Message);

        _autoMocker.GetMock<IFoodStore>()
            .Verify(x => x.Insert(request), Times.Never);
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
    public void GivenNoMatchingFoodExists_WhenGet_ThenThrowNotFoundException()
    {
        //Given
        var id = _fixture.Create<int>();

        //When Then
        var act = () => _subject.Get(id);
        act.Should().Throw<NotFoundException>()
            .WithMessage($"{nameof(Food)} with id [{id}] does NOT exist.");
    }

    [Fact]
    public async Task WhenUpdate_ThenReturnsNoContentStatusAndUpdateFood()
    {
        //Given
        var id = _fixture.Create<int>();
        var request = _fixture.Create<CreateOrUpdateFoodRequest>();

        //When
        var result = (NoContentResult)await _subject.Update(id, request);

        //Then
        result.StatusCode.Should().Be(StatusCodes.Status204NoContent);

        _autoMocker.GetMock<IFoodStore>()
            .Verify(x => x.Update(id, request), Times.Once);
    }

    [Fact]
    public async Task GivenValidatorThrowsNotFoundException_WhenUpdate_ThenThrowNotFoundException()
    {
        //Given
        var id = _fixture.Create<int>();
        var request = _fixture.Create<CreateOrUpdateFoodRequest>();

        var exception = _fixture.Create<NotFoundException>();
        _autoMocker.GetMock<IFoodValidator>()
            .Setup(x => x.ThrowIfFoodDoesNotExist(id))
            .Throws(exception);

        //When Then
        var act = () => _subject.Update(id, request);
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage(exception.Message);

        _autoMocker.GetMock<IFoodStore>()
            .Verify(x => x.Update(id, request), Times.Never);
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

    [Fact]
    public async Task GivenValidatorThrowsNotFoundException_WhenDelete_ThenThrowNotFoundException()
    {
        //Given
        var id = _fixture.Create<int>();

        var exception = _fixture.Create<NotFoundException>();
        _autoMocker.GetMock<IFoodValidator>()
            .Setup(x => x.ThrowIfFoodDoesNotExist(id))
            .Throws(exception);

        //When Then
        var act = () => _subject.Delete(id);
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage(exception.Message);

        _autoMocker.GetMock<IFoodStore>()
            .Verify(x => x.Delete(id), Times.Never);
    }
}
