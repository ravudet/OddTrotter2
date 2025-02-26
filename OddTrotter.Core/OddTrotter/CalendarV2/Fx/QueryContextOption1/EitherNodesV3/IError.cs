namespace Fx.QueryContextOption1.EitherNodesV3
{
    public interface IError<out TError>
    {
        TError Value { get; }
    }
}
