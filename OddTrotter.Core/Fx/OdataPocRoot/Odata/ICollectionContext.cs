namespace Fx.OdataPocRoot.Odata
{
    using System.Linq.Expressions;
    using System;

    public interface ICollectionContext<T>
    {
        OdataCollection<T> Values { get; }

        ICollectionContext<T> Select<TProperty>(Expression<Func<T, TProperty>> selector);

        ICollectionContext<T> Top(int count);

        ICollectionContext<T> OrderBy<TProperty>(Expression<Func<T, TProperty>> selector);

        ICollectionContext<T> Filter(Expression<Func<T, bool>> predicate);
    }
}
