using Microsoft.EntityFrameworkCore;
using Shelf.Life.Database.Contexts;
using Shelf.Life.Database.Models;
using Shelf.Life.Database.Stores;
using Shelf.Life.Domain.Models;

namespace Shelf.Life.Database.Tests.Stores;
public class FoodStoreTests
{
    private readonly Fixture _fixture = new();

    private readonly FoodStore _subject;
    private readonly Mock<DatabaseContext> _mockContext;

    public FoodStoreTests()
    {
        _mockContext = new Mock<DatabaseContext>();

        _subject = new FoodStore(_mockContext.Object);
    }

    [Fact]
    public async Task GivenMatchingFoodExist_WhenFindByName_ThenReturnMatchingFood()
    {
        //Given
        var matchingFood = _fixture.Create<FoodDto>();
        var foods = _fixture.CreateMany<FoodDto>().Append(matchingFood);
        var name = matchingFood.Name;

        _mockContext.Setup(x => x.Foods).Returns(GetMockSet(foods).Object);

        //When
        var result = await _subject.FindByName(name);

        //Then
        result.Should().BeEquivalentTo(matchingFood);
    }

    [Fact]
    public async Task GivenNoMatchingFoodExist_WhenFindByName_ThenReturnNull()
    {
        //Given
        var foods = _fixture.CreateMany<FoodDto>();
        var name = _fixture.Create<string>();

        _mockContext.Setup(x => x.Foods).Returns(GetMockSet(foods).Object);

        //When
        var result = await _subject.FindByName(name);

        //Then
        result.Should().BeNull();
    }

    [Fact]
    public async Task GivenFoodsExist_WhenGet_ThenReturnAllFoods()
    {
        //Given
        var foods = _fixture.CreateMany<FoodDto>();

        _mockContext.Setup(x => x.Foods).Returns(GetMockSet(foods).Object);

        //When
        var result = await _subject.Get();

        //Then
        result.Should().BeEquivalentTo(foods);
    }

    [Fact]
    public async Task GivenFood_WhenInsert_ThenFoodInserted()
    {
        //Given
        var request = _fixture.Create<CreateFoodRequest>();

        var foods = _fixture.CreateMany<FoodDto>();

        _mockContext.Setup(x => x.Foods).Returns(GetMockSet(foods).Object);

        //When
        await _subject.Insert(request);

        //Then
        _mockContext.Verify(x => x.Foods.AddAsync(
                It.Is<FoodDto>(f => IsFoodDtoMatchingRequest(f, request)),
                default
            ), Times.Once);

        _mockContext.Verify(x => x.SaveChangesAsync(default), Times.Once);
    }

    private bool IsFoodDtoMatchingRequest(FoodDto foodDto, CreateFoodRequest request)
    {
        return foodDto.Name == request.Name;
    }

    private Mock<DbSet<FoodDto>> GetMockSet(IEnumerable<FoodDto> foods)
    {
        var data = foods.AsQueryable();

        var mockSet = new Mock<DbSet<FoodDto>>();
        mockSet.As<IQueryable<FoodDto>>().Setup(m => m.Provider).Returns(data.Provider);
        mockSet.As<IQueryable<FoodDto>>().Setup(m => m.Expression).Returns(data.Expression);
        mockSet.As<IQueryable<FoodDto>>().Setup(m => m.ElementType).Returns(data.ElementType);
        mockSet.As<IQueryable<FoodDto>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

        return mockSet;
    }
}
