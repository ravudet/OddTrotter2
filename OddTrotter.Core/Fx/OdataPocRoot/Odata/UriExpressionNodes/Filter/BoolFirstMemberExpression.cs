////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Fx.OdataPocRoot.Odata.UriExpressionNodes.Filter
{
    public abstract class BoolFirstMemberExpression
    {
        private BoolFirstMemberExpression()
        {
        }

        public sealed class BoolMemberExpressionNode : BoolFirstMemberExpression
        {
            public BoolMemberExpressionNode(BoolMemberExpression boolMemberExpression)
            {
                BoolMemberExpression = boolMemberExpression;
            }

            public BoolMemberExpression BoolMemberExpression { get; }
        }

        public abstract class InscopeVariable : BoolFirstMemberExpression
        {
            private InscopeVariable(BoolInscopeVariableExpression boolInscopeVariableExpression)
            {
                this.BoolInscopeVariableExpression = boolInscopeVariableExpression;
            }

            public BoolInscopeVariableExpression BoolInscopeVariableExpression { get; }

            public sealed class Variable : InscopeVariable
            {
                public Variable(BoolInscopeVariableExpression boolInscopeVariableExpression)
                    : base(boolInscopeVariableExpression)
                {
                }
            }

            public sealed class Member : InscopeVariable
            {
                public Member(
                    BoolInscopeVariableExpression boolInscopeVariableExpression, 
                    BoolMemberExpression boolMemberExpression)
                    : base(boolInscopeVariableExpression)
                {
                    BoolMemberExpression = boolMemberExpression;
                }

                public BoolMemberExpression BoolMemberExpression { get; }
            }
        }
    }
}
