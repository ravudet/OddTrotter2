namespace Fx.OdataPocRoot.Odata
{
    using Fx.OdataPocRoot.Odata.UriExpressionNodes.Select;

    public abstract class FilterExpressionNode
    {
    }

    /// <summary>
    /// TODO move stuff out of here if it's not actually shared across different expression types
    /// </summary>
    public abstract class OdataUriNode //// TODO is there a reason to use inheritance here? maybe just use a static class instead?
    {
        public sealed class QualifiedActionName : OdataUriNode
        {
        }

        public sealed class QualifiedFunctionName : OdataUriNode
        {
        }

    }
}
