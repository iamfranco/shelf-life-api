using Microsoft.EntityFrameworkCore;
using Shelf.Life.Database.Contexts;
using Shelf.Life.Database.Models;
using Shelf.Life.Database.Stores;
using Shelf.Life.Domain.Models.Requests;

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
    public void GivenMatchingFoodExist_WhenFindByName_ThenReturnMatchingFood()
    {
        //Given
        var matchingFood = _fixture.Create<FoodDto>();
        var foods = _fixture.CreateMany<FoodDto>().Append(matchingFood);
        var name = matchingFood.Name;

        _mockContext.Setup(x => x.Foods).Returns(GetMockSet(foods).Object);

        //When
        var result = _subject.FindByName(name);

        //Then
        result.Should().BeEquivalentTo(matchingFood);
    }

    [Fact]
    public void GivenNoMatchingFoodExist_WhenFindByName_ThenReturnNull()
    {
        //Given
        var foods = _fixture.CreateMany<FoodDto>();
        var name = _fixture.Create<string>();

        _mockContext.Setup(x => x.Foods).Returns(GetMockSet(foods).Object);

        //When
        var result = _subject.FindByName(name);

        //Then
        result.Should().BeNull();
    }

    [Fact]
    public void GivenMatchingFoodExists_WhenFindById_ThenReturnMatchingFood()
    {
        //Given
        var matchingFood = _fixture.Create<FoodDto>();
        var foods = _fixture.CreateMany<FoodDto>().Append(matchingFood);
        var id = matchingFood.Id;

        _mockContext.Setup(x => x.Foods).Returns(GetMockSet(foods).Object);

        //When
        var result = _subject.FindById(id);

        //Then
        result.Should().BeEquivalentTo(matchingFood);
    }

    [Fact]
    public void GivenNoNoMatchingFoodExists_WhenFindById_ThenReturnNull()
    {
        //Given
        var foods = _fixture.CreateMany<FoodDto>();
        var id = _fixture.Create<int>();

        _mockContext.Setup(x => x.Foods).Returns(GetMockSet(foods).Object);

        //When
        var result = _subject.FindById(id);

        //Then
        result.Should().BeNull();
    }

    [Fact]
    public void GivenFoodsExist_WhenGet_ThenReturnAllFoods()
    {
        //Given
        var foods = _fixture.CreateMany<FoodDto>();

        _mockContext.Setup(x => x.Foods).Returns(GetMockSet(foods).Object);

        //When
        var result = _subject.Get();

        //Then
        result.Should().BeEquivalentTo(foods);
    }

    [Fact]
    public void GivenMatchingFoodsExist_WhenQueryByPartialName_ThenReturnMatchingFoods()
    {
        //Given
        var partialName = _fixture.Create<string>();

        var matchingFoodsSameCase = _fixture.Build<FoodDto>()
            .With(x => x.Name, partialName + _fixture.Create<string>())
            .CreateMany();

        var matchingFoodsWrongCase = _fixture.Build<FoodDto>()
            .With(x => x.Name, partialName.ToUpper() + _fixture.Create<string>())
            .CreateMany();

        var matchingFoods = matchingFoodsSameCase.Concat(matchingFoodsWrongCase);

        var foods = _fixture.CreateMany<FoodDto>().Concat(matchingFoods);

        _mockContext.Setup(x => x.Foods).Returns(GetMockSet(foods).Object);

        //When
        var result = _subject.QueryByPartialName(partialName);

        //Then
        result.Should().BeEquivalentTo(matchingFoods);
    }

    [Fact]
    public async Task GivenFood_WhenInsert_ThenFoodInserted()
    {
        //Given
        var request = _fixture.Create<CreateOrUpdateFoodRequest>();

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

    [Fact]
    public async Task GivenUpdateRequest_WhenUpdate_ThenFoodUpdated()
    {
        //Given
        var matchingFood = _fixture.Create<FoodDto>();
        var foods = _fixture.CreateMany<FoodDto>().Append(matchingFood);

        _mockContext.Setup(x => x.Foods).Returns(GetMockSet(foods).Object);

        var id = matchingFood.Id;
        var request = _fixture.Create<CreateOrUpdateFoodRequest>();

        //When
        await _subject.Update(id, request);

        //Then
        _mockContext.Verify(
            x => x.Foods.Update(
                It.Is<FoodDto>(foodDto => foodDto.Id == id && foodDto.Name == request.Name)
            ),
            Times.Once
        );

        _mockContext.Verify(x => x.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task GivenNoMatchingFoodForId_WhenUpdate_ThenNoAction()
    {
        //Given
        var foods = Enumerable.Empty<FoodDto>();
        _mockContext.Setup(x => x.Foods).Returns(GetMockSet(foods).Object);

        var id = _fixture.Create<int>();
        var request = _fixture.Create<CreateOrUpdateFoodRequest>();

        //When
        await _subject.Update(id, request);

        //Then
        _mockContext.Verify(
            x => x.Foods.Update(It.IsAny<FoodDto>()),
            Times.Never
        );

        _mockContext.Verify(x => x.SaveChangesAsync(default), Times.Never);
    }

    [Fact]
    public async Task GivenId_WhenDelete_ThenFoodDeleted()
    {
        //Given
        var matchingFood = _fixture.Create<FoodDto>();
        var foods = _fixture.CreateMany<FoodDto>().Append(matchingFood);

        _mockContext.Setup(x => x.Foods).Returns(GetMockSet(foods).Object);

        var id = matchingFood.Id;

        //When
        await _subject.Delete(id);

        //Then
        _mockContext.Verify(
            x => x.Foods.Remove(matchingFood),
            Times.Once
        );

        _mockContext.Verify(x => x.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task GivenNoMatchingFoodForId_WhenDelete_ThenNoAction()
    {
        //Given
        var id = _fixture.Create<int>();

        var foods = Enumerable.Empty<FoodDto>();
        _mockContext.Setup(x => x.Foods).Returns(GetMockSet(foods).Object);

        //When
        await _subject.Delete(id);

        //Then
        _mockContext.Verify(
            x => x.Foods.Remove(It.IsAny<FoodDto>()),
            Times.Never
        );

        _mockContext.Verify(x => x.SaveChangesAsync(default), Times.Never);
    }

    private bool IsFoodDtoMatchingRequest(FoodDto foodDto, CreateOrUpdateFoodRequest request)
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
