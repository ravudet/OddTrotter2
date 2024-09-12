namespace Fx.OdataPocRoot.Odata
{
    using System.Linq.Expressions;
    using System;
    using System.Threading.Tasks;

    public interface IInstanceContext<T>
    {
        Task<T> Evaluate();

        IInstanceContext<T> Select<TProperty>(Expression<Func<T, TProperty>> selector);
    }
}
