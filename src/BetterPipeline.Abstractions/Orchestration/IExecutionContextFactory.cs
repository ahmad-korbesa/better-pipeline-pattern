using Microsoft.Extensions.Logging;

namespace BetterPipeline.Abstractions
{
    public interface IExecutionContextFactory
    {
        IExecutionContext CreateRequestExecutionContext<TRequest>(TRequest request, ILogger logger) where TRequest : class;
    }
}