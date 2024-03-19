using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Moq.AutoMock;
using Shelf.Life.API.Controllers;
using Shelf.Life.Domain.Models;
using Shelf.Life.Domain.Services;
using System.Net;

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
    public async Task WhenGetAll_ThenReturnsOkStatusAndAllStorageItems()
    {
        //Given
        var storageItems = _fixture.CreateMany<StorageItem>();
        _autoMocker.GetMock<IStorageItemService>()
            .Setup(x => x.Get())
            .ReturnsAsync(storageItems);

        //When
        var result = (OkObjectResult) await _subject.GetAll();

        //Then
        result.StatusCode.Should().Be((int)HttpStatusCode.OK);
        result.Value.Should().Be(storageItems);
    }

    [Fact]
    public async Task GivenStorageItemServiceFails_WhenGetAll_ThenReturnInternalServerError()
    {
        //Given
        var exception = _fixture.Create<Exception>();
        _autoMocker.GetMock<IStorageItemService>()
            .Setup(x => x.Get())
            .ThrowsAsync(exception);

        //When
        var result = (ObjectResult)await _subject.GetAll();

        //Then
        result.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        result.Value.Should().Be($"{nameof(StorageItemService)}:GET throws unexpected exception: {exception.Message}");
    }

    [Fact]
    public async Task GivenRequest_WhenCreate_ThenReturnNoContentStatus()
    {
        //Given
        var request = _fixture.Create<CreateStorageItemRequest>();

        //When
        var result = (NoContentResult)await _subject.Create(request);

        //Then
        result.StatusCode.Should().Be((int)HttpStatusCode.NoContent);

        _autoMocker.GetMock<IStorageItemService>()
            .Verify(x => x.Insert(request), Times.Once);
    }
}
