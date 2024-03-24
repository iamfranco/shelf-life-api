using Shelf.Life.Domain.Models;
using Shelf.Life.Domain.Models.Requests;
using Shelf.Life.Domain.Services;
using Shelf.Life.Domain.Stores;

namespace Shelf.Life.Domain.Tests.Services;
public class StorageItemServiceTests
{
    private readonly Fixture _fixture = new();
    private readonly AutoMocker _autoMocker = new();

    private readonly StorageItemService _subject;

    public StorageItemServiceTests()
    {
        _subject = _autoMocker.CreateInstance<StorageItemService>();
    }

    [Fact]
    public async Task WhenGet_ThenReturnsAllStorageItems()
    {
        //Given
        var storageItems = _fixture.CreateMany<StorageItem>();
        _autoMocker.GetMock<IStorageItemStore>()
            .Setup(x => x.Get())
            .ReturnsAsync(storageItems);

        //When
        var result = await _subject.Get();

        //Then
        result.Should().BeEquivalentTo(storageItems);
    }

    [Fact]
    public async Task GivenRequest_WhenInsert_ThenInsertCalled()
    {
        //Given
        var request = _fixture.Create<CreateStorageItemRequest>();

        //When
        await _subject.Insert(request);

        //Then
        _autoMocker.GetMock<IStorageItemStore>()
            .Verify(x => x.Insert(request), Times.Once);
    }
}
