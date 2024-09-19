////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Fx.OdataPocRoot.Odata.Odata.RequestEvaluator
{
    using Fx.OdataPocRoot.Odata.UriExpressionVisitorImplementations;

    public sealed class RequestEvaluatorSettings
    {
        private RequestEvaluatorSettings(
            FilterToStringVisitor filterToStringVisitor,
            SelectToStringVisitor selectToStringVisitor)
        {
            FilterToStringVisitor = filterToStringVisitor;
            SelectToStringVisitor = selectToStringVisitor;
        }

        public static RequestEvaluatorSettings Default { get; } = new RequestEvaluatorSettings(
            new FilterToStringVisitor(CommonToStringVisitor.Default),
            new SelectToStringVisitor());

        public FilterToStringVisitor FilterToStringVisitor { get; }

        public SelectToStringVisitor SelectToStringVisitor { get; }

        //// TODO implement builder
    }
}
