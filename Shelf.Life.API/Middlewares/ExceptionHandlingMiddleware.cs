using Shelf.Life.API.Models;
using Shelf.Life.Domain.Exceptions;

namespace Shelf.Life.API.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            context.Request.EnableBuffering();

            await _next(context);
        }
        catch (NotFoundException exception)
        {
            await HandleExceptionAsync(context, exception, StatusCodes.Status404NotFound);
        }
        catch (BadRequestException exception)
        {
            await HandleExceptionAsync(context, exception, StatusCodes.Status400BadRequest);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception, StatusCodes.Status500InternalServerError);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception, int statusCode)
    {
        context.Response.StatusCode = statusCode;

        var body = await ReadStreamAsync(context.Request.Body);

        var errorResponse = new ErrorResponse(
            exception.Message,
            context.Request.Method,
            context.Request.Path,
            body
        );

        await context.Response.WriteAsJsonAsync(errorResponse);
    }

    private static async Task<string> ReadStreamAsync(Stream stream)
    {
        using var sr = new StreamReader(stream);
        sr.BaseStream.Position = 0;
        var requestBodyString = await sr.ReadToEndAsync();
        return requestBodyString;
    }
}
