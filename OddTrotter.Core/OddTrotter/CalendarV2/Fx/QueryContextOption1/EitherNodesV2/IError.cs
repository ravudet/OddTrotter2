namespace Fx.QueryContextOption1.EitherNodesV2
{
    public interface IError<out TError>
    {
        TError Value { get; }
    }
}
