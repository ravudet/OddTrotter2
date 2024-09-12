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
            public PrimitiveProperty(PrimitiveProperty property)
            {
                Property = property;
            }

            public PrimitiveProperty Property { get; }
        }

        public sealed class PrimitiveCollectionProperty : SelectProperty
        {
            public PrimitiveCollectionProperty(OdataIdentifier property, IEnumerable<SelectOptionPc> nestedSelects)
            {
                Property = property;
                NestedSelects = nestedSelects; //// TODO i don't like this name
            }

            public OdataIdentifier Property { get; }

            public IEnumerable<SelectOptionPc> NestedSelects { get; }
        }

        public sealed class NavigationProperty : SelectProperty
        {
            public NavigationProperty(NavigationProperty property)
            {
                Property = property;
            }

            public NavigationProperty Property { get; }
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
                public SelectOption(SelectPath selectPath, IEnumerable<SelectOption> selectOptions)
                    : base(selectPath)
                {
                    SelectOptions = selectOptions;
                }

                public IEnumerable<SelectOption> SelectOptions { get; }
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
