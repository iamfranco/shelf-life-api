using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shelf.Life.API.Controllers;
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
    public async Task GivenRequest_WhenCreate_ThenReturnOkStatus()
    {
        //Given
        var request = _fixture.Create<CreateOrUpdateStorageItemRequest>();

        var createdStorageItem = _fixture.Create<StorageItem>();
        _autoMocker.GetMock<IStorageItemStore>()
            .Setup(x => x.Insert(request))
            .ReturnsAsync(createdStorageItem);

        //When
        var result = (OkObjectResult)await _subject.Create(request);

        //Then
        result.StatusCode.Should().Be(StatusCodes.Status200OK);
        result.Value.Should().Be(createdStorageItem);
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
    public async Task GivenIdAndRequest_WhenUpdate_ThenReturnsOkStatus()
    {
        //Given
        var id = _fixture.Create<int>();
        var request = _fixture.Create<CreateOrUpdateStorageItemRequest>();

        var updatedStorageItem = _fixture.Create<StorageItem>();
        _autoMocker.GetMock<IStorageItemStore>()
            .Setup(x => x.Update(id, request))
            .ReturnsAsync(updatedStorageItem);

        //When
        var result = (OkObjectResult)await _subject.Update(id, request);

        //Then
        result.StatusCode.Should().Be(StatusCodes.Status200OK);
        result.Value.Should().Be(updatedStorageItem);
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
}
