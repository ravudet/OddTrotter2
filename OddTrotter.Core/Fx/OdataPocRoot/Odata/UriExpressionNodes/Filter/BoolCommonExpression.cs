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
                / boolFunctionExpr
                / boolMethodCallExpr
                / boolParenExpr
                / boolCastExpr
                / isofExpr
                / notExpr
                / primitiveLiteral eqExpr
                / primitiveLiteral neExpr
                / primitiveLiteral ltExpr
                / primitiveLiteral leExpr
                / primitiveLiteral gtExpr
                / primitiveLiteral geExpr
                / primitiveLiteral hasExpr
                / primitiveLiteral inExpr
                / TODO
                / boolPrimitiveLiteral andExpr
                / boolPrimitiveLiteral orExpr
                / TODO
                )

boolCastExpr =
                TODO only if you can cast to bool

boolParenExpr =
                OPEN BWS boolCommonExpr BWS CLOSE

boolFunctionExpr = 
                    TODO

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
                        / entityNavigationProperty  boolSingleNavigationExpr
                        / complexColProperty boolComplexColPathExpr
                        / primitiveColProperty boolCollectionPathExpr
                        / primitiveProperty [ boolPrimitivePathExpr ]
                        / streamProperty boolPrimitivePathExpr
                        )

boolPrimitivePathExpr =
                        "/" [ boolAnnotationExpr / boundFunctionExpr ]

boolComplexColPathExpr = 
                        [ "/" qualifiedComplexTypeName ]
                        [ boolCollectionPathExpr ]

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
