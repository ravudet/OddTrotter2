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
            else
            {
                throw new Exception("TODO");
            }
        }

        private static (BooleanValue? BooleanValue, PrimitiveLiteral? PrimitiveLiteral) TraverseConstantExpression(
            ConstantExpression expression)
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
    }
}
