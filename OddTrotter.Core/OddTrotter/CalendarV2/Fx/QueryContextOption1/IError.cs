namespace Fx.QueryContextOption1
{
    public interface IError<out TError>
    {
        TError Value { get; }
    }
}
