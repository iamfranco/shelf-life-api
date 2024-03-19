using AutoFixture;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using Shelf.Life.Domain.Models;
using Shelf.Life.Domain.Services;
using Shelf.Life.Domain.Stores;

namespace Shelf.Life.Domain.Tests.Services;
public class FoodServiceTests
{
    private readonly Fixture _fixture = new();
    private readonly AutoMocker _autoMocker = new();

    private readonly FoodService _subject;
    public FoodServiceTests()
    {
        _subject = _autoMocker.CreateInstance<FoodService>();
    }

    [Fact]
    public async Task GivenMatchingFoodExist_WhenFindMatchingFood_ThenReturnMatchingFood()
    {
        //Given
        var request = _fixture.Create<CreateFoodRequest>();
        var matchingFood = _fixture.Create<Food>();
        _autoMocker.GetMock<IFoodStore>()
            .Setup(x => x.FindByName(request.Name))
            .ReturnsAsync(matchingFood);

        //When
        var result = await _subject.FindMatchingFood(request);

        //Then
        result.Should().Be(matchingFood);
    }

    [Fact]
    public async Task GivenNoMatchingFood_WhenFindMatchingFood_ThenReturnNull()
    {
        //Given
        var request = _fixture.Create<CreateFoodRequest>();
        _autoMocker.GetMock<IFoodStore>()
            .Setup(x => x.FindByName(request.Name))
            .ReturnsAsync((Food?)null);

        //When
        var result = await _subject.FindMatchingFood(request);

        //Then
        result.Should().BeNull();
    }

    [Fact]
    public async Task WhenGet_ThenReturnsAllFoods()
    {
        //Given
        var foods = _fixture.CreateMany<Food>();
        _autoMocker.GetMock<IFoodStore>()
            .Setup(x => x.Get())
            .ReturnsAsync(foods);

        //When
        var result = await _subject.Get();

        //Then
        result.Should().BeEquivalentTo(foods);
    }

    [Fact]
    public async Task GivenRequest_WhenInsert_ThenInsertCalled()
    {
        //Given
        var request = _fixture.Create<CreateFoodRequest>();

        //When
        await _subject.Insert(request);

        //Then
        _autoMocker.GetMock<IFoodStore>()
            .Verify(x => x.Insert(request), Times.Once);
    }

    [Fact]
    public async Task GivenFoodWithMatchingNameExists_WhenInsert_ThenInsertNotCalled()
    {
        //Given
        var request = _fixture.Create<CreateFoodRequest>();
        var matchingFood = _fixture.Create<Food>();
        _autoMocker.GetMock<IFoodStore>()
            .Setup(x => x.FindByName(request.Name))
            .ReturnsAsync(matchingFood);

        //When
        await _subject.Insert(request);

        //Then
        _autoMocker.GetMock<IFoodStore>()
            .Verify(x => x.Insert(request), Times.Never);
    }
}
