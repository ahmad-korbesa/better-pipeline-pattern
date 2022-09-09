namespace BetterPipeline.Abstractions
{
    public class None<T> : Maybe<T>
    {
        public override T? Value => default;
        public override string ToString()
        {
            return $"None: {typeof(T)} {Value}";
        }
    }

}
