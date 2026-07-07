using Grpc.Core;
using Grpc.Core.Interceptors;

namespace WebpConverter.Infrastructure.Interceptors;

public class ExceptionInterceptor(ILogger<ExceptionInterceptor> logger) : Interceptor
{
    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            return await continuation(request, context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unhandled exception occurred during the gRPC call.");
            
            // Convertimos la excepción en un RpcException estándar
            var status = new Status(StatusCode.Internal, "An unexpected error occurred processing the request.");
            throw new RpcException(status);
        }
    }
}
