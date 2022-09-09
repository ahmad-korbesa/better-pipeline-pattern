using Microsoft.Extensions.Logging;

namespace BetterPipeline.Abstractions
{
    public class ExecutionContextFactory : IExecutionContextFactory
    {
        IExecutionContext IExecutionContextFactory.CreateRequestExecutionContext<TRequest>
            (TRequest request, ILogger logger)
        {
            return new DefaultExecutionContext(request, logger);
        }
    }
}
