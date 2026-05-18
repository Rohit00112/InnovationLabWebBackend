using System.Text.Json;
using System.Net.Mime;
using Microsoft.AspNetCore.Http;
using InnovationLab.Shared.Dtos;

namespace InnovationLab.Shared.Middlewares;

public class GlobalExceptionMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            var traceId = context.TraceIdentifier;
            int statusCode = 500;
            string message = "An unexpected error occurred.";
            List<string> errors = [ex.Message];

            var errorResponse = new ErrorResponseDto(traceId, errors);

            var generalResponse = new GeneralResponseDto<ErrorResponseDto>
            {
                StatusCode = statusCode,
                Message = message,
                Result = errorResponse
            };

            context.Response.StatusCode = statusCode;
            context.Response.ContentType = MediaTypeNames.Application.Json;
            var json = JsonSerializer.Serialize(generalResponse);
            await context.Response.WriteAsync(json);
        }
    }
}