namespace Fx.OdataPocRoot.Odata
{
    using System;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    //// parse uri and check its parts in tests
    //// have an "evaluate" method on the contexts?
    //// transition calendarcontext and its related types to fx.odatapocroot

    public interface ICollectionContext<T>
    {
        Task<OdataCollection<T>> Evaluate();

        ICollectionContext<T> Select<TProperty>(Expression<Func<T, TProperty>> selector);

        ICollectionContext<T> Top(int count);

        ICollectionContext<T> OrderBy<TProperty>(Expression<Func<T, TProperty>> selector);

        ICollectionContext<T> Filter(Expression<Func<T, bool>> predicate);

        //// TODO have an indexer; somehow get an instancecontext from it
    }
}
