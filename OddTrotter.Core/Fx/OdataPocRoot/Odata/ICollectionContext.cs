namespace Fx.OdataPocRoot.Odata
{
    using System.Linq.Expressions;
    using System;

    //// TODO update todolistservice to use calendarcontext
    //// TODO update calendarservice to use calendarcontext
    //// parse uri and check its parts in tests
    //// have an "evaluate" method on the contexts?
    //// transition calendarcontext and its related types to fx.odatapocroot

    public interface ICollectionContext<T>
    {
        OdataCollection<T> Values { get; }

        ICollectionContext<T> Select<TProperty>(Expression<Func<T, TProperty>> selector);

        ICollectionContext<T> Top(int count);

        ICollectionContext<T> OrderBy<TProperty>(Expression<Func<T, TProperty>> selector);

        ICollectionContext<T> Filter(Expression<Func<T, bool>> predicate);
    }
}
