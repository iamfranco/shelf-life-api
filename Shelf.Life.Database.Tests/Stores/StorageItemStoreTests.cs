using Shelf.Life.Database.Contexts;
using Shelf.Life.Database.Models;
using Shelf.Life.Database.Stores;
using Shelf.Life.Database.Tests.TestHelpers;
using Shelf.Life.Domain.Models.Requests;

namespace Shelf.Life.Database.Tests.Stores;
public class StorageItemStoreTests
{
    private readonly Fixture _fixture = new();

    private readonly StorageItemStore _subject;
    private readonly Mock<DatabaseContext> _mockContext;

    public StorageItemStoreTests()
    {
        _mockContext = new Mock<DatabaseContext>();
        _subject = new StorageItemStore(_mockContext.Object);
    }

    [Fact]
    public async Task GivenId_WhenDelete_ThenStorageItemDeleted()
    {
        //Given
        var matchingStorageItem = _fixture.Create<StorageItemDto>();
        var storageItems = _fixture.CreateMany<StorageItemDto>().Append(matchingStorageItem);
        SetDbStorageItems(storageItems);

        var id = matchingStorageItem.Id;

        //When
        await _subject.Delete(id);

        //Then
        _mockContext.Verify(
            x => x.StorageItems.Remove(matchingStorageItem),
            Times.Once
        );

        _mockContext.Verify(x => x.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task GivenNoMatchingStorageItemExist_WhenDelete_ThenNoAction()
    {
        //Given
        var storageItems = Enumerable.Empty<StorageItemDto>();
        SetDbStorageItems(storageItems);

        var id = _fixture.Create<int>();

        //When
        await _subject.Delete(id);

        //Then
        _mockContext.Verify(
            x => x.StorageItems.Remove(It.IsAny<StorageItemDto>()),
            Times.Never
        );

        _mockContext.Verify(x => x.SaveChangesAsync(default), Times.Never);
    }

    [Fact]
    public void GivenMatchingStorageItemExists_WhenFindById_ThenReturnMatchingStorageItem()
    {
        //Given
        var matchingStorageItem = _fixture.Create<StorageItemDto>();
        var storageItems = _fixture.CreateMany<StorageItemDto>().Append(matchingStorageItem);
        SetDbStorageItems(storageItems);

        var id = matchingStorageItem.Id;

        //When
        var result = _subject.FindById(id);

        //Then
        result.Should().BeEquivalentTo(matchingStorageItem);
    }

    [Fact]
    public void GivenNoMatchingStorageItemExist_WhenFindById_ThenReturnNull()
    {
        //Given
        var storageItems = Enumerable.Empty<StorageItemDto>();
        SetDbStorageItems(storageItems);

        var id = _fixture.Create<int>();

        //When
        var result = _subject.FindById(id);

        //Then
        result.Should().BeNull();
    }

    [Fact]
    public void GivenStorageItemExist_WhenGet_ThenReturnAllStorageItems()
    {
        //Given
        var storageItems = _fixture.CreateMany<StorageItemDto>();
        SetDbStorageItems(storageItems);

        //When
        var result = _subject.Get();

        //Then
        result.Should().BeEquivalentTo(storageItems, options => options.ExcludingMissingMembers());
    }

    [Fact]
    public async Task GivenRequest_WhenInsert_ThenStorageItemInserted()
    {
        //Given
        var storageItems = _fixture.CreateMany<StorageItemDto>();
        SetDbStorageItems(storageItems);

        var request = _fixture.Create<CreateOrUpdateStorageItemRequest>();

        //When
        await _subject.Insert(request);

        //Then
        _mockContext.Verify(x => x.StorageItems.AddAsync(
                It.Is<StorageItemDto>(si => IsStorageItemDtoMatchingRequest(si, request)),
                default
            ), Times.Once);

        _mockContext.Verify(x => x.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task GivenIdAndRequest_WhenUpdate_ThenStorageItemUpdated()
    {
        //Given
        var matchingStorageItem = _fixture.Create<StorageItemDto>();
        var storageItems = _fixture.CreateMany<StorageItemDto>().Append(matchingStorageItem);
        SetDbStorageItems(storageItems);

        var id = matchingStorageItem.Id;
        var request = _fixture.Create<CreateOrUpdateStorageItemRequest>();

        //When
        await _subject.Update(id, request);

        //Then
        _mockContext.Verify(
            x => x.StorageItems.Update(
                It.Is<StorageItemDto>(si => si.Id == id && IsStorageItemDtoMatchingRequest(si, request))
            ),
            Times.Once
        );

        _mockContext.Verify(x => x.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task GivenNoMatchingStorageItemExists_WhenUpdate_ThenNoAction()
    {
        //Given
        var storageItems = Enumerable.Empty<StorageItemDto>();
        SetDbStorageItems(storageItems);

        var id = _fixture.Create<int>();
        var request = _fixture.Create<CreateOrUpdateStorageItemRequest>();

        //When
        await _subject.Update(id, request);

        //Then
        _mockContext.Verify(
            x => x.StorageItems.Update(It.IsAny<StorageItemDto>()),
            Times.Never
        );

        _mockContext.Verify(x => x.SaveChangesAsync(default), Times.Never);
    }

    private bool IsStorageItemDtoMatchingRequest(StorageItemDto storageItemDto, CreateOrUpdateStorageItemRequest request)
    {
        return storageItemDto.FoodId == request.FoodId &&
            storageItemDto.ExpiryDate == request.ExpiryDate;
    }

    private void SetDbStorageItems(IEnumerable<StorageItemDto> storageItems)
    {
        _mockContext.Setup(x => x.StorageItems).Returns(DbMockTestHelper.GetMockSet(storageItems).Object);
    }
}
