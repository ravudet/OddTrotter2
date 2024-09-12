
using Fx.OdataPocRoot.Odata.UriExpressionNodes.Common;

namespace Fx.OdataPocRoot.Odata.UriExpressionNodes.Select
{
    public abstract class SelectOption
    {
        private SelectOption()
        {
        }

        public sealed class SelectOptionPcNode : SelectOption
        {
            public SelectOptionPcNode(SelectOptionPc selectOptionPc)
            {
                SelectOptionPc = selectOptionPc;
            }

            public SelectOptionPc SelectOptionPc { get; }
        }

        public sealed class ComputeNode : SelectOption
        {
            public ComputeNode(Fx.OdataPocRoot.Odata.UriExpressionNodes.Compute.Compute compute)
            {
                Compute = compute;
            }

            public Fx.OdataPocRoot.Odata.UriExpressionNodes.Compute.Compute Compute { get; }
        }

        public sealed class SelectNode : SelectOption
        {
            public SelectNode(Fx.OdataPocRoot.Odata.UriExpressionNodes.Select.Select select)
            {
                Select = select;
            }

            public UriExpressionNodes.Select.Select Select { get; }
        }

        public sealed class ExpandNode : SelectOption
        {
            public ExpandNode(Fx.OdataPocRoot.Odata.UriExpressionNodes.Expand.Expand expand)
            {
                Expand = expand;
            }

            public Expand.Expand Expand { get; }
        }

        public sealed class AliasAndValueNode : SelectOption
        {
            public AliasAndValueNode(AliasAndValue aliasAndValue)
            {
                AliasAndValue = aliasAndValue;
            }

            public AliasAndValue AliasAndValue { get; }
        }
    }
}
