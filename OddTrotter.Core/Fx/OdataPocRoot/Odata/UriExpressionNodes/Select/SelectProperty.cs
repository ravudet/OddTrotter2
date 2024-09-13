namespace Fx.OdataPocRoot.Odata.UriExpressionNodes.Select
{
    using System.Collections.Generic;

    using Fx.OdataPocRoot.Odata.UriExpressionNodes.Common;

    public abstract class SelectProperty
    {
        private SelectProperty()
        {
        }

        public sealed class PrimitiveProperty : SelectProperty
        {
            public PrimitiveProperty(Fx.OdataPocRoot.Odata.UriExpressionNodes.Common.PrimitiveProperty property)
            {
                Property = property;
            }

            public Fx.OdataPocRoot.Odata.UriExpressionNodes.Common.PrimitiveProperty Property { get; }
        }

        public sealed class PrimitiveCollectionProperty : SelectProperty
        {
            public PrimitiveCollectionProperty(OdataIdentifier property, IEnumerable<SelectOptionPc> nestedOptions)
            {
                Property = property;
                NestedOptions = nestedOptions; //// TODO i don't like this name
            }

            public OdataIdentifier Property { get; }

            public IEnumerable<SelectOptionPc> NestedOptions { get; }
        }

        public sealed class NavigationProperty : SelectProperty
        {
            public NavigationProperty(Fx.OdataPocRoot.Odata.UriExpressionNodes.Common.NavigationProperty property)
            {
                Property = property;
            }

            public Fx.OdataPocRoot.Odata.UriExpressionNodes.Common.NavigationProperty Property { get; }
        }

        public abstract class FullSelectPath : SelectProperty
        {
            private FullSelectPath(SelectPath selectPath)
            {
                SelectPath = selectPath;
            }

            public SelectPath SelectPath { get; }

            public sealed class SelectOption : FullSelectPath
            {
                public SelectOption(SelectPath selectPath, IEnumerable<Fx.OdataPocRoot.Odata.UriExpressionNodes.Select.SelectOption> selectOptions)
                    : base(selectPath)
                {
                    SelectOptions = selectOptions;
                }

                public IEnumerable<Fx.OdataPocRoot.Odata.UriExpressionNodes.Select.SelectOption> SelectOptions { get; }
            }

            public sealed class SelectPropertyNode : FullSelectPath
            {
                public SelectPropertyNode(SelectPath selectPath, SelectProperty selectPropertyNode)
                    : base(selectPath)
                {
                    SelectProperty = selectPropertyNode;
                }

                public SelectProperty SelectProperty { get; }
            }
        }
    }
}
