namespace Fx.OdataPocRoot.Odata.UriExpressionVisitorImplementations //// TODO better name
{
    using Fx.OdataPocRoot.Odata.UriExpressionNodes.Common;
    using Fx.OdataPocRoot.Odata.UriExpressionNodes.Select;
    using Fx.OdataPocRoot.Odata.UriExpressionVisitors;
    using System.Linq;
    using System.Text;
    using System.Threading;

    public sealed class SelectToStringVisitor
    {
        private readonly CommonToStringVisitor commonToStringVisitor = CommonToStringVisitor.Default;

        public void Visit(Select node, StringBuilder builder)
        {
            //// TODO should the node have a readonlylist instead of an ienumerable for selectitems?
            var selectItems = node.SelectItems.ToList();
            if (selectItems.Count == 0)
            {
                //// TODO is this actually legal? model it somehow if it's not
                return;
            }

            Visit(selectItems[0], builder);
            for (int i = 1; i < selectItems.Count; ++i)
            {
                builder.Append(",");
                Visit(selectItems[i], builder);
            }
        }

        public void Visit(SelectItem node, StringBuilder builder)
        {
            //// TODO have individual visit methods?
            if (node is SelectItem.Star star)
            {
                builder.Append("*");
            }
            else if (node is SelectItem.AllOperationsInSchema allOperationsInSchema)
            {
                Visit(allOperationsInSchema, builder);
                builder.Append(".*");
            }
            else if (node is SelectItem.PropertyPath propertyPath)
            {
            }
        }

        private sealed class CommonToStringVisitor
        {
            private CommonToStringVisitor()
            {
            }

            public static CommonToStringVisitor Default { get; } = new CommonToStringVisitor();

            public void Visit(Namespace node, StringBuilder builder)
            {
                //// TODO should the node have a readonlylist of namespace parts?
                var namespaceParts = node.NamespaceParts.ToList();
                if (namespaceParts.Count == 0)
                {
                    //// TODO is this actually legal? model it somehow if it's not
                    return;
                }

                Visit(namespaceParts[0], builder);
                for (int i = 1; i < namespaceParts.Count; ++i)
                {
                    builder.Append(".");
                    Visit(namespaceParts[i], builder);
                }
            }

            public void Visit(OdataIdentifier node, StringBuilder builder)
            {
                builder.Append(node.Identifier); //// TODO is there really nothign els eyou need?
            }
        }
    }
}
