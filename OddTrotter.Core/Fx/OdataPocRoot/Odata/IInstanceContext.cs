namespace Fx.OdataPocRoot.Odata
{
    using System.Linq.Expressions;
    using System;
    using System.Threading.Tasks;

    public interface IInstanceContext<T>
    {
        Task<T> Evaluate(); //// TODO use 09/12 whiteboard photo for evaluate method

        IInstanceContext<T> Select<TProperty>(Expression<Func<T, TProperty>> selector);
    }
}
