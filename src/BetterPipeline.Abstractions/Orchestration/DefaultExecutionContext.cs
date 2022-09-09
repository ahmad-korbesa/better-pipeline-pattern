using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace BetterPipeline.Abstractions
{
    public class DefaultExecutionContext : IExecutionContext
    {
        private readonly ConcurrentDictionary<string, object> contextVariables;
        private readonly object input;
        private readonly ILogger logger;

        public DefaultExecutionContext(object input, ILogger logger)
        {
            contextVariables = new ConcurrentDictionary<string, object>();
            this.input = input;
            this.logger = logger;
        }

        internal DefaultExecutionContext(object input, ILogger logger,
            Dictionary<string, object> contextVariables)
        {
            this.contextVariables = new ConcurrentDictionary<string, object>();
            
            foreach (var item in contextVariables)
            {
                this.contextVariables.TryAdd(item.Key, item.Value);
            }

            this.input = input;
            this.logger = logger;
        }
        public Task AddContextVariable(string name, object value)
            => Task.FromResult(contextVariables.AddOrUpdate(name, value, (key, val) => value));

        public IExecutionContext Clone()
        {
            return new DefaultExecutionContext(this.input, this.logger,
                this.contextVariables.Select(kvp =>
                 new KeyValuePair<string, object>(kvp.Key, kvp.Value))
                .ToDictionary(v => v.Key, v => v.Value)
                );
        }

        public Task<object?> GetContextVariable(string key)
           => Task.FromResult(contextVariables.GetValueOrDefault(key));

        public T? GetPipelineInput<T>()
        {
            return (T)this.input;
        }

        public void LogError(Exception ex, string message)
            => logger?.LogError(ex,message);

        public void LogInformation(string message) => logger?.LogInformation(message);

        public void LogWarning(string message) => logger?.LogWarning(message);
    }
}
