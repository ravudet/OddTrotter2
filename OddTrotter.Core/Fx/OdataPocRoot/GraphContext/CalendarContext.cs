namespace Fx.OdataPocRoot.GraphContext
{
    using System;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    using Fx.OdataPocRoot.Graph;
    using Fx.OdataPocRoot.Odata;

    public sealed class CalendarContext : IInstanceContext<Calendar>
    {
        public Task<Calendar> Evaluate()
        {
            throw new System.NotImplementedException();
        }

        public IInstanceContext<Calendar> Select<TProperty>(Expression<Func<Calendar, TProperty>> selector)
        {
            throw new System.NotImplementedException();
        }
    }
}
