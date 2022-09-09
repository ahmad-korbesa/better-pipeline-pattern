namespace BetterPipeline.Abstractions;

public abstract class Maybe<T>
{
    public abstract T? Value { get; }

    //implicit cast on output/ returns
    public static implicit operator T?(Maybe<T> @this) => @this.Value;

    //for implicit cast on input values/ assignment
    public static implicit operator Maybe<T>(T @this) => @this.ToMaybe();
}

