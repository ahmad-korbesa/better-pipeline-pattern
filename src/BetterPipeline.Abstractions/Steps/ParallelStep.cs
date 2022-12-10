using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.Threading;

namespace BetterPipeline.Abstractions.Orchestration
{
    public class ParallelStep<TReq, TRes> : IStep<TReq, TRes>
    {

        public ParallelStep(ICollection<IStep<TReq, TRes>> actions,
            Func<ICollection<TRes>, TRes> mergeFunction)
        {
            SubSteps = actions;
            MergeFunction = mergeFunction;
        }


        public ICollection<IStep<TReq, TRes>> SubSteps { get; set; }

        public Func<ICollection<TRes>, TRes> MergeFunction { get; }

        public async Task<Maybe<TRes>> Execute(Maybe<TReq> input, IExecutionContext context, CancellationToken token = default)
        {
            var tasks = SubSteps.Select(async action =>
            {
                return await input.BindAsync(action, context, token);
            }).ToArray();

            var results = await Task.WhenAll(tasks);

            //if (context is object && context.HasLoggingCapability)
            //{
            //    var problems = results
            //        .OfType<Problem<TRes>>()
            //        .Select((item, index) => (item, index))
            //        .ToList();

            //    var noneValues = results
            //        .OfType<None<TRes>>()
            //        .Select((item, index) => (item, index))
            //        .ToList();
            //    foreach (var nonVal in noneValues)
            //    {
            //        context.LogWarning($"none value at {SubSteps.ElementAt(nonVal.index)}");
            //    }
            //}

            var parallelResults = results.OfType<Some<TRes>>()
                .Select(x => x.Value)
                .ToList();

            // context?.LogInformation($"sub steps with some result {parallelResults.Count}/{SubSteps.Count}");

            var faultyResults = results.OfType<Problem<TRes>>()
                .Select((item, index) => (item, index))
                .ToList();
            if (faultyResults.Count > 0)
            {
                // context?.LogInformation($"sub steps with exception result {faultyResults.Count}/{SubSteps.Count}");
                foreach (var problem in faultyResults)
                {
                    // context?.LogError(problem.item.Exception, $"Error at step {SubSteps.ElementAt(problem.index)}");
                }
            }

            var noneResults = results.OfType<None<TRes>>()
                .Select((item, index) => (item, index))
                .ToList();
            if (noneResults.Count > 0)
            {
                // context?.LogInformation($"sub steps with no result {noneResults.Count}/{SubSteps.Count}");
                foreach (var nonVal in noneResults)
                {
                    // context?.LogWarning($"none value at step {SubSteps.ElementAt(nonVal.index)}");
                }
            }

            if (parallelResults.Count > 0)
            {
                var mergeResult = MergeFunction(parallelResults);
                return mergeResult;
            }
            else if (parallelResults.Count == 0 && SubSteps.Count == 0)
            {
                return new None<TRes>();
            }
            else
            {
                return faultyResults.Count == SubSteps.Count ?
                     new Problem<TRes>(faultyResults.First().item.Exception) : // if all results have exceptions
                     new None<TRes>(); //then there are some none there
            }

        }


        public override string ToString()
        {
            return $"Parallel: {SubSteps.Count} tasks";
        }
    }


}
