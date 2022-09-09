namespace BetterPipeline.Abstractions.Orchestration
{
    public class BranchingStep<TInput, TOutput> : IStep<TInput, TOutput>
    {
        public BranchingStep(Func<TInput, bool> predicate, IStep<TInput, TOutput> ifTrue, IStep<TInput, TOutput> ifFalse)
        {
            Predicate = predicate;
            IfTrue = ifTrue;
            IfFalse = ifFalse;
        }

        public Func<TInput, bool> Predicate { get; }
        public IStep<TInput, TOutput> IfTrue { get; }
        public IStep<TInput, TOutput> IfFalse { get; }

        public async Task<Maybe<TOutput>> Execute(Maybe<TInput> input, IExecutionContext context, CancellationToken token = default)
        {
            return Predicate(input) ? 
                await input.BindAsync(IfTrue, context, token) 
                : await input.BindAsync(IfFalse, context, token);
        }
    }
}
