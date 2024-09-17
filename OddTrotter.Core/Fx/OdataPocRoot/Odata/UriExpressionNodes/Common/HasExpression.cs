////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Fx.OdataPocRoot.Odata.UriExpressionNodes.Common
{
    public sealed class HasExpression
    {
        public HasExpression(Enum @enum)
        {
            Enum = @enum;
        }

        public Enum Enum { get; }
    }
}
