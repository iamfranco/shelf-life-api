using Microsoft.EntityFrameworkCore;
using Shelf.Life.Database.Contexts;
using Shelf.Life.Database.Models;
using Shelf.Life.Database.Stores;
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
    public async Task GivenFoodsExist_WhenGet_ThenReturnAllFoods()
    {
        //Given
        var storageItems = _fixture.CreateMany<StorageItemDto>();

        _mockContext.Setup(x => x.StorageItems).Returns(GetMockSet(storageItems).Object);

        //When
        var result = await _subject.Get();

        //Then
        result.Should().BeEquivalentTo(storageItems, options => options.ExcludingMissingMembers());
    }

    [Fact]
    public async Task GivenFood_WhenInsert_ThenFoodInserted()
    {
        //Given
        var request = _fixture.Create<CreateStorageItemRequest>();

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

    private bool IsStorageItemDtoMatchingRequest(StorageItemDto storageItemDto, CreateStorageItemRequest request)
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
