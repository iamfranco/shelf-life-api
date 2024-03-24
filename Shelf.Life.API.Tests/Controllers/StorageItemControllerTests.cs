using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shelf.Life.API.Controllers;
using Shelf.Life.API.Validators;
using Shelf.Life.API.Validators.Models;
using Shelf.Life.Domain.Models;
using Shelf.Life.Domain.Models.Requests;
using Shelf.Life.Domain.Stores;

namespace Shelf.Life.API.Tests.Controllers;
public class StorageItemControllerTests
{
    private readonly Fixture _fixture = new();
    private readonly AutoMocker _autoMocker = new();

    private readonly StorageItemController _subject;

    public StorageItemControllerTests()
    {
        _subject = _autoMocker.CreateInstance<StorageItemController>();
    }

    [Fact]
    public void WhenGetAll_ThenReturnsOkStatusAndAllStorageItems()
    {
        //Given
        var storageItems = _fixture.CreateMany<StorageItem>();
        _autoMocker.GetMock<IStorageItemStore>()
            .Setup(x => x.Get())
            .Returns(storageItems);

        //When
        var result = (OkObjectResult)_subject.GetAll();

        //Then
        result.StatusCode.Should().Be(StatusCodes.Status200OK);
        result.Value.Should().Be(storageItems);
    }

    [Fact]
    public async Task GivenRequest_WhenCreate_ThenReturnNoContentStatus()
    {
        //Given
        var request = _fixture.Create<CreateOrUpdateStorageItemRequest>();

        //When
        var result = (NoContentResult)await _subject.Create(request);

        //Then
        result.StatusCode.Should().Be(StatusCodes.Status204NoContent);

        _autoMocker.GetMock<IStorageItemStore>()
            .Verify(x => x.Insert(request), Times.Once);
    }

    [Fact]
    public void GivenId_WhenGet_ThenReturnsMatchingStorageItem()
    {
        //Given
        var id = _fixture.Create<int>();

        var matchingFood = _fixture.Create<StorageItem>();
        _autoMocker.GetMock<IStorageItemStore>()
            .Setup(x => x.FindById(id))
            .Returns(matchingFood);

        //When
        var result = (OkObjectResult)_subject.Get(id);

        //Then
        result.StatusCode.Should().Be(StatusCodes.Status200OK);
        result.Value.Should().Be(matchingFood);
    }

    [Fact]
    public void GivenNoMatchingStorageItemExist_WhenGet_ThenThrowsNotFoundException()
    {
        //Given
        var id = _fixture.Create<int>();

        //When Then
        var act = () => _subject.Get(id);
        act.Should().Throw<NotFoundException>()
            .WithMessage($"{nameof(StorageItem)} with id [{id}] does NOT exist.");
    }

    [Fact]
    public async Task GivenIdAndRequest_WhenUpdate_ThenReturnsNoContentAndUpdatesStorageItem()
    {
        //Given
        var id = _fixture.Create<int>();
        var request = _fixture.Create<CreateOrUpdateStorageItemRequest>();

        //When
        var result = (NoContentResult)await _subject.Update(id, request);

        //Then
        result.StatusCode.Should().Be(StatusCodes.Status204NoContent);

        _autoMocker.GetMock<IStorageItemStore>()
            .Verify(x => x.Update(id, request), Times.Once);
    }

    [Fact]
    public async Task GivenValidatorThrowsNotFoundException_WhenUpdate_ThenThrowsNotFoundException()
    {
        //Given
        var id = _fixture.Create<int>();
        var request = _fixture.Create<CreateOrUpdateStorageItemRequest>();

        var exception = _fixture.Create<NotFoundException>();
        _autoMocker.GetMock<IStorageItemValidator>()
            .Setup(x => x.ThrowIfStorageItemDoesNotExist(id))
            .Throws(exception);

        //When Then
        var act = () => _subject.Update(id, request);
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage(exception.Message);

        _autoMocker.GetMock<IStorageItemStore>()
            .Verify(x => x.Update(id, request), Times.Never);
    }

    [Fact]
    public async Task GivenId_WhenDelete_ThenReturnsNoContentAndDeletesStorageItem()
    {
        //Given
        var id = _fixture.Create<int>();

        //When
        var result = (NoContentResult)await _subject.Delete(id);

        //Then
        result.StatusCode.Should().Be(StatusCodes.Status204NoContent);

        _autoMocker.GetMock<IStorageItemStore>()
            .Verify(x => x.Delete(id), Times.Once);
    }

    [Fact]
    public async Task GivenValidatorThrowsNotFoundException_WhenDelete_ThenThrowsNotFoundException()
    {
        //Given
        var id = _fixture.Create<int>();

        var exception = _fixture.Create<NotFoundException>();
        _autoMocker.GetMock<IStorageItemValidator>()
            .Setup(x => x.ThrowIfStorageItemDoesNotExist(id))
            .Throws(exception);

        //When Then
        var act = () => _subject.Delete(id);
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage(exception.Message);

        _autoMocker.GetMock<IStorageItemStore>()
            .Verify(x => x.Delete(id), Times.Never);
    }
}
