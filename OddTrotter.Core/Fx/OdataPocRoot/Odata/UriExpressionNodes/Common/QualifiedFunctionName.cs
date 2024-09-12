namespace Fx.OdataPocRoot.Odata.UriExpressionNodes.Common
{
    using System.Collections.Generic;

    public sealed class QualifiedFunctionName
    {
        public QualifiedFunctionName(Namespace @namespace, OdataIdentifier function, IEnumerable<OdataIdentifier> parameterNames)
        {
            Namespace = @namespace;
            Function = function;
            ParameterNames = parameterNames;
        }

        public Namespace Namespace { get; }

        public OdataIdentifier Function { get; }

        public IEnumerable<OdataIdentifier> ParameterNames { get; }
    }
}
