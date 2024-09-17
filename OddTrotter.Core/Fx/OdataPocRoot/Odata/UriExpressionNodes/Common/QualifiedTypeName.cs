////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Fx.OdataPocRoot.Odata.UriExpressionNodes.Common
{
    public abstract class QualifiedTypeName
    {
        private QualifiedTypeName()
        {
        }

        public sealed class SingleValue : QualifiedTypeName
        {
            public SingleValue(SingleQualifiedTypeName singleQualifiedTypeName)
            {
                SingleQualifiedTypeName = singleQualifiedTypeName;
            }

            public SingleQualifiedTypeName SingleQualifiedTypeName { get; }
        }

        public sealed class MultiValue : QualifiedTypeName
        {
            public MultiValue(SingleQualifiedTypeName singleQualifiedTypeName)
            {
                SingleQualifiedTypeName = singleQualifiedTypeName;
            }

            public SingleQualifiedTypeName SingleQualifiedTypeName { get; }
        }
    }
}
