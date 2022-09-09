namespace BetterPipeline.Abstractions.Orchestration
{
    public class FilterStep<T> : IStep<T, T>
    {
        public FilterStep(Func<T?, Task<bool>> predicate)
        {
            Predicate = predicate;
        }

        public Func<T?, Task<bool>> Predicate { get; }


        public async Task<Maybe<T>> Execute(Maybe<T> input, IExecutionContext context, CancellationToken token = default)
        {
            try
            {
                if (await Predicate(input))
                {
                    return input;
                }
                else
                {
                    return new None<T>();
                }
            }
            catch (Exception ex)
            {
                return new Problem<T>(ex);
            }
        }

        public override string ToString()
        {
            return $"filter: {Predicate}";
        }
    }
}
