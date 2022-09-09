namespace BetterPipeline.Abstractions.Orchestration
{
    public class NoOpStep<T> : IStep<T, T>
    {
        public async Task<Maybe<T>> Execute(Maybe<T> input, IExecutionContext context, CancellationToken token = default)
        {
            return input;
        }

        public override string ToString()
        {
            return $"No Op";
        }
    }
}
