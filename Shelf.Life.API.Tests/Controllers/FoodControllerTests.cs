using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq.AutoMock;
using Shelf.Life.API.Controllers;
using System.Net;

namespace Shelf.Life.API.Tests.Controllers;
public class FoodControllerTests
{
    private readonly AutoMocker _autoMocker = new();
    private readonly Fixture _fixture = new();

    private readonly FoodController _subject;
    public FoodControllerTests()
    {
        _subject = _autoMocker.CreateInstance<FoodController>();
    }

    [Fact]
    public async Task WhenGetAll_ThenReturnOkStatus()
    {
        //When
        var result = (OkResult) await _subject.GetAll();

        //Then
        result.StatusCode.Should().Be((int)HttpStatusCode.OK);
    }
}
