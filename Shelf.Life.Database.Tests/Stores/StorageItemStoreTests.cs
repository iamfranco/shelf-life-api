using Microsoft.EntityFrameworkCore;
using Shelf.Life.Database.Contexts;
using Shelf.Life.Database.Models;
using Shelf.Life.Database.Stores;
using Shelf.Life.Domain.Models;
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
        var storageItem = _fixture.Create<StorageItemDto>();
        var storageItems = _fixture.CreateMany<StorageItemDto>().Append(storageItem);

        _mockContext.Setup(x => x.StorageItems).Returns(GetMockSet(storageItems).Object);

        var id = storageItem.Id;

        //When
        var result = _subject.Delete(id);

        //Then
        _mockContext.Verify(
            x => x.StorageItems.Remove(storageItem),
            Times.Once
        );

        _mockContext.Verify(x => x.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task GivenNoMatchingStorageItemExist_WhenDelete_ThenNoAction()
    {
        //Given
        var id = _fixture.Create<int>();

        var storageItems = Enumerable.Empty<StorageItemDto>();
        _mockContext.Setup(x => x.StorageItems).Returns(GetMockSet(storageItems).Object);

        //When
        var result = _subject.Delete(id);

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
        var id = matchingStorageItem.Id;

        _mockContext.Setup(x => x.StorageItems).Returns(GetMockSet(storageItems).Object);

        //When
        var result = _subject.FindById(id);

        //Then
        result.Should().BeEquivalentTo(matchingStorageItem);
    }

    [Fact]
    public void GivenNoMatchingStorageItemExist_WhenFindById_ThenReturnNull()
    {
        //Given
        var id = _fixture.Create<int>();
        var storageItems = Enumerable.Empty<StorageItemDto>();

        _mockContext.Setup(x => x.StorageItems).Returns(GetMockSet(storageItems).Object);

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

        _mockContext.Setup(x => x.StorageItems).Returns(GetMockSet(storageItems).Object);

        //When
        var result = _subject.Get();

        //Then
        result.Should().BeEquivalentTo(storageItems, options => options.ExcludingMissingMembers());
    }

    [Fact]
    public async Task GivenRequest_WhenInsert_ThenStorageItemInserted()
    {
        //Given
        var request = _fixture.Create<CreateOrUpdateStorageItemRequest>();

        var storageItems = _fixture.CreateMany<StorageItemDto>();

        _mockContext.Setup(x => x.StorageItems).Returns(GetMockSet(storageItems).Object);

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

        _mockContext.Setup(x => x.StorageItems).Returns(GetMockSet(storageItems).Object);

        var id = matchingStorageItem.Id;
        var request = _fixture.Create<CreateOrUpdateStorageItemRequest>();

        //When
        await _subject.Update(id, request);

        //Then
        _mockContext.Verify(
            x => x.StorageItems.Update(
                It.Is<StorageItemDto>(si => si.FoodId == request.FoodId && si.ExpiryDate == request.ExpiryDate)
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
        _mockContext.Setup(x => x.StorageItems).Returns(GetMockSet(storageItems).Object);

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

    private Mock<DbSet<StorageItemDto>> GetMockSet(IEnumerable<StorageItemDto> storageItems)
    {
        var data = storageItems.AsQueryable();

        var mockSet = new Mock<DbSet<StorageItemDto>>();
        mockSet.As<IQueryable<StorageItemDto>>().Setup(m => m.Provider).Returns(data.Provider);
        mockSet.As<IQueryable<StorageItemDto>>().Setup(m => m.Expression).Returns(data.Expression);
        mockSet.As<IQueryable<StorageItemDto>>().Setup(m => m.ElementType).Returns(data.ElementType);
        mockSet.As<IQueryable<StorageItemDto>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

        return mockSet;
    }
}
