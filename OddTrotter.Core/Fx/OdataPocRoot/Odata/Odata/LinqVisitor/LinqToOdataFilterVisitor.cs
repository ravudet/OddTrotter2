////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Fx.OdataPocRoot.Odata.Odata.LinqVisitor
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;
    using Fx.OdataPocRoot.Odata.UriExpressionNodes.Filter;

    public static class CommonMethods
    {
        public static class String
        {
            public static MethodInfo EndsWith { get; }

            static String()
            {
                EndsWith = ((BinaryExpression)((Expression<Func<string, bool>>)(test => test.EndsWith(string.Empty))).Body).Method!;
            }
        }
    }

    public sealed class LinqToOdataFilterVisitor : ExpressionVisitor<Filter, Void>
    {
        protected override Filter Visit(BinaryExpression node, Void context)
        {
            //// TODO does it make sense to treat this as "this method converts a 'binaryexpression' to a 'filter'" and then have another visitor for "this method converts a 'binaryexpression' to a 'boolcommonexpression'" and another visitor for "this method converts a 'binaryexpression' to a 'boolmethodcallexpression'" and so on?
            if (node.Type != typeof(bool))
            {
                throw new System.Exception($"It is not meaningful to convert the following to an OData 'Filter' expression: a .NET '{node.GetType().Name}' with method '{node.Method?.Name}' and node type '{node.NodeType}' that returns '{node.Type.Name}'");
            }

            BoolCommonExpression boolCommonExpression;
            if (node.Method == CommonMethods.String.EndsWith)
            {
                boolCommonExpression = new BoolCommonExpression.Eighth(null!);
            }
            else
            {
                throw new System.Exception("TODO can't convert");
            }

            return new Filter(boolCommonExpression);
        }

        protected override Filter Visit(BlockExpression node, Void context)
        {
            throw new System.NotImplementedException();
        }

        protected override Filter Visit(ConditionalExpression node, Void context)
        {
            throw new System.NotImplementedException();
        }

        protected override Filter Visit(ConstantExpression node, Void context)
        {
            throw new System.NotImplementedException();
        }

        protected override Filter Visit(DebugInfoExpression node, Void context)
        {
            throw new System.NotImplementedException();
        }

        protected override Filter Visit(DefaultExpression node, Void context)
        {
            throw new System.NotImplementedException();
        }

        protected override Filter Visit(DynamicExpression node, Void context)
        {
            throw new System.NotImplementedException();
        }

        protected override Filter Visit(GotoExpression node, Void context)
        {
            throw new System.NotImplementedException();
        }

        protected override Filter Visit(IndexExpression node, Void context)
        {
            throw new System.NotImplementedException();
        }

        protected override Filter Visit(InvocationExpression node, Void context)
        {
            throw new System.NotImplementedException();
        }

        protected override Filter Visit(LabelExpression node, Void context)
        {
            throw new System.NotImplementedException();
        }

        protected override Filter Visit(LambdaExpression node, Void context)
        {
            //// TODO how to recognize if this is the entry point lambda versus a lambda defined in a expression?
            return this.Dispatch(node.Body, context);
        }

        protected override Filter Visit(ListInitExpression node, Void context)
        {
            throw new System.NotImplementedException();
        }

        protected override Filter Visit(LoopExpression node, Void context)
        {
            throw new System.NotImplementedException();
        }

        protected override Filter Visit(MemberExpression node, Void context)
        {
            throw new System.NotImplementedException();
        }

        protected override Filter Visit(MemberInitExpression node, Void context)
        {
            throw new System.NotImplementedException();
        }

        protected override Filter Visit(MethodCallExpression node, Void context)
        {
            throw new System.NotImplementedException();
        }

        protected override Filter Visit(NewArrayExpression node, Void context)
        {
            throw new System.NotImplementedException();
        }

        protected override Filter Visit(NewExpression node, Void context)
        {
            throw new System.NotImplementedException();
        }

        protected override Filter Visit(ParameterExpression node, Void context)
        {
            throw new System.NotImplementedException();
        }

        protected override Filter Visit(RuntimeVariablesExpression node, Void context)
        {
            throw new System.NotImplementedException();
        }

        protected override Filter Visit(SwitchExpression node, Void context)
        {
            throw new System.NotImplementedException();
        }

        protected override Filter Visit(TryExpression node, Void context)
        {
            throw new System.NotImplementedException();
        }

        protected override Filter Visit(TypeBinaryExpression node, Void context)
        {
            throw new System.NotImplementedException();
        }

        protected override Filter Visit(UnaryExpression node, Void context)
        {
            throw new System.NotImplementedException();
        }
    }
}
