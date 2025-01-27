namespace CalendarV2.Fx.QueryContext
{
    using global::System.Threading.Tasks;

    /// <summary>
    /// TODO covariance and contravariance
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <typeparam name="TError"></typeparam>
    public interface IQueryContext<TValue, TError>
    {
        Task<QueryResult<TValue, TError>> Evaluate();
    }
}
