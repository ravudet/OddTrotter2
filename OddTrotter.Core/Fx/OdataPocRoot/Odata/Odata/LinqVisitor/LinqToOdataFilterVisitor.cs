////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Fx.OdataPocRoot.Odata.Odata.LinqVisitor
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
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

    public sealed class StringToOdataIdentifierVisitor
    {
        public OdataIdentifier Dispatch(string node, Void context)
        {
            //// TODO validate the string...
            return new OdataIdentifier(node);
        }
    }

    public sealed class StringEnumerableToNamesapceVisitor
    {
        private readonly StringToOdataIdentifierVisitor stringToOdataIdentifierVisitor;

        public StringEnumerableToNamesapceVisitor()
        {
            this.stringToOdataIdentifierVisitor = new StringToOdataIdentifierVisitor();
        }

        public Namespace Dispatch(IEnumerable<string> node, Void context)
        {
            //// TODO i can't decide if lazy evaluation is a good idea here...
            var namespaceParts = new List<OdataIdentifier>();
            foreach (var namespacePart in node)
            {
                namespaceParts.Add(this.stringToOdataIdentifierVisitor.Dispatch(namespacePart, context));
            }

            return new Namespace(namespaceParts);
        }
    }

    public sealed class TypeToQualifiedEnumTypeNameVisitor
    {
        private readonly StringEnumerableToNamesapceVisitor stringEnumerableToNamesapceVisitor;

        private readonly StringToOdataIdentifierVisitor stringToOdataIdentifierVisitor;

        public TypeToQualifiedEnumTypeNameVisitor()
        {
            this.stringEnumerableToNamesapceVisitor = new StringEnumerableToNamesapceVisitor();
            this.stringToOdataIdentifierVisitor = new StringToOdataIdentifierVisitor();
        }

        public QualifiedEnumTypeName Dispatch(Type node, Void context)
        {
            //// TODO do you want to revalidate stuff everywhere? like, "node" should be an enum here...
            if (node.Namespace == null)
            {
                throw new Exception("TODO no .net type can be an enum if it doesn't have a namespace");
            }

            var namespaces = node.Namespace.Split('.');

            var @namespace = this.stringEnumerableToNamesapceVisitor.Dispatch(namespaces, context);
            var enumerationTypeName = this.stringToOdataIdentifierVisitor.Dispatch(node.Name, context);
            return new QualifiedEnumTypeName(@namespace, enumerationTypeName);
        }
    }

    public sealed class TypeToQualifiedEntityTypeNameVisitor
    {
        private readonly StringEnumerableToNamesapceVisitor stringEnumerableToNamesapceVisitor;

        private readonly StringToOdataIdentifierVisitor stringToOdataIdentifierVisitor;

        public TypeToQualifiedEntityTypeNameVisitor()
        {
            this.stringEnumerableToNamesapceVisitor = new StringEnumerableToNamesapceVisitor();
            this.stringToOdataIdentifierVisitor = new StringToOdataIdentifierVisitor();
        }

        public QualifiedEntityTypeName Dispatch(Type node, Void context)
        {
            if (node.Namespace == null)
            {
                throw new Exception("TODO no .net type can be an enum if it doesn't have a namespace");
            }

            var namespaces = node.Namespace.Split('.');

            var @namespace = this.stringEnumerableToNamesapceVisitor.Dispatch(namespaces, context);
            var entityTypeName = this.stringToOdataIdentifierVisitor.Dispatch(node.Name, context);
            return new QualifiedEntityTypeName(@namespace, entityTypeName);
        }
    }

    public sealed class LinqToSingleQualifiedTypeNameVisitor : ExpressionVisitor<SingleQualifiedTypeName, Void>
    {
        private readonly TypeToQualifiedEnumTypeNameVisitor typeToQualifiedEnumTypeNameVisitor;

        private readonly TypeToQualifiedEntityTypeNameVisitor typeToQualifiedEntityTypeNameVisitor;

        public LinqToSingleQualifiedTypeNameVisitor()
        {
            this.typeToQualifiedEnumTypeNameVisitor = new TypeToQualifiedEnumTypeNameVisitor();
            this.typeToQualifiedEntityTypeNameVisitor = new TypeToQualifiedEntityTypeNameVisitor();
        }

        protected override SingleQualifiedTypeName Visit(BinaryExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override SingleQualifiedTypeName Visit(BlockExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override SingleQualifiedTypeName Visit(ConditionalExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override SingleQualifiedTypeName Visit(ConstantExpression node, Void context)
        {
            //// TODO the subvisitors here need to have a way to update the casing and names and such of the types (e.g., the "Calendar" class should be translated to "calendar", and also you need to have a way to configure namespaces if the caller doesn't want their .NET ones exposed)

            if (!(node.Value is Type type))
            {
                throw new Exception("TODO use a good error message here");
            }

            if (type.IsEnum)
            {
                var qualifiedEnumTypeName = this.typeToQualifiedEnumTypeNameVisitor.Dispatch(type, context);
                return new SingleQualifiedTypeName.QualifiedEnumType(qualifiedEnumTypeName);
            }
            /*else if (is typedef)
            {
                //// TODO is there a corresponding .NET construct?
            }*/
            else if (type == typeof(byte[]))
            {
                //// TODO you're breaking your convention here, but there would be too many visitors and too much duplicated if statements to continue following it...judgement call; you might go back on it if you need to return primitivetypenames in other places
                var primitiveTypeName = new PrimitiveTypeName.Binary();
                return new SingleQualifiedTypeName.PrimitiveType(primitiveTypeName);
            }
            else if (type == typeof(bool))
            {
                var primitiveTypeName = new PrimitiveTypeName.Boolean();
                return new SingleQualifiedTypeName.PrimitiveType(primitiveTypeName);
            }
            else if (type == typeof(byte))
            {
                var primitiveTypeName = new PrimitiveTypeName.Byte();
                return new SingleQualifiedTypeName.PrimitiveType(primitiveTypeName);
            }
            else if (type == typeof(DateOnly))
            {
                var primitiveTypeName = new PrimitiveTypeName.Date();
                return new SingleQualifiedTypeName.PrimitiveType(primitiveTypeName);
            }
            else if (type == typeof(DateTimeOffset))
            {
                //// TODO do you also want to include datetime here?
                var primitiveTypeName = new PrimitiveTypeName.DateTimeOffset();
                return new SingleQualifiedTypeName.PrimitiveType(primitiveTypeName);
            }
            else if (type == typeof(decimal))
            {
                var primitiveTypeName = new PrimitiveTypeName.Decimal();
                return new SingleQualifiedTypeName.PrimitiveType(primitiveTypeName);
            }
            else if (type == typeof(double))
            {
                var primitiveTypeName = new PrimitiveTypeName.Double();
                return new SingleQualifiedTypeName.PrimitiveType(primitiveTypeName);
            }
            else if (type == typeof(TimeSpan))
            {
                var primitiveTypeName = new PrimitiveTypeName.Duration();
                return new SingleQualifiedTypeName.PrimitiveType(primitiveTypeName);
            }
            else if (type == typeof(Guid))
            {
                var primitiveTypeName = new PrimitiveTypeName.Guid();
                return new SingleQualifiedTypeName.PrimitiveType(primitiveTypeName);
            }
            else if (type == typeof(short))
            {
                var primitiveTypeName = new PrimitiveTypeName.Int16();
                return new SingleQualifiedTypeName.PrimitiveType(primitiveTypeName);
            }
            else if (type == typeof(int))
            {
                var primitiveTypeName = new PrimitiveTypeName.Int32();
                return new SingleQualifiedTypeName.PrimitiveType(primitiveTypeName);
            }
            else if (type == typeof(long))
            {
                var primitiveTypeName = new PrimitiveTypeName.Int64();
                return new SingleQualifiedTypeName.PrimitiveType(primitiveTypeName);
            }
            else if (type == typeof(sbyte))
            {
                var primitiveTypeName = new PrimitiveTypeName.Sbyte();
                return new SingleQualifiedTypeName.PrimitiveType(primitiveTypeName);
            }
            else if (type == typeof(float))
            {
                var primitiveTypeName = new PrimitiveTypeName.Single();
                return new SingleQualifiedTypeName.PrimitiveType(primitiveTypeName);
            }
            else if (type == typeof(Stream))
            {
                var primitiveTypeName = new PrimitiveTypeName.Stream();
                return new SingleQualifiedTypeName.PrimitiveType(primitiveTypeName);
            }
            else if (type == typeof(TimeOnly))
            {
                var primitiveTypeName = new PrimitiveTypeName.TimeOfDay();
                return new SingleQualifiedTypeName.PrimitiveType(primitiveTypeName);
            }
            else
            {
                //// TODO how to differentiate between entity and complex type; you can't just look for a key property because entity base types are allowed to not define a key
                var qualifiedEntityTypeName = this.typeToQualifiedEntityTypeNameVisitor.Dispatch(type, context);
                return new SingleQualifiedTypeName.QualifiedEntityType(qualifiedEntityTypeName);
            }

            throw new Exception("TODO there's no meaningful mapping from node to an odata type name");
        }

        protected override SingleQualifiedTypeName Visit(DebugInfoExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override SingleQualifiedTypeName Visit(DefaultExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override SingleQualifiedTypeName Visit(DynamicExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override SingleQualifiedTypeName Visit(GotoExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override SingleQualifiedTypeName Visit(IndexExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override SingleQualifiedTypeName Visit(InvocationExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override SingleQualifiedTypeName Visit(LabelExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override SingleQualifiedTypeName Visit(LambdaExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override SingleQualifiedTypeName Visit(ListInitExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override SingleQualifiedTypeName Visit(LoopExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override SingleQualifiedTypeName Visit(MemberExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override SingleQualifiedTypeName Visit(MemberInitExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override SingleQualifiedTypeName Visit(MethodCallExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override SingleQualifiedTypeName Visit(NewArrayExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override SingleQualifiedTypeName Visit(NewExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override SingleQualifiedTypeName Visit(ParameterExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override SingleQualifiedTypeName Visit(RuntimeVariablesExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override SingleQualifiedTypeName Visit(SwitchExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override SingleQualifiedTypeName Visit(TryExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override SingleQualifiedTypeName Visit(TypeBinaryExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override SingleQualifiedTypeName Visit(UnaryExpression node, Void context)
        {
            throw new NotImplementedException();
        }
    }

    public sealed class LinqToQualifiedTypeNameVisitor : ExpressionVisitor<QualifiedTypeName, Void>
    {
        private readonly LinqToSingleQualifiedTypeNameVisitor linqToSingleQualifiedTypeNameVisitor;

        public LinqToQualifiedTypeNameVisitor()
        {
            this.linqToSingleQualifiedTypeNameVisitor = new LinqToSingleQualifiedTypeNameVisitor();
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
                var singleQualifiedTypeName = this.linqToSingleQualifiedTypeNameVisitor.Dispatch(node, context);
                if (node.Type.IsSubclassOf(typeof(IEnumerable)))
                {
                    return new QualifiedTypeName.MultiValue(singleQualifiedTypeName);
                }
                else
                {
                    return new QualifiedTypeName.SingleValue(singleQualifiedTypeName);
                }
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
        private readonly StringToOdataIdentifierVisitor stringToOdataIdentifierVisitor;

        public LinqToCommonExpressionVisitor()
        {
            this.stringToOdataIdentifierVisitor = new StringToOdataIdentifierVisitor();
        }

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
            //// TODO make sure that it's an odataproperty<T>?
            //// TODO reminder that commonexpression is not accurate to the odata standard, so this whole class is basically a hack
            var identifier = this.stringToOdataIdentifierVisitor.Dispatch(node.Member.Name, context);
            if (node.Expression == null)
            {
                throw new Exception("TODO i don't understand waht cases result in a null expression here...");
            }

            var commonExpression = this.Dispatch(node.Expression, context);
            if (commonExpression is CommonExpression.TodoTerminal todoTerminal && todoTerminal.Identifier.Identifier == "$this")
            {
                return new CommonExpression.TodoTerminal(identifier);
            }
            else
            {
                return new CommonExpression.Todo(identifier, commonExpression);
            }
        }

        protected override CommonExpression Visit(MemberInitExpression node, Void context)
        {
            throw new NotImplementedException();
        }

        protected override CommonExpression Visit(MethodCallExpression node, Void context)
        {
            throw new NotImplementedException();
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
            var identifier = new OdataIdentifier("$this");
            return new CommonExpression.TodoTerminal(identifier);
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
            CommonExpression commonExpression;
            if (node.Left is MethodCallExpression methodCallExpression &&
                methodCallExpression.Method == CommonMethods.Object.GetType)
            {
                var accessorExpression = methodCallExpression.Object;
                if (accessorExpression == null)
                {
                    throw new Exception("TODO i don't think you can get here");
                }

                commonExpression = this.linqToCommonExpressionVisitor.Dispatch(accessorExpression, context);
            }
            else
            {
                throw new Exception("TODO the left side needs to be an accessor");
            }

            QualifiedTypeName qualifiedTypeName;
            if (node.Right is ConstantExpression constantExpression && constantExpression.Type == typeof(Type))
            {
                qualifiedTypeName = this.linqToQualifiedTypeNameVisitor.Dispatch(constantExpression, context);
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
