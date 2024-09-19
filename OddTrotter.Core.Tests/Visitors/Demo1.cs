namespace Visitors
{
    using System.Text;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public abstract class Parent
    {
        private Parent()
        {
        }

        protected abstract TResult Accept<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);

        public abstract class Visitor<TResult, TContext>
        {
            public TResult Dispatch(Parent node, TContext context)
            {
                return node.Accept(this, context);
            }

            public abstract TResult Visit(Child1 node, TContext context);

            public abstract TResult Visit(Child2 node, TContext context);

            public abstract TResult Visit(Child3 node, TContext context);
        }

        public sealed class Child1 : Parent
        {
            protected sealed override TResult Accept<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Visit(this, context);
            }
        }

        public sealed class Child2 : Parent
        {
            protected sealed override TResult Accept<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Visit(this, context);
            }
        }

        public abstract class Child3 : Parent
        {
            private Child3()
            {
            }

            protected sealed override TResult Accept<TResult, TContext>(Parent.Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Visit(this, context);
            }

            protected abstract TResult Accept<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);

            public new abstract class Visitor<TResult, TContext>
            {
                public TResult Dispatch(Child3 node, TContext context)
                {
                    return node.Accept(this, context);
                }

                public abstract TResult Visit(GrandChild1 node, TContext context);

                public abstract TResult Visit(GrandChild2 node, TContext context);
            }

            public sealed class GrandChild1 : Child3
            {
                protected sealed override TResult Accept<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Visit(this, context);
                }
            }

            public sealed class GrandChild2 : Child3
            {
                protected sealed override TResult Accept<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Visit(this, context);
                }
            }
        }
    }

    public struct Void
    {
    }

    public sealed class ToStringVisitor : Parent.Visitor<Void, StringBuilder>
    {
        private ToStringVisitor()
        {
        }

        public static ToStringVisitor Instance { get; } = new ToStringVisitor();

        public override Void Visit(Parent.Child1 node, StringBuilder context)
        {
            context.Append("child1");
            return default;
        }

        public override Void Visit(Parent.Child2 node, StringBuilder context)
        {
            context.Append("child2");
            return default;
        }

        public override Void Visit(Parent.Child3 node, StringBuilder context)
        {
            return Child3ToStringVisitor.Instance.Dispatch(node, context);
        }

        private sealed class Child3ToStringVisitor : Parent.Child3.Visitor<Void, StringBuilder>
        {
            private Child3ToStringVisitor()
            {
            }

            public static Child3ToStringVisitor Instance { get; } = new Child3ToStringVisitor();

            public override Void Visit(Parent.Child3.GrandChild1 node, StringBuilder context)
            {
                context.Append("grandchild1");
                return default;
            }

            public override Void Visit(Parent.Child3.GrandChild2 node, StringBuilder context)
            {
                context.Append("grandchild2");
                return default;
            }
        }
    }

    [TestClass]
    public sealed class Demo1
    {
        [TestMethod]
        public void Test()
        {
            var toStringVisitor = ToStringVisitor.Instance;

            var builder = new StringBuilder();
            toStringVisitor.Dispatch(new Parent.Child1(), builder);
            toStringVisitor.Dispatch(new Parent.Child2(), builder);
            toStringVisitor.Dispatch(new Parent.Child3.GrandChild1(), builder);
            toStringVisitor.Dispatch(new Parent.Child3.GrandChild2(), builder);

            var value = builder.ToString();
            Assert.AreEqual("child1child2grandchild1grandchild2", value);
        }
    }
}
