namespace BetterPipeline.Abstractions
{
    public interface IExecutionContext
    {
        IExecutionContext Clone();

        T? GetPipelineInput<T>() ;

        Task AddContextVariable(string name, object value);

        Task<object?> GetContextVariable(string key);
        void LogError(Exception ex, string message);
    }
}