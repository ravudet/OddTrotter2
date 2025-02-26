namespace Fx.QueryContextOption1
{
    public interface IEmpty<out TValue, out TError> : IQueryResultNode<TValue, TError> //// TODO does all of the interface inheritance here lead to weird things like visiting an empty with a value?
    {
    }
}
