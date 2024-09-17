////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Fx.OdataPocRoot.Odata.UriExpressionNodes.Common
{
    public abstract class ImplicitVariableExpression
    {
        private ImplicitVariableExpression()
        {
        }

        public sealed class It : ImplicitVariableExpression
        {
            public It()
            {
                //// TODO singletons?
            }
        }

        public sealed class This : ImplicitVariableExpression
        {
            public This()
            {
            }
        }
    }
}
