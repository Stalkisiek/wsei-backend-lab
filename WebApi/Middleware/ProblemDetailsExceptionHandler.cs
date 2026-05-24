using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Logging;
using CoreApp.Exceptions;

namespace WebApi.Middleware;

public class ProblemDetailsExceptionHandler
{
    private readonly RequestDelegate _next;
    private readonly ProblemDetailsFactory _factory;
    private readonly ILogger<ProblemDetailsExceptionHandler> _logger;

    public ProblemDetailsExceptionHandler(RequestDelegate next, ProblemDetailsFactory factory, ILogger<ProblemDetailsExceptionHandler> logger)
    {
        _next = next;
        _factory = factory;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            if (ex is LecturerNotFoundException)
            {
                _logger.LogInformation($"Exception '{ex.Message}' handled!");
                var problem = _factory.CreateProblemDetails(
                    context,
                    StatusCodes.Status400BadRequest,
                    title: "Contact service error!",
                    type: "Service error",
                    detail: ex.Message
                );
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                context.Response.ContentType = "application/problem+json";
                await context.Response.WriteAsJsonAsync(problem);
                return;
            }

            if (ex is KeyNotFoundException)
            {
                _logger.LogInformation($"KeyNotFound '{ex.Message}' handled as 404!");
                var problem = _factory.CreateProblemDetails(
                    context,
                    StatusCodes.Status404NotFound,
                    title: "Not Found",
                    type: "NotFound",
                    detail: ex.Message
                );
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                context.Response.ContentType = "application/problem+json";
                await context.Response.WriteAsJsonAsync(problem);
                return;
            }

            _logger.LogError(ex, "Unhandled exception");
            var unexpected = _factory.CreateProblemDetails(
                context,
                StatusCodes.Status500InternalServerError,
                title: "Unhandled exception",
                type: "Server error",
                detail: ex.Message
            );
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/problem+json";
            await context.Response.WriteAsJsonAsync(unexpected);
        }
    }
}


