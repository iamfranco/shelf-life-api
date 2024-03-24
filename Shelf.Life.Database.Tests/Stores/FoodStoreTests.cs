using Shelf.Life.Database.Contexts;
using Shelf.Life.Database.Models;
using Shelf.Life.Database.Stores;
using Shelf.Life.Database.Tests.TestHelpers;
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
        SetDbFoods(foods);

        var name = matchingFood.Name;

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
        SetDbFoods(foods);

        var name = _fixture.Create<string>();

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
        SetDbFoods(foods);

        var id = matchingFood.Id;

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
        SetDbFoods(foods);

        var id = _fixture.Create<int>();

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
        SetDbFoods(foods);

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
        SetDbFoods(foods);

        //When
        var result = _subject.QueryByPartialName(partialName);

        //Then
        result.Should().BeEquivalentTo(matchingFoods);
    }

    [Fact]
    public async Task GivenFood_WhenInsert_ThenFoodInserted()
    {
        //Given
        var foods = _fixture.CreateMany<FoodDto>();
        SetDbFoods(foods);

        var request = _fixture.Create<CreateOrUpdateFoodRequest>();

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
        SetDbFoods(foods);

        var id = matchingFood.Id;
        var request = _fixture.Create<CreateOrUpdateFoodRequest>();

        //When
        await _subject.Update(id, request);

        //Then
        _mockContext.Verify(
            x => x.Foods.Update(
                It.Is<FoodDto>(foodDto => foodDto.Id == id && IsFoodDtoMatchingRequest(foodDto, request))
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
        SetDbFoods(foods);

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
        SetDbFoods(foods);

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
        SetDbFoods(foods);

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

    private void SetDbFoods(IEnumerable<FoodDto> foods)
    {
        _mockContext.Setup(x => x.Foods).Returns(DbMockTestHelper.GetMockSet(foods).Object);
    }
}
