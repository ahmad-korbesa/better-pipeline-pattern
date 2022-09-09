using System.Threading.Tasks;
using System.Threading;

namespace BetterPipeline.Abstractions.Orchestration
{
    public class SequentialStep<TReq, TResInner, TRes> : IStep<TReq, TRes>
    {
     
        public SequentialStep(IStep<TReq, TResInner> first, IStep<TResInner, TRes> second)
        {
            First = first ?? throw new System.ArgumentNullException(nameof(first));
            Second = second ?? throw new System.ArgumentNullException(nameof(second));
        }

        public IStep<TReq, TResInner> First { get; set; }

        public IStep<TResInner, TRes> Second { get; set; }

        public async Task<Maybe<TRes>> Execute(Maybe<TReq> input, IExecutionContext context,
            CancellationToken token = default)
        {
            return await (await input.BindAsync(First, context, token))
                .BindAsync(Second, context, token);
        }
        public override string ToString()
        {
            return $"Sequencial {First} -> {Second}";
        }
    }


}
