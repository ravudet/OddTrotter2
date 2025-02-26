namespace Fx.QueryContextOption1
{
    public interface IError<out TValue, out TError> : ITerminal<TValue, TError>
    {
        TError Value { get; }
    }
}
