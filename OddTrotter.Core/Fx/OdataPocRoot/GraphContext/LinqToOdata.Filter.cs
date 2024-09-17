namespace Fx.OdataPocRoot.GraphContext
{
    using Fx.OdataPocRoot.Odata.UriExpressionNodes.Common;
    using Fx.OdataPocRoot.Odata.UriExpressionNodes.Filter;
    using System;
    using System.Linq.Expressions;

    public static partial class LinqToOdata
    {
        public static Filter Filter<TType>(Expression<Func<TType, bool>> predicate)
        {
            return TraverseFilter(predicate.Body);

            ////return new Filter(new BoolCommonExpression.First(new BooleanValue.True()));
        }

        private static Filter TraverseFilter(Expression expression)
        {
            //// TODO do this in its completeness
            if (expression is ConstantExpression constantExpression)
            {
                var traversed = TraverseConstantExpression(constantExpression);
                if (traversed.BooleanValue == null)
                {
                    throw new Exception("tODO");
                }

                return new Filter(new BoolCommonExpression.First(traversed.BooleanValue));
            }
            else if (expression is MemberExpression memberExpression)
            {
                var traversed = TraverseMemberExpression(memberExpression);
                if (traversed.PrimitiveLiteral != null &&
                    traversed.PrimitiveLiteral is PrimitiveLiteral.BooleanValueNode booleanValueNode)
                {
                    return new Filter(new BoolCommonExpression.First(booleanValueNode.BooleanValue));
                }
                else if (traversed.BoolFirstMemberExpression != null)
                {
                    return new Filter(new BoolCommonExpression.Third(traversed.BoolFirstMemberExpression));
                }
                else
                {
                    throw new Exception("TODO a c# member was accessed, but it was not a boolean literal and it was not a closure on a boolean local variable");
                }
            }
            else
            {
                throw new Exception("TODO");
            }
        }

        private static (BooleanValue? BooleanValue, PrimitiveLiteral? PrimitiveLiteral)
            TraverseConstantExpression(ConstantExpression expression)
        {
            //// TODO nullable value in each of these branches?
            if (expression.Type == typeof(bool))
            {
                BooleanValue booleanValue =
                    (bool)expression.Value! ? new BooleanValue.True() : new BooleanValue.False();
                return (booleanValue, new PrimitiveLiteral.BooleanValueNode(booleanValue));
            }
            else if (expression.Type == typeof(string))
            {
                return (null, new PrimitiveLiteral.StringValue((string)expression.Value!));
            }
            else if (expression.Type == typeof(DateTime))
            {
                return (null, new PrimitiveLiteral.DateTimeOffsetValue((DateTime)expression.Value!));
            }
            else
            {
                throw new Exception("TODO implement other constant expression translsations");
            }
        }

        private static (PrimitiveLiteral? PrimitiveLiteral, BoolFirstMemberExpression? BoolFirstMemberExpression) 
            TraverseMemberExpression(MemberExpression expression)
        {
            var parameterAccess = TraverseParameterAccess(expression);
            if (parameterAccess != null)
            {
                return (null, parameterAccess);
            }

            var closure = TraverseClosure(expression);
            return (closure, null);
        }

        private static BoolFirstMemberExpression? TraverseParameterAccess(MemberExpression expression)
        {
            return null;
        }

        private static PrimitiveLiteral? TraverseClosure(MemberExpression expression)
        {
            return null;
        }
    }
}
