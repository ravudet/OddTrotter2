namespace Fx.QueryContextOption1
{
    public interface IQueryResult<out TValue, out TError>
    {
        IQueryResultNode<TValue, TError> Nodes { get; }
    }
}
