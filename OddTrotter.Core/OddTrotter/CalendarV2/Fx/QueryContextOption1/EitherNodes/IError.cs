namespace Fx.QueryContextOption1.EitherNodes
{
    public interface IError<out TError>
    {
        TError Value { get; }
    }
}
