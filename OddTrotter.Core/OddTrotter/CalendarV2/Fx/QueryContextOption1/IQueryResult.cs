namespace Fx.QueryContextOption1
{
    /// <summary>
    /// TODO covariance and contravariance if possible
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <typeparam name="TError"></typeparam>
    public interface IQueryResult<TValue, TError>
    {
        QueryResultNode<TValue, TError> Nodes { get; }
    }
}
