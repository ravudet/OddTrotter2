////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Fx.OdataPocRoot.Odata.Odata.LinqVisitor
{
    using System.Linq.Expressions;

    public struct Void
    {
    }

    public static class ExpressionVisitorExtensions
    {
        public static TResult Dispatch<TResult>(this ExpressionVisitor<TResult, Void> visitor, Expression node)
        {
            return visitor.Dispatch(node, default);
        }

        public static void Dispatch<TContext>(this ExpressionVisitor<Void, TContext> visitor, Expression node, TContext context)
        {
            visitor.Dispatch(node, context);
        }
    }

    /// <summary>
    /// TODO this class has nothing to do with odata or oddtrotter
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <typeparam name="TContext"></typeparam>
    public abstract class ExpressionVisitor<TResult, TContext>
    {
        public TResult Dispatch(Expression node, TContext context)
        {
            var visited = new NestedVisitor(this, context).Visit(node);
            if (visited is VisitedExpression visitedExpression)
            {
                return visitedExpression.Value;
            }
            else
            {
                throw new System.Exception("TODO getting here indicates a fundamental bug");
            }
        }

        protected abstract TResult Visit(BinaryExpression node, TContext context);

        protected abstract TResult Visit(BlockExpression node, TContext context);

        protected abstract TResult Visit(ConditionalExpression node, TContext context);

        protected abstract TResult Visit(ConstantExpression node, TContext context);

        protected abstract TResult Visit(DebugInfoExpression node, TContext context);

        protected abstract TResult Visit(DefaultExpression node, TContext context);

        protected abstract TResult Visit(DynamicExpression node, TContext context);

        protected abstract TResult Visit(GotoExpression node, TContext context);

        protected abstract TResult Visit(IndexExpression node, TContext context);

        protected abstract TResult Visit(InvocationExpression node, TContext context);

        protected abstract TResult Visit(LabelExpression node, TContext context);

        protected abstract TResult Visit(LambdaExpression node, TContext context);

        protected abstract TResult Visit(ListInitExpression node, TContext context);

        protected abstract TResult Visit(LoopExpression node, TContext context);

        protected abstract TResult Visit(MemberExpression node, TContext context);

        protected abstract TResult Visit(MemberInitExpression node, TContext context);

        protected abstract TResult Visit(MethodCallExpression node, TContext context);

        protected abstract TResult Visit(NewArrayExpression node, TContext context);

        protected abstract TResult Visit(NewExpression node, TContext context);

        protected abstract TResult Visit(ParameterExpression node, TContext context);

        protected abstract TResult Visit(RuntimeVariablesExpression node, TContext context);

        protected abstract TResult Visit(SwitchExpression node, TContext context);

        protected abstract TResult Visit(TryExpression node, TContext context);

        protected abstract TResult Visit(TypeBinaryExpression node, TContext context);

        protected abstract TResult Visit(UnaryExpression node, TContext context);

        private sealed class VisitedExpression : Expression
        {
            public VisitedExpression(TResult value)
            {
                this.Value = value;
            }

            public TResult Value { get; set; }
        }

        private sealed class NestedVisitor : ExpressionVisitor
        {
            private readonly ExpressionVisitor<TResult, TContext> expressionVisitor;

            private readonly TContext context;

            public NestedVisitor(ExpressionVisitor<TResult, TContext> expressionVisitor, TContext context)
            {
                this.expressionVisitor = expressionVisitor;
                this.context = context;
            }

            protected override Expression VisitBinary(BinaryExpression node)
            {
                return new VisitedExpression(this.expressionVisitor.Visit(node, this.context));
            }

            protected override Expression VisitBlock(BlockExpression node)
            {
                return new VisitedExpression(this.expressionVisitor.Visit(node, this.context));
            }

            protected override Expression VisitConditional(ConditionalExpression node)
            {
                return new VisitedExpression(this.expressionVisitor.Visit(node, this.context));
            }

            protected override Expression VisitConstant(ConstantExpression node)
            {
                return new VisitedExpression(this.expressionVisitor.Visit(node, this.context));
            }

            protected override Expression VisitDebugInfo(DebugInfoExpression node)
            {
                return new VisitedExpression(this.expressionVisitor.Visit(node, this.context));
            }

            protected override Expression VisitDefault(DefaultExpression node)
            {
                return new VisitedExpression(this.expressionVisitor.Visit(node, this.context));
            }

            protected override Expression VisitDynamic(DynamicExpression node)
            {
                return new VisitedExpression(this.expressionVisitor.Visit(node, this.context));
            }

            protected override Expression VisitGoto(GotoExpression node)
            {
                return new VisitedExpression(this.expressionVisitor.Visit(node, this.context));
            }

            protected override Expression VisitIndex(IndexExpression node)
            {
                return new VisitedExpression(this.expressionVisitor.Visit(node, this.context));
            }

            protected override Expression VisitInvocation(InvocationExpression node)
            {
                return new VisitedExpression(this.expressionVisitor.Visit(node, this.context));
            }

            protected override Expression VisitLabel(LabelExpression node)
            {
                return new VisitedExpression(this.expressionVisitor.Visit(node, this.context));
            }

            protected override Expression VisitLambda<T>(Expression<T> node)
            {
                return new VisitedExpression(this.expressionVisitor.Visit(node, this.context));
            }

            protected override Expression VisitListInit(ListInitExpression node)
            {
                return new VisitedExpression(this.expressionVisitor.Visit(node, this.context));
            }

            protected override Expression VisitLoop(LoopExpression node)
            {
                return new VisitedExpression(this.expressionVisitor.Visit(node, this.context));
            }

            protected override Expression VisitMember(MemberExpression node)
            {
                return new VisitedExpression(this.expressionVisitor.Visit(node, this.context));
            }

            protected override Expression VisitMemberInit(MemberInitExpression node)
            {
                return new VisitedExpression(this.expressionVisitor.Visit(node, this.context));
            }

            protected override Expression VisitMethodCall(MethodCallExpression node)
            {
                return new VisitedExpression(this.expressionVisitor.Visit(node, this.context));
            }

            protected override Expression VisitNewArray(NewArrayExpression node)
            {
                return new VisitedExpression(this.expressionVisitor.Visit(node, this.context));
            }

            protected override Expression VisitNew(NewExpression node)
            {
                return new VisitedExpression(this.expressionVisitor.Visit(node, this.context));
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                return new VisitedExpression(this.expressionVisitor.Visit(node, this.context));
            }

            protected override Expression VisitRuntimeVariables(RuntimeVariablesExpression node)
            {
                return new VisitedExpression(this.expressionVisitor.Visit(node, this.context));
            }

            protected override Expression VisitSwitch(SwitchExpression node)
            {
                return new VisitedExpression(this.expressionVisitor.Visit(node, this.context));
            }

            protected override Expression VisitTry(TryExpression node)
            {
                return new VisitedExpression(this.expressionVisitor.Visit(node, this.context));
            }

            protected override Expression VisitTypeBinary(TypeBinaryExpression node)
            {
                return new VisitedExpression(this.expressionVisitor.Visit(node, this.context));
            }

            protected override Expression VisitUnary(UnaryExpression node)
            {
                return new VisitedExpression(this.expressionVisitor.Visit(node, this.context));
            }
        }
    }
}
