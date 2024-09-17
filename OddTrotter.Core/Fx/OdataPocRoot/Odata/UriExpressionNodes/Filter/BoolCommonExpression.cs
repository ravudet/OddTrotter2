namespace Fx.OdataPocRoot.Odata.UriExpressionNodes.Filter
{
    using Fx.OdataPocRoot.Odata.UriExpressionNodes.Common;

    public abstract class BoolCommonExpression
    {
        private BoolCommonExpression()
        {
        }

        public sealed class First : BoolCommonExpression
        {
            public First(BooleanValue booleanValue)
            {
                BooleanValue = booleanValue;
            }

            public BooleanValue BooleanValue { get; }
        }

        //// TODO do other derived types after you've created abnf for boolcommonexpr
        /*




boolCommonExpr =
                (
                boolPrimitiveLiteral
                / boolRootExpr
                / boolFirstMemberExpr
                / ????
                )

boolPrimitiveLiteral =
                        booleanValue

boolRootExpr =
                TODO

boolFirstMemberExpr =
                        boolMemberExpr
                        / boolInscopeVariableExpr [ "/" boolMemberExpr ]

boolMemberExpr = 
                [ qualifiedEntityTypeName "/" ]
                (
                boolPropertyPathExpr
                / boundFunctionExpr
                / boolAnnotationExpr
                )

boolPropertyPathExpr = 
                        (
                        entityColNavigationProperty boolCollectionNavigationExpr
                        / ????
                        )

boolCollectionNavigationExpr = 
                                [ "/" qualifiedEntityTypeName ]
                                [ keyPredicate boolSingleNavigationExpr
                                / boolCollectionPathExpr
                                ]

boolSingleNavigationExpr = 
                            "/" boolMemberExpr

boolCollectionPathExpr = 
                        TODO

boolAnnotationExpr =
                    TODO

boolInscopeVariableExpr =
                            implicitVariableExpr
                            / parameterAlias
        */
    }
}
