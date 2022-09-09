using BetterPipeline.Abstractions.Orchestration;
using System.Collections.Immutable;

namespace BetterPipeline.Abstractions
{
    public static class Step
    {
        public static IStep<TInput, TOutput> Create<TInput, TOutput>(Func<TInput?, IExecutionContext, Task<Maybe<TOutput>>> func)
            => new UnaryStep<TInput, TOutput>(new GenericStep<TInput, TOutput>(func));

        public static IStep<T, T> Create<T>(Func<T?, IExecutionContext, Task<Maybe<T>>> func)
           => new UnaryStep<T, T>(new GenericStep<T, T>(func));

        public static IStep<TInput, TOutput> Create<TInput, TOutput>(Func<TInput?, IExecutionContext, CancellationToken, Task<Maybe<TOutput>>> func)
          => new UnaryStep<TInput, TOutput>(new GenericStep<TInput, TOutput>(func));

        public static IStep<T, T> Create<T>(Func<T?, IExecutionContext, CancellationToken, Task<Maybe<T>>> func)
           => new UnaryStep<T, T>(new GenericStep<T, T>(func));

        public static FilterStep<T> Filter<T>(Func<T?, Task<bool>> predicate)
            => new FilterStep<T>(predicate);

        public static IStep<T, T> NoOp<T>() => new NoOpStep<T>();

        public static IStep<TInput, TOutput> InParallel<TInput, TOutput>(
            ICollection<IStep<TInput, TOutput>> actions,
            Func<ICollection<TOutput>, TOutput> mergeFunction)
            => new ParallelStep<TInput, TOutput>(actions, mergeFunction);
    }

    public static class StepsExtensions
    {
        public static IStep<TInput, TOutput> ThenMap<TInput, TTemp, TOutput>(this IStep<TInput, TTemp> @this,
            Func<TTemp, TOutput> map)
            => @this.Then(Step.Create<TTemp, TOutput>(async (input, ctx, token) => map(input)));

        public static IStep<TInput, TOutput> Then<TInput, TMid, TOutput>(this IStep<TInput, TMid> @this,
            IStep<TMid, TOutput> next)
            => (@this, next) switch
            {
                (NoOpStep<TInput> _, var second) when typeof(TInput).IsEquivalentTo(typeof(TMid))
                => (IStep<TInput, TOutput>)second,
                (var first, NoOpStep<TOutput> _) when typeof(TOutput).IsEquivalentTo(typeof(TMid))
                => (IStep<TInput, TOutput>)first,
                (var first, var second) => new SequentialStep<TInput, TMid, TOutput>(first, second)
            };
        //            => new SequentialStep<TInput, TMid, TOutput>(@this, next);

        public static IStep<TInput, TOutput> AsUnary<TInput, TOutput>(this IStep<TInput, TOutput> @this)
            => @this switch
            {
                UnaryStep<TInput, TOutput> u => u,
                var action => new UnaryStep<TInput, TOutput>(action)
            };

        public static BranchingStepBuilder<TInput, TOutput> When<TInput, TOutput>(this IStep<TInput, TOutput> @this,
            Func<TInput, bool> predicate)
            => new BranchingStepBuilder<TInput, TOutput>(@this, predicate);
    }

    public class BranchingStepBuilder<TInput, TOutput>
    {
        public BranchingStepBuilder(IStep<TInput, TOutput> IfTrue, Func<TInput, bool> Predicate)
        {
            this.IfTrue = IfTrue;
            this.Predicate = Predicate;
        }
        public IStep<TInput, TOutput> Otherwise(IStep<TInput, TOutput> IfFalse)
        => new BranchingStep<TInput, TOutput>(Predicate, IfTrue, IfFalse);

        public Func<TInput, bool> Predicate { get; }

        public IStep<TInput, TOutput> IfTrue { get; }
    }

}
