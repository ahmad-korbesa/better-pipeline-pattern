namespace BetterPipeline.Abstractions.Orchestration
{
    public class UnaryStep<TReq, TRes> : IStep<TReq, TRes>
    {
        public UnaryStep(IStep<TReq, TRes> action)
        {
            Action = action;
        }

        public IStep<TReq, TRes> Action { get; }

        public Task<Maybe<TRes>> Execute(Maybe<TReq> input, IExecutionContext context, CancellationToken token = default)
        {
            return input.BindAsync(Action, context, token);
        }
        public override string ToString()
        {
            return $"Unary {Action}";
        }
    }


}
