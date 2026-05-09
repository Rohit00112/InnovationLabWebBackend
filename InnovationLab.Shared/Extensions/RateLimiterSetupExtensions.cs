using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;
using System.Threading.RateLimiting;

namespace InnovationLab.Shared.Extensions;

public static class RateLimiterSetupExtensions
{
    public static IServiceCollection AddSlidingWindowRateLimiter(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
            {
                var ipAddress =
                    httpContext.Connection.RemoteIpAddress?.ToString();

                ipAddress ??= "unknown";

                if (ipAddress is "::1" or "127.0.0.1")
                {
                    return RateLimitPartition.GetNoLimiter(ipAddress);
                }

                return RateLimitPartition.GetSlidingWindowLimiter(
                    partitionKey: ipAddress,
                    factory: _ => new SlidingWindowRateLimiterOptions
                    {
                        PermitLimit = 100,
                        Window = TimeSpan.FromMinutes(1),
                        SegmentsPerWindow = 6,
                        QueueLimit = 0,
                        QueueProcessingOrder =
                            QueueProcessingOrder.OldestFirst,
                        AutoReplenishment = true
                    });
            });

            options.OnRejected = async (context, cancellationToken) =>
            {
                context.HttpContext.Response.StatusCode =
                    StatusCodes.Status429TooManyRequests;

                context.HttpContext.Response.ContentType =
                    "application/json";

                if (context.Lease.TryGetMetadata(
                        MetadataName.RetryAfter,
                        out var retryAfter))
                {
                    context.HttpContext.Response.Headers.RetryAfter =
                        ((int)retryAfter.TotalSeconds)
                        .ToString(NumberFormatInfo.InvariantInfo);
                }

                var response = new
                {
                    error = "rate_limit_exceeded",
                    message = "Too many requests. Please try again later."
                };

                await context.HttpContext.Response.WriteAsJsonAsync(
                    response,
                    cancellationToken);
            };
        });

        return services;
    }
}