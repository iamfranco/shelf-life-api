using Shelf.Life.Database.Models;
using Shelf.Life.Domain.Models;
using Shelf.Life.Domain.Models.Requests;

namespace Shelf.Life.Database.Tests.Models;
public class StorageItemDtoTests
{
    private readonly Fixture _fixture = new();

    [Fact]
    public void GivenStorageItemDto_WhenToStorageItem_ThenReturnStorageItem()
    {
        //Given
        var storageItemDto = _fixture.Create<StorageItemDto>();

        //When
        var result = storageItemDto.ToStorageItem();

        //Then
        result.GetType().Should().Be(typeof(StorageItem));
        result.Should().BeEquivalentTo(storageItemDto, options => options.ExcludingMissingMembers());
    }

    [Fact]
    public void GivenRequest_WhenFromRequest_ThenReturnStorageItemDto()
    {
        //Given
        var request = _fixture.Create<CreateStorageItemRequest>();

        //When
        var result = StorageItemDto.FromRequest(request);

        //Then
        result.GetType().Should().Be(typeof(StorageItemDto));
        result.Should().BeEquivalentTo(request);
    }
}
