namespace BetterPipeline.Abstractions
{
    public class Some<T> : Maybe<T>
    {
        public override T Value { get; }
        public Some(T value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return $"Some: {typeof(T)} {Value}";
        }
    }

}
