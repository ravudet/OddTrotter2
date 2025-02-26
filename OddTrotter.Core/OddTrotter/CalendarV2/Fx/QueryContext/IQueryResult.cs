namespace Fx.QueryContext
{
    public interface IQueryResult<out TValue, out TError>
    {
        IQueryResultNode<TValue, TError> Nodes { get; }
    }
}
