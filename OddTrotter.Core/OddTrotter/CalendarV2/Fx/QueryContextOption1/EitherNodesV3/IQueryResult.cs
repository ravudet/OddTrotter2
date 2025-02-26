namespace Fx.QueryContextOption1.EitherNodesV3
{
    public interface IQueryResult<out TValue, out TError>
    {
        IQueryResultNode<TValue, TError> Nodes { get; }
    }
}
