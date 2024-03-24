using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Shelf.Life.API.Middlewares;
using Shelf.Life.API.Models;
using Shelf.Life.API.Validators.Models;
using System.Text;

namespace Shelf.Life.API.Tests.Middlewares;
public class ExceptionHandlingMiddlewareTests
{
    private readonly Fixture _fixture = new();

    [Fact]
    public async Task GivenNextSuccessful_WhenInvokeAsync_ThenNextCalled()
    {
        //Given
        var mockNext = new Mock<RequestDelegate>();
        var subject = new ExceptionHandlingMiddleware(next: mockNext.Object);

        var context = GetTestContext();

        //When
        await subject.InvokeAsync(context);

        //Then
        mockNext.Verify(x => x.Invoke(context), Times.Once);
    }

    [Fact]
    public async Task GivenNextThrowsException_WhenInvokeAsync_ThenContextResponseHasInternalServerErrorWithErrorResponse()
    {
        //Given
        var exception = _fixture.Create<Exception>();
        var subject = new ExceptionHandlingMiddleware(next: (context) => throw exception);

        var requestBody = _fixture.Create<string>();
        var context = GetTestContext(requestBody);
        string expectedResponseBody = CreateExpectedResponseBody(exception, requestBody, context);

        //When
        await subject.InvokeAsync(context);

        //Then
        context.Response.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);

        var responseBody = await ReadStreamAsync(context.Response.Body);
        responseBody.Should().BeEquivalentTo(expectedResponseBody);
    }

    [Fact]
    public async Task GivenNextThrowsNotFoundException_WhenInvokeAsync_ThenContextResponseHasNotFoundStatusWithErrorResponse()
    {
        //Given
        var exception = _fixture.Create<NotFoundException>();
        var subject = new ExceptionHandlingMiddleware(next: (context) => throw exception);

        var requestBody = _fixture.Create<string>();
        var context = GetTestContext(requestBody);
        string expectedResponseBody = CreateExpectedResponseBody(exception, requestBody, context);

        //When
        await subject.InvokeAsync(context);

        //Then
        context.Response.StatusCode.Should().Be(StatusCodes.Status404NotFound);

        var responseBody = await ReadStreamAsync(context.Response.Body);
        responseBody.Should().BeEquivalentTo(expectedResponseBody);
    }

    [Fact]
    public async Task GivenNextThrowsBadRequestException_WhenInvokeAsync_ThenContextResponseHasBadRequestStatusWithErrorResponse()
    {
        //Given
        var exception = _fixture.Create<BadRequestException>();
        var subject = new ExceptionHandlingMiddleware(next: (context) => throw exception);

        var requestBody = _fixture.Create<string>();
        var context = GetTestContext(requestBody);
        string expectedResponseBody = CreateExpectedResponseBody(exception, requestBody, context);

        //When
        await subject.InvokeAsync(context);

        //Then
        context.Response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);

        var responseBody = await ReadStreamAsync(context.Response.Body);
        responseBody.Should().BeEquivalentTo(expectedResponseBody);
    }

    private HttpContext GetTestContext(string? requestBody = null)
    {
        var context = new DefaultHttpContext();
        context.Request.Method = _fixture.Create<string>();
        context.Request.Path = "/api/test";
        context.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(requestBody ?? _fixture.Create<string>()));
        context.Response.Body = new MemoryStream();
        return context;
    }

    private static string CreateExpectedResponseBody(Exception exception, string requestBody, HttpContext context)
    {
        return JsonConvert.SerializeObject(new ErrorResponse(
            exception.Message,
            context.Request.Method,
            context.Request.Path,
            requestBody
        ));
    }

    private static async Task<string> ReadStreamAsync(Stream stream)
    {
        using var sr = new StreamReader(stream);
        sr.BaseStream.Position = 0;

        return await sr.ReadToEndAsync();
    }
}
