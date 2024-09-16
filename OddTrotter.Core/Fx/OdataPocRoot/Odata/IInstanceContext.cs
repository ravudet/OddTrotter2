namespace Fx.OdataPocRoot.Odata
{
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System;
    using System.Threading.Tasks;
    using Fx.OdataPocRoot.Graph;

    public interface IInstanceContext<T>
    {
        Task<T> Evaluate(); //// TODO use 09/12 whiteboard photo for evaluate method
        //// TODO do a uri comparer for tests

        IInstanceContext<T> Select<TProperty>(Expression<Func<T, TProperty>> selector); //// TODO should these query option methods return a strong type?

        IInstanceContext<TProperty> SubContext<TProperty>(Expression<Func<T, OdataInstanceProperty<TProperty>>> selector); //// TODO better name

        ICollectionContext<TProperty> SubContext<TProperty>(Expression<Func<T, OdataCollectionProperty<TProperty>>> selector); //// TODO better name
    }
}
