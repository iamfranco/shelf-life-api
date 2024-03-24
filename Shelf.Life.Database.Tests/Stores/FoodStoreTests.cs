using Newtonsoft.Json;
using Shelf.Life.Database.Contexts;
using Shelf.Life.Database.Models;
using Shelf.Life.Database.Stores;
using Shelf.Life.Database.Tests.TestHelpers;
using Shelf.Life.Domain.Exceptions;
using Shelf.Life.Domain.Models;
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
    public void GivenNoMatchingFoodExists_WhenFindById_ThenThrowNotFoundException()
    {
        //Given
        var foods = _fixture.CreateMany<FoodDto>();
        SetDbFoods(foods);

        var id = _fixture.Create<int>();

        //When Then
        var act = () => _subject.FindById(id);
        act.Should().Throw<NotFoundException>()
            .WithMessage($"{nameof(Food)} with id [{id}] does NOT exist.");
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
        var result = await _subject.Insert(request);

        //Then
        _mockContext.Verify(x => x.Foods.Add(
                It.Is<FoodDto>(f => IsFoodDtoMatchingRequest(f, request))
            ), Times.Once);

        _mockContext.Verify(x => x.SaveChangesAsync(default), Times.Once);

        result.Should().BeEquivalentTo(request);
    }

    [Fact]
    public async Task GivenFoodAlreadyExist_WhenInsert_ThenThrowBadRequestException()
    {
        //Given
        var request = _fixture.Create<CreateOrUpdateFoodRequest>();

        var matchingFood = _fixture.Build<FoodDto>()
            .With(x => x.Name, request.Name).Create();
        var foods = _fixture.CreateMany<FoodDto>().Append(matchingFood);
        SetDbFoods(foods);

        //When Then
        var act = () => _subject.Insert(request);
        await act.Should().ThrowAsync<BadRequestException>()
            .WithMessage($"{nameof(Food)} with name [{request.Name}] already exist. Food: {JsonConvert.SerializeObject(matchingFood)}");

        _mockContext.Verify(x => x.Foods.Add(It.IsAny<FoodDto>()), Times.Never);
        _mockContext.Verify(x => x.SaveChangesAsync(default), Times.Never);
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
        var result = await _subject.Update(id, request);

        //Then
        _mockContext.Verify(
            x => x.Foods.Update(
                It.Is<FoodDto>(foodDto => foodDto.Id == id && IsFoodDtoMatchingRequest(foodDto, request))
            ),
            Times.Once
        );

        _mockContext.Verify(x => x.SaveChangesAsync(default), Times.Once);

        result.Should().BeEquivalentTo(request);
    }

    [Fact]
    public async Task GivenNoMatchingFoodForId_WhenUpdate_ThenThrowNotFoundException()
    {
        //Given
        var foods = Enumerable.Empty<FoodDto>();
        SetDbFoods(foods);

        var id = _fixture.Create<int>();
        var request = _fixture.Create<CreateOrUpdateFoodRequest>();

        //When Then
        var act = () => _subject.Update(id, request);
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"{nameof(Food)} with id [{id}] does NOT exist.");

        _mockContext.Verify(x => x.Foods.Update(It.IsAny<FoodDto>()), Times.Never);
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
    public async Task GivenNoMatchingFoodForId_WhenDelete_ThenThrowNotFoundException()
    {
        //Given
        var id = _fixture.Create<int>();

        var foods = Enumerable.Empty<FoodDto>();
        SetDbFoods(foods);

        //When Then
        var act = () => _subject.Delete(id);
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"{nameof(Food)} with id [{id}] does NOT exist.");

        _mockContext.Verify(x => x.Foods.Remove(It.IsAny<FoodDto>()), Times.Never);
        _mockContext.Verify(x => x.SaveChangesAsync(default), Times.Never);
    }

    private static bool IsFoodDtoMatchingRequest(FoodDto foodDto, CreateOrUpdateFoodRequest request)
    {
        return foodDto.Name == request.Name;
    }

    private void SetDbFoods(IEnumerable<FoodDto> foods)
    {
        _mockContext.Setup(x => x.Foods).Returns(DbMockTestHelper.GetMockSet(foods).Object);
    }
}
