////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Fx.OdataPocRoot.Odata.Odata.LinqVisitor
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;
    using Fx.OdataPocRoot.Odata.UriExpressionNodes.Common;
    using Fx.OdataPocRoot.Odata.UriExpressionNodes.Filter;

    public static class CommonMethods
    {
        public static class Object
        {
            public static new MethodInfo GetType { get; }

            static Object()
            {
                Expression<Func<object, System.Type>> getTypeExpression = (_) => _.GetType();
                var getTypeMethodCallExpression = (MethodCallExpression)getTypeExpression.Body;
                GetType = getTypeMethodCallExpression.Method;
            }
        }

        public static class Type
        {
            public static new MethodInfo Equals { get; }

            static Type()
            {
                Expression<Func<bool>> equalsExpression = () => typeof(object) == typeof(object);
                var equalsBinaryExpression = (BinaryExpression)equalsExpression.Body;
                Equals = equalsBinaryExpression.Method!;
            }
        }
    }

    public sealed class TypeToSingleQualifiedTypeNameVisitor
    {
        public SingleQualifiedTypeName Dispatch(Type type)
        {
            throw new Exception("tODO");
        }
    }

    public sealed class LinqToQualifiedTypeNameVisitor : ExpressionVisitor<QualifiedTypeName, Void>
    {
        private readonly TypeToSingleQualifiedTypeNameVisitor typeToSingleQualifiedTypeNameVisitor;

        public LinqToQualifiedTypeNameVisitor()
        {
            this.typeToSingleQualifiedTypeNameVisitor = new TypeToSingleQualifiedTypeNameVisitor();
        }

        protected override QualifiedTypeName Visit(BinaryExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override QualifiedTypeName Visit(BlockExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override QualifiedTypeName Visit(ConditionalExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override QualifiedTypeName Visit(ConstantExpression node, Void context)
        {
            if (node.Type == typeof(Type))
            {
                //// TODO check for collection
                var singleQualifiedTypeName = this.typeToSingleQualifiedTypeNameVisitor.Dispatch(node.Type);
                return new QualifiedTypeName.SingleValue(singleQualifiedTypeName);
            }
            else
            {
                throw new Exception("TODO is there more to implement here?");
            }
        }

        protected override QualifiedTypeName Visit(DebugInfoExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override QualifiedTypeName Visit(DefaultExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override QualifiedTypeName Visit(DynamicExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override QualifiedTypeName Visit(GotoExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override QualifiedTypeName Visit(IndexExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override QualifiedTypeName Visit(InvocationExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override QualifiedTypeName Visit(LabelExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override QualifiedTypeName Visit(LambdaExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override QualifiedTypeName Visit(ListInitExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override QualifiedTypeName Visit(LoopExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override QualifiedTypeName Visit(MemberExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override QualifiedTypeName Visit(MemberInitExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override QualifiedTypeName Visit(MethodCallExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override QualifiedTypeName Visit(NewArrayExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override QualifiedTypeName Visit(NewExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override QualifiedTypeName Visit(ParameterExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override QualifiedTypeName Visit(RuntimeVariablesExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override QualifiedTypeName Visit(SwitchExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override QualifiedTypeName Visit(TryExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override QualifiedTypeName Visit(TypeBinaryExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override QualifiedTypeName Visit(UnaryExpression node, Void context)
        {
            throw new NotImplementedException();
        }
    }

    public sealed class LinqToCommonExpressionVisitor : ExpressionVisitor<CommonExpression, Void>
    {
        protected override CommonExpression Visit(BinaryExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override CommonExpression Visit(BlockExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override CommonExpression Visit(ConditionalExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override CommonExpression Visit(ConstantExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override CommonExpression Visit(DebugInfoExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override CommonExpression Visit(DefaultExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override CommonExpression Visit(DynamicExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override CommonExpression Visit(GotoExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override CommonExpression Visit(IndexExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override CommonExpression Visit(InvocationExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override CommonExpression Visit(LabelExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override CommonExpression Visit(LambdaExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override CommonExpression Visit(ListInitExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override CommonExpression Visit(LoopExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override CommonExpression Visit(MemberExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override CommonExpression Visit(MemberInitExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override CommonExpression Visit(MethodCallExpression node, Void context)
        {
            //// TODO
            return new CommonExpression.TodoTerminal(new OdataIdentifier("asdf"));
        }

        protected override CommonExpression Visit(NewArrayExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override CommonExpression Visit(NewExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override CommonExpression Visit(ParameterExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override CommonExpression Visit(RuntimeVariablesExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override CommonExpression Visit(SwitchExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override CommonExpression Visit(TryExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override CommonExpression Visit(TypeBinaryExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override CommonExpression Visit(UnaryExpression node, Void context)
        {
            throw new NotImplementedException();
        }
    }

    public sealed class LinqToIsofExpressionVisitor : ExpressionVisitor<IsofExpression, Void>
    {
        private readonly LinqToCommonExpressionVisitor linqToCommonExpressionVisitor;

        private readonly LinqToQualifiedTypeNameVisitor linqToQualifiedTypeNameVisitor;

        public LinqToIsofExpressionVisitor()
        {
            this.linqToCommonExpressionVisitor = new LinqToCommonExpressionVisitor();
            this.linqToQualifiedTypeNameVisitor = new LinqToQualifiedTypeNameVisitor();
        }

        protected override IsofExpression Visit(BinaryExpression node, Void context)
        {
            //// TODO you could probably do something so that people can write typeof(string) == prop.GetType(), but let's just assume the left side is the accessor and the right side is the typeof for now
            /*Expression typeOperand;
            Expression accessorOperand;*/

            CommonExpression commonExpression;
            if (node.Left is MethodCallExpression methodCallExpression &&
                methodCallExpression.Method == CommonMethods.Object.GetType)
            {
                //// TODO you need everything *before* the .gettype call...
                commonExpression = this.linqToCommonExpressionVisitor.Dispatch(node.Left, context);
            }
            else
            {
                throw new Exception("TODO the left side needs to be an accessor");
            }

            QualifiedTypeName qualifiedTypeName;
            if (node.Right is ConstantExpression constantExpression && constantExpression.Type == typeof(Type))
            {
                qualifiedTypeName = this.linqToQualifiedTypeNameVisitor.Dispatch(node.Right, context);
            }
            else
            {
                throw new Exception("TODO the right side needs to be a typeof");
            }

            return new IsofExpression(commonExpression, qualifiedTypeName);
        }

        protected override IsofExpression Visit(BlockExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override IsofExpression Visit(ConditionalExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override IsofExpression Visit(ConstantExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override IsofExpression Visit(DebugInfoExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override IsofExpression Visit(DefaultExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override IsofExpression Visit(DynamicExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override IsofExpression Visit(GotoExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override IsofExpression Visit(IndexExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override IsofExpression Visit(InvocationExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override IsofExpression Visit(LabelExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override IsofExpression Visit(LambdaExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override IsofExpression Visit(ListInitExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override IsofExpression Visit(LoopExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override IsofExpression Visit(MemberExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override IsofExpression Visit(MemberInitExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override IsofExpression Visit(MethodCallExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override IsofExpression Visit(NewArrayExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override IsofExpression Visit(NewExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override IsofExpression Visit(ParameterExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override IsofExpression Visit(RuntimeVariablesExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override IsofExpression Visit(SwitchExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override IsofExpression Visit(TryExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override IsofExpression Visit(TypeBinaryExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override IsofExpression Visit(UnaryExpression node, Void context)
        {
            throw new NotImplementedException();
        }
    }

    public sealed class LinqToBoolCommonExpressionVisitor : ExpressionVisitor<BoolCommonExpression, Void>
    {
        private readonly LinqToIsofExpressionVisitor linqToIsofExpressionVisitor;

        public LinqToBoolCommonExpressionVisitor()
        {
            this.linqToIsofExpressionVisitor = new LinqToIsofExpressionVisitor();
        }

        protected override BoolCommonExpression Visit(BinaryExpression node, Void context)
        {
            if (node.Method == CommonMethods.Type.Equals)
            {
                //// TODO one side must be a type and the other must be something that results in a gettype
                var isofExpression = this.linqToIsofExpressionVisitor.Dispatch(node, context);
                return new BoolCommonExpression.Eighth(isofExpression);
            }
            else
            {
                throw new System.Exception("TODO can't convert");
            }
        }

        protected override BoolCommonExpression Visit(BlockExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override BoolCommonExpression Visit(ConditionalExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override BoolCommonExpression Visit(ConstantExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override BoolCommonExpression Visit(DebugInfoExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override BoolCommonExpression Visit(DefaultExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override BoolCommonExpression Visit(DynamicExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override BoolCommonExpression Visit(GotoExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override BoolCommonExpression Visit(IndexExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override BoolCommonExpression Visit(InvocationExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override BoolCommonExpression Visit(LabelExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override BoolCommonExpression Visit(LambdaExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override BoolCommonExpression Visit(ListInitExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override BoolCommonExpression Visit(LoopExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override BoolCommonExpression Visit(MemberExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override BoolCommonExpression Visit(MemberInitExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override BoolCommonExpression Visit(MethodCallExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override BoolCommonExpression Visit(NewArrayExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override BoolCommonExpression Visit(NewExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override BoolCommonExpression Visit(ParameterExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override BoolCommonExpression Visit(RuntimeVariablesExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override BoolCommonExpression Visit(SwitchExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override BoolCommonExpression Visit(TryExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override BoolCommonExpression Visit(TypeBinaryExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override BoolCommonExpression Visit(UnaryExpression node, Void context)
        {
            throw new NotImplementedException();
        }
    }

    public sealed class LinqToOdataFilterVisitor : ExpressionVisitor<Filter, Void>
    {
        private readonly LinqToBoolCommonExpressionVisitor linqToBoolCommonExpressionVisitor;

        public LinqToOdataFilterVisitor()
            : this(new LinqToBoolCommonExpressionVisitor())
        {
        }

        public LinqToOdataFilterVisitor(
            LinqToBoolCommonExpressionVisitor linqToBoolCommonExpressionVisitor)
        {
            //// TODO can you have some logger or something to track the traversal of the tree?
            //// TODO does constructor injection make sense here? or is context of the use-case too important?
            this.linqToBoolCommonExpressionVisitor = linqToBoolCommonExpressionVisitor;
        }

        protected override Filter Visit(BinaryExpression node, Void context)
        {
            //// TODO does it make sense to treat this as "this method converts a 'binaryexpression' to a 'filter'" and then have another visitor for "this method converts a 'binaryexpression' to a 'boolcommonexpression'" and another visitor for "this method converts a 'binaryexpression' to a 'boolmethodcallexpression'" and so on?
            if (node.Type != typeof(bool))
            {
                //// TODO despite having a lot of inforation, this message doesn't actually comunicate that it's the return type of "bool" that's the problem
                throw new System.Exception($"It is not meaningful to convert the following to an OData 'Filter' expression: a .NET '{node.GetType().Name}' with method '{node.Method?.Name}' and node type '{node.NodeType}' that returns '{node.Type.Name}'");
            }

            var boolCommonExpression = this.linqToBoolCommonExpressionVisitor.Dispatch(node, context);
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
