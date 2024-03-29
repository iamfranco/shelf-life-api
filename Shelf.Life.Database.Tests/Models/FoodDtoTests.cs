﻿using Shelf.Life.Database.Models;
using Shelf.Life.Domain.Models;
using Shelf.Life.Domain.Models.Requests;

namespace Shelf.Life.Database.Tests.Models;
public class FoodDtoTests
{
    private readonly Fixture _fixture = new();

    [Fact]
    public void GivenFoodDto_WhenToFood_ThenReturnFood()
    {
        //Given
        var foodDto = _fixture.Create<FoodDto>();

        //When
        var result = foodDto.ToFood();

        //Then
        result.GetType().Should().Be(typeof(Food));
        result.Should().BeEquivalentTo(foodDto);
    }

    [Fact]
    public void GivenRequest_WhenFromRequest_ThenReturnFoodDto()
    {
        //Given
        var request = _fixture.Create<CreateOrUpdateFoodRequest>();

        //When
        var result = FoodDto.FromRequest(request);

        //Then
        result.GetType().Should().Be(typeof(FoodDto));
        result.Should().BeEquivalentTo(request);
    }

    [Fact]
    public void GivenUpdateRequest_WhenUpdate_ThenUpdatesFoodDto()
    {
        //Given
        var foodDto = _fixture.Create<FoodDto>();
        var request = _fixture.Create<CreateOrUpdateFoodRequest>();

        //When
        foodDto.Update(request);

        //Then
        foodDto.Should().BeEquivalentTo(request);
    }
}
