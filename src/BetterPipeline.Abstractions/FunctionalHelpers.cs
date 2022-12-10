
using BetterPipeline.Abstractions.Orchestration;

namespace BetterPipeline.Abstractions
{

    public static class MonadsExtensions
    {
        public static bool IsSome<T>(this Maybe<T> @this)
            => @this switch
            {
                Some<T> v => true,
                _ => false
            };

        public static bool IsNone<T>(this Maybe<T> @this)
            => @this switch
            {
                None<T> v => true,
                _ => false
            };

        public static bool IsProblem<T>(this Maybe<T> @this)
          => @this switch
          {
              Problem<T> v => true,
              _ => false
          };
        public static Maybe<T> ToMaybe<T>(this T @this)
            => @this switch
            {
                null => new None<T>(),
                _ => new Some<T>(@this)
            };

        public static Maybe<T> Mutate<T>(this Maybe<T> @this, Action<T> mutator) =>
            @this switch
            {
                Some<T> sth when
                !typeof(T).IsValueType &&
                !EqualityComparer<T>.Default.Equals(sth.Value, default(T))
                => Apply(sth, mutator),

                Some<T> sth when
                typeof(T).IsValueType => Apply(sth, mutator),

                Some<T> sth when
                EqualityComparer<T>.Default.Equals(sth.Value, default(T))
                    => new None<T>(),


                Problem<T> ex => new Problem<T>(ex.Exception),
                _ => new None<T>()
            };

        public static Maybe<TToType> Bind<TFromType, TToType>(this Maybe<TFromType> @this,
            Func<TFromType, TToType> f) => @this switch
            {
                Some<TFromType> sth when
                !typeof(TFromType).IsValueType &&
                !EqualityComparer<TFromType>.Default.Equals(sth.Value, default(TFromType))
                    => ExecuteFunc(f, sth),

                Some<TFromType> sth when
                typeof(TFromType).IsValueType => ExecuteFunc(f, sth),

                Some<TFromType> sth when
                EqualityComparer<TFromType>.Default.Equals(sth.Value, default(TFromType))
                    => new None<TToType>(),
                None<TFromType> _ => new None<TToType>(),
                Problem<TFromType> ex => new Problem<TToType>(ex.Exception),
                _ => new None<TToType>()
            };

        public static async Task<Maybe<TToType>> BindAsync<TFromType, TToType>(this Maybe<TFromType> @this,
           Func<TFromType, Task<TToType>> f) => @this switch
           {
               Some<TFromType> sth when
               !typeof(TFromType).IsValueType &&
               !EqualityComparer<TFromType>.Default.Equals(sth.Value, default(TFromType))
                   => await ExecuteFuncAsync(f, sth),

               Some<TFromType> sth when
               typeof(TFromType).IsValueType => await ExecuteFuncAsync(f, sth),

               Some<TFromType> sth when
               EqualityComparer<TFromType>.Default.Equals(sth.Value, default(TFromType))
                   => new None<TToType>(),

               None<TFromType> _ => new None<TToType>(),

               Problem<TFromType> ex => new Problem<TToType>(ex.Exception),

               _ => new None<TToType>()
           };


        public static async Task<Maybe<TToType>> BindAsync<TFromType, TToType>(this Maybe<TFromType> @this,
            IStep<TFromType, TToType> step,
            IExecutionContext executionContext,
            CancellationToken token = default) => @this switch
            {
                Some<TFromType> sth when
                !typeof(TFromType).IsValueType &&
                !EqualityComparer<TFromType>.Default.Equals(sth.Value, default(TFromType))
                    => await ExecuteStep(step, executionContext, token, sth),

                Some<TFromType> sth when
                typeof(TFromType).IsValueType => await ExecuteStep(step, executionContext, token, sth),

                Some<TFromType> sth when
                !typeof(TFromType).IsValueType &&
                EqualityComparer<TFromType>.Default.Equals(sth.Value, default(TFromType))
                   => new None<TToType>(),

                None<TFromType> _ => new None<TToType>(),

                Problem<TFromType> ex => new Problem<TToType>(ex.Exception),

                _ => new None<TToType>()
            };

        private static async Task<Maybe<TToType>> ExecuteStep<TFromType, TToType>(IStep<TFromType, TToType> step,
            IExecutionContext executionContext, CancellationToken token, Some<TFromType> sth)
        {
            try
            {
                return await step.Execute(sth, executionContext, token);
            }
            catch (Exception ex)
            {
                // executionContext?.LogError(ex, $"Error at step {step}");
                return new Problem<TToType>(new PipelineException($"Error at step {step}", ex));

            }
        }

        private static Maybe<T> Apply<T>(Some<T> sth, Action<T> mutator)
        {
            try
            {
                mutator(sth);
                return sth;
            }
            catch (Exception ex)
            {
                return new Problem<T>(new PipelineException($"Error at mutator {mutator}", ex));

            }
        }

        private static Maybe<TToType> ExecuteFunc<TFromType, TToType>(Func<TFromType, TToType> f, Some<TFromType> sth)
        {
            try
            {
                return f(sth).ToMaybe();
            }
            catch (Exception ex)
            {
                return new Problem<TToType>(new PipelineException($"Error at function {f}", ex));
            }
        }

        private static async Task<Maybe<TToType>> ExecuteFuncAsync<TFromType, TToType>(Func<TFromType, Task<TToType>> f, Some<TFromType> sth)
        {
            try
            {
                return (await f(sth)).ToMaybe();
            }
            catch (Exception ex)
            {
                return new Problem<TToType>(new PipelineException($"Error at function {f}", ex));
            }
        }

    }

}
