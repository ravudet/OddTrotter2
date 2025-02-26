namespace Fx.QueryContextOption1.EitherNodes
{
    public interface IElement<out TValue, out TError> : IQueryResultNode<TValue, TError>
    {
        TValue Value { get; }

        IQueryResultNode<TValue, TError> Next();
    }
}
