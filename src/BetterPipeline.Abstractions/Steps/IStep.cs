namespace BetterPipeline.Abstractions
{
    public interface IStep
    {

    }

    public interface IStep<TInput, TOutput> : IStep
    {

        Task<Maybe<TOutput>> Execute(Maybe<TInput> input, IExecutionContext context, CancellationToken token = default);
    }

}
