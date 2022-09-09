namespace BetterPipeline.Abstractions.Orchestration
{
    public class GenericStep<TReq, TRes> : IStep<TReq, TRes>
    {
        private readonly Func<TReq?, IExecutionContext, CancellationToken, Task<Maybe<TRes>>>? funcWithCancellationToken;

        private readonly Func<TReq?, IExecutionContext, Task<Maybe<TRes>>> func;

        public GenericStep(Func<TReq?, IExecutionContext, Task<Maybe<TRes>>> func) => this.func = func;

        public GenericStep(Func<TReq?, IExecutionContext, CancellationToken, Task<Maybe<TRes>>> func)
        {
            funcWithCancellationToken = func;
            this.func = NoOpFunc;
        }

        //public static implicit operator T?(Maybe<T> @this) => @this.Value;

        //for implicit cast on input values/ assignment
        public static implicit operator GenericStep<TReq,TRes>(Func<TReq?, IExecutionContext, Task<Maybe<TRes>>> @this) => new GenericStep<TReq, TRes>(@this);

        public static implicit operator GenericStep<TReq, TRes>(Func<TReq?, IExecutionContext, CancellationToken, Task<Maybe<TRes>>> @this) => new GenericStep<TReq, TRes>(@this);


        private static Task<Maybe<TRes>> NoOpFunc(TReq? req, IExecutionContext ctx) =>
           Task.FromResult<Maybe<TRes>>(new None<TRes>());

        public async Task<Maybe<TRes>> Execute(Maybe<TReq> input, IExecutionContext context, CancellationToken token = default) =>
            funcWithCancellationToken is not null ? await funcWithCancellationToken(input, context, token) : await func(input, context);


        public override string ToString() =>
            funcWithCancellationToken is object ? $"generic: {funcWithCancellationToken}" : $"generic: {func}";
    }
}
