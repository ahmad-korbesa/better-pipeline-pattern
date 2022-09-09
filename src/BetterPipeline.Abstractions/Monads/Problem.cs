using System;

namespace BetterPipeline.Abstractions
{
    public class Problem<T> : Maybe<T>
    {
        public Exception Exception { get; }

        public override T? Value => default;

        public Problem(Exception ex)
        {
            Exception = ex;
        }
        public override string ToString()
        {
            return $"Problem: {typeof(T)} {Exception.Message}";
        }
    }

}
