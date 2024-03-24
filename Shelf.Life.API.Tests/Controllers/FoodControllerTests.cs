using Microsoft.AspNetCore.Mvc;
using Shelf.Life.API.Controllers;
using Shelf.Life.Domain.Models;
using Shelf.Life.Domain.Models.Requests;
using Shelf.Life.Domain.Stores;
using System;
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
        result.StatusCode.Should().Be((int)HttpStatusCode.OK);
        result.Value.Should().BeEquivalentTo(foods);
    }

    [Fact]
    public void GivenExceptionThrown_WhenGetAll_ThenReturnInternalServerError()
    {
        //Given
        var exception = _fixture.Create<Exception>();
        _autoMocker.GetMock<IFoodStore>()
            .Setup(x => x.Get())
            .Throws(exception);

        //When
        var result = (ObjectResult)_subject.GetAll();

        //Then
        result.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        result.Value.Should().Be($"{nameof(FoodController)}:{nameof(_subject.GetAll)} throws unexpected exception: {exception.Message}");
    }

    [Fact]
    public async Task GivenCreateFoodRequest_WhenCreate_ThenReturnNoContentStatus()
    {
        //Given
        var request = _fixture.Create<CreateOrUpdateFoodRequest>();

        //When
        var result = (NoContentResult)await _subject.Create(request);

        //Then
        result.StatusCode.Should().Be((int)HttpStatusCode.NoContent);

        _autoMocker.GetMock<IFoodStore>()
            .Verify(x => x.Insert(request), Times.Once);
    }

    [Fact]
    public async Task GivenFoodAlreadyExist_WhenCreate_ThenReturnBadRequestStatus()
    {
        //Given
        var request = _fixture.Create<CreateOrUpdateFoodRequest>();

        var matchingFood = _fixture.Create<Food>();
        _autoMocker.GetMock<IFoodStore>()
            .Setup(x => x.FindByName(request.Name))
            .Returns(matchingFood);

        //When
        var result = (BadRequestObjectResult)await _subject.Create(request);

        //Then
        result.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        result.Value.Should().Be($"{nameof(FoodController)}:{nameof(_subject.Create)} failed. Food with same name already exist, duplicate food: {JsonSerializer.Serialize(matchingFood)}");

        _autoMocker.GetMock<IFoodStore>()
            .Verify(x => x.Insert(request), Times.Never);
    }

    [Fact]
    public async Task GivenExceptionThrownDuringFindByName_WhenCreate_ThenReturnInternalServerError()
    {
        //Given
        var request = _fixture.Create<CreateOrUpdateFoodRequest>();

        var exception = _fixture.Create<Exception>();
        _autoMocker.GetMock<IFoodStore>()
            .Setup(x => x.FindByName(request.Name))
            .Throws(exception);

        //When
        var result = (ObjectResult)await _subject.Create(request);

        //Then
        result.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        result.Value.Should().Be($"{nameof(FoodController)}:{nameof(_subject.Create)} throws unexpected exception: {exception.Message}");
    }

    [Fact]
    public async Task GivenExceptionThrownDuringInsert_WhenCreate_ThenReturnInternalServerError()
    {
        //Given
        var request = _fixture.Create<CreateOrUpdateFoodRequest>();

        var exception = _fixture.Create<Exception>();
        _autoMocker.GetMock<IFoodStore>()
            .Setup(x => x.Insert(request))
            .ThrowsAsync(exception);

        //When
        var result = (ObjectResult)await _subject.Create(request);

        //Then
        result.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        result.Value.Should().Be($"{nameof(FoodController)}:{nameof(_subject.Create)} throws unexpected exception: {exception.Message}");
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
        result.StatusCode.Should().Be((int)HttpStatusCode.OK);
        result.Value.Should().BeEquivalentTo(matchingFoods);
    }

    [Fact]
    public void GivenExceptionThrown_WhenGetByPartialName_ThenReturnInternalServerError()
    {
        //Given
        var partialName = _fixture.Create<string>();

        var exception = _fixture.Create<Exception>();
        _autoMocker.GetMock<IFoodStore>()
            .Setup(x => x.QueryByPartialName(partialName))
            .Throws(exception);

        //When
        var result = (ObjectResult)_subject.GetByPartialName(partialName);

        //Then
        result.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        result.Value.Should().Be($"{nameof(FoodController)}:{nameof(_subject.GetByPartialName)} throws unexpected exception: {exception.Message}");
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
        result.StatusCode.Should().Be((int)HttpStatusCode.OK);
        result.Value.Should().Be(matchingFood);
    }

    [Fact]
    public void GivenNoMatchingFoodExists_WhenGet_ThenReturnsNotFoundStatus()
    {
        //Given
        var id = _fixture.Create<int>();

        //When
        var result = (NotFoundResult)_subject.Get(id);

        //Then
        result.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
    }

    [Fact]
    public void GivenExceptionThrown_WhenGet_ThenReturnsInternalServerError()
    {
        //Given
        var id = _fixture.Create<int>();
        var exception = _fixture.Create<Exception>();
        _autoMocker.GetMock<IFoodStore>()
            .Setup(x => x.FindById(id))
            .Throws(exception);

        //When
        var result = (ObjectResult)_subject.Get(id);

        //Then
        result.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        result.Value.Should().Be($"{nameof(FoodController)}:{nameof(_subject.Get)} throws unexpected exception: {exception.Message}");
    }

    [Fact]
    public async Task GivenMatchingFoodExists_WhenUpdate_ThenReturnsNoContentStatusAndUpdateFood()
    {
        //Given
        var id = _fixture.Create<int>();
        var request = _fixture.Create<CreateOrUpdateFoodRequest>();

        var matchingFood = _fixture.Create<Food>();
        _autoMocker.GetMock<IFoodStore>()
            .Setup(x => x.FindById(id))
            .Returns(matchingFood);

        //When
        var result = (NoContentResult)await _subject.Update(id, request);

        //Then
        result.StatusCode.Should().Be((int)HttpStatusCode.NoContent);

        _autoMocker.GetMock<IFoodStore>()
            .Verify(x => x.Update(id, request), Times.Once);
    }

    [Fact]
    public async Task GivenNoMatchingFoodExists_WhenUpdate_ThenReturnsNotFoundStatus()
    {
        //Given
        var id = _fixture.Create<int>();
        var request = _fixture.Create<CreateOrUpdateFoodRequest>();

        //When
        var result = (NotFoundObjectResult)await _subject.Update(id, request);

        //Then
        result.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        result.Value.Should().Be($"{nameof(FoodController)}:{nameof(_subject.Update)} failed. Food with id {id} not found.");
    }

    [Fact]
    public async Task GivenExceptionThrownDuringFindById_WhenUpdate_ThenReturnsInternalServerError()
    {
        //Given
        var id = _fixture.Create<int>();
        var request = _fixture.Create<CreateOrUpdateFoodRequest>();

        var exception = _fixture.Create<Exception>();
        _autoMocker.GetMock<IFoodStore>()
            .Setup(x => x.FindById(id))
            .Throws(exception);

        //When
        var result = (ObjectResult)await _subject.Update(id, request);

        //Then
        result.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        result.Value.Should().Be($"{nameof(FoodController)}:{nameof(_subject.Update)} throws unexpected exception: {exception.Message}");
    }

    [Fact]
    public async Task GivenExceptionThrownDuringUpdate_WhenUpdate_ThenReturnsInternalServerError()
    {
        //Given
        var id = _fixture.Create<int>();
        var request = _fixture.Create<CreateOrUpdateFoodRequest>();

        var matchingFood = _fixture.Create<Food>();
        _autoMocker.GetMock<IFoodStore>()
            .Setup(x => x.FindById(id))
            .Returns(matchingFood);

        var exception = _fixture.Create<Exception>();
        _autoMocker.GetMock<IFoodStore>()
            .Setup(x => x.Update(id, request))
            .Throws(exception);

        //When
        var result = (ObjectResult)await _subject.Update(id, request);

        //Then
        result.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        result.Value.Should().Be($"{nameof(FoodController)}:{nameof(_subject.Update)} throws unexpected exception: {exception.Message}");
    }

    [Fact]
    public async Task GivenMatchingFoodExists_WhenDelete_ThenReturnsNoContentAndDeletesFood()
    {
        //Given
        var id = _fixture.Create<int>();

        var matchingFood = _fixture.Create<Food>();
        _autoMocker.GetMock<IFoodStore>()
            .Setup(x => x.FindById(id))
            .Returns(matchingFood);

        //When
        var result = (NoContentResult)await _subject.Delete(id);

        //Then
        result.StatusCode.Should().Be((int)HttpStatusCode.NoContent);

        _autoMocker.GetMock<IFoodStore>()
            .Verify(x => x.Delete(id), Times.Once);
    }

    [Fact]
    public async Task GivenNoMatchingFoodExists_WhenDelete_ThenReturnsNotFoundStatus()
    {
        //Given
        var id = _fixture.Create<int>();

        //When
        var result = (NotFoundObjectResult)await _subject.Delete(id);

        //Then
        result.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        result.Value.Should().Be($"{nameof(FoodController)}:{nameof(_subject.Delete)} failed. Food with id {id} not found.");
        _autoMocker.GetMock<IFoodStore>()
            .Verify(x => x.Delete(id), Times.Never);
    }

    [Fact]
    public async Task GivenExceptionThrownDuringFindById_WhenDelete_ThenReturnsInternalServerError()
    {
        //Given
        var id = _fixture.Create<int>();

        var exception = _fixture.Create<Exception>();
        _autoMocker.GetMock<IFoodStore>()
            .Setup(x => x.FindById(id))
            .Throws(exception);

        //When
        var result = (ObjectResult)await _subject.Delete(id);

        //Then
        result.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        result.Value.Should().Be($"{nameof(FoodController)}:{nameof(_subject.Delete)} throws unexpected exception: {exception.Message}");
        _autoMocker.GetMock<IFoodStore>()
            .Verify(x => x.Delete(id), Times.Never);
    }

    [Fact]
    public async Task GivenExceptionThrownDuringDelete_WhenDelete_ThenReturnsInternalServerError()
    {
        //Given
        var id = _fixture.Create<int>();

        var matchingFood = _fixture.Create<Food>();
        _autoMocker.GetMock<IFoodStore>()
            .Setup(x => x.FindById(id))
            .Returns(matchingFood);

        var exception = _fixture.Create<Exception>();
        _autoMocker.GetMock<IFoodStore>()
            .Setup(x => x.Delete(id))
            .ThrowsAsync(exception);

        //When
        var result = (ObjectResult)await _subject.Delete(id);

        //Then
        result.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        result.Value.Should().Be($"{nameof(FoodController)}:{nameof(_subject.Delete)} throws unexpected exception: {exception.Message}");
    }
}
