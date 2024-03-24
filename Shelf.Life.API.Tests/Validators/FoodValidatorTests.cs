using Newtonsoft.Json;
using Shelf.Life.API.Validators;
using Shelf.Life.API.Validators.Models;
using Shelf.Life.Domain.Models;
using Shelf.Life.Domain.Stores;

namespace Shelf.Life.API.Tests.Validators;
public class FoodValidatorTests
{
    private readonly Fixture _fixture = new();
    private readonly AutoMocker _autoMocker = new();

    private readonly FoodValidator _subject;

    public FoodValidatorTests()
    {
        _subject = _autoMocker.CreateInstance<FoodValidator>();
    }

    [Fact]
    public void GivenNoMatchingFoodExist_WhenThrowIfFoodDoesNotExist_ThenThrowNotFoundException()
    {
        //Given
        var id = _fixture.Create<int>();

        //When Then
        var act = () => _subject.ThrowIfFoodDoesNotExist(id);
        act.Should().Throw<NotFoundException>()
            .WithMessage($"{nameof(Food)} with id [{id}] does NOT exist.");
    }

    [Fact]
    public void GivenMatchingFoodExists_WhenThrowIfFoodDoesNotExist_ThenNoExceptionThrown()
    {
        //Given
        var id = _fixture.Create<int>();

        var food = _fixture.Create<Food>();
        _autoMocker.GetMock<IFoodStore>()
            .Setup(x => x.FindById(id))
            .Returns(food);

        //When Then
        var act = () => _subject.ThrowIfFoodDoesNotExist(id);
        act.Should().NotThrow<Exception>();
    }

    [Fact]
    public void GivenMatchingFoodExists_WhenThrowIfFoodExists_ThenThrowBadRequestException()
    {
        //Given
        var name = _fixture.Create<string>();

        var food = _fixture.Create<Food>();
        _autoMocker.GetMock<IFoodStore>()
            .Setup(x => x.FindByName(name))
            .Returns(food);

        //When Then
        var act = () => _subject.ThrowIfFoodExists(name);
        act.Should().Throw<BadRequestException>()
            .WithMessage($"{nameof(Food)} with name [{name}] already exists. Food: {JsonConvert.SerializeObject(food)}");
    }

    [Fact]
    public void GivenNoMatchingFoodExist_WhenThrowIfFoodExists_ThenNoExceptionThrown()
    {
        //Given
        var name = _fixture.Create<string>();

        //When Then
        var act = () => _subject.ThrowIfFoodExists(name);
        act.Should().NotThrow<Exception>();
    }
}
