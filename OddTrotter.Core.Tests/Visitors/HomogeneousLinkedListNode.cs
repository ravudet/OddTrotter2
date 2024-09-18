namespace Visitors.HomogeneousLinkedListNode
{    
    using System;
    using System.Text;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public abstract class HomogeneousLinkedListNode<TElement>
    {
        private HomogeneousLinkedListNode()
        {
        }

        protected abstract TResult Accept<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);

        public abstract class Visitor<TResult, TContext>
        {
            public TResult Traverse(HomogeneousLinkedListNode<TElement> node, TContext context)
            {
                return node.Accept(this, context);
            }

            public abstract TResult Visit(Terminal node, TContext context);

            public abstract TResult Visit<TTheRest>(Todo<TTheRest> node, TContext context) where TTheRest : HomogeneousLinkedListNode<TElement>;
        }

        public sealed class Terminal : HomogeneousLinkedListNode<TElement>
        {
            private Terminal()
            {
            }

            public static Terminal Instance { get; } = new Terminal();

            protected sealed override TResult Accept<TResult, TContext>(
                Visitor<TResult, TContext> visitor, 
                TContext context)
            {
                return visitor.Visit(this, context);
            }
        }

        public sealed class Todo<TTheRest> : HomogeneousLinkedListNode<TElement> where TTheRest : HomogeneousLinkedListNode<TElement>
        {
            public Todo(TElement element, TTheRest theRest)
            {
                Element = element;
                TheRest = theRest;
            }

            public TElement Element { get; }

            public TTheRest TheRest { get; }

            protected sealed override TResult Accept<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Visit(this, context);
            }
        }
    }

    public static class HomogeneousLinkedListNodeExtensions
    {
        public static HomogeneousLinkedListNode<TElement>.Todo<TTheRest> Prepend<TElement, TTheRest>(this TTheRest node, TElement element) where TTheRest : HomogeneousLinkedListNode<TElement>
        {
            return new HomogeneousLinkedListNode<TElement>.Todo<TTheRest>(element, node);
        }
    }

    public struct Void
    {
    }

    public sealed class ToStringVisitor<TElement> : HomogeneousLinkedListNode<TElement>.Visitor<Void, (StringBuilder Builder, bool First)>
    {
        private readonly Func<TElement, string> transcriber;

        public ToStringVisitor(Func<TElement, string> transcriber)
        {
            this.transcriber = transcriber;
        }

        public override Void Visit(HomogeneousLinkedListNode<TElement>.Terminal node, (StringBuilder Builder, bool First) context)
        {
            context.Builder.Append("]");
            return default;
        }

        public override Void Visit<TTheRest>(HomogeneousLinkedListNode<TElement>.Todo<TTheRest> node, (StringBuilder Builder, bool First) context)
        {
            if (context.First)
            {
                context.Builder.Append("[");
            }
            else
            {
                context.Builder.Append(", ");
            }

            context.Builder.Append(this.transcriber(node.Element));

            this.Traverse(node.TheRest, (context.Builder, false));
            return default;
        }
    }

    [TestClass]
    public sealed class Demo3
    {
        [TestMethod]
        public void Test()
        {
            var list = HomogeneousLinkedListNode<string>.Terminal.Instance
                .Prepend("fourth")
                .Prepend("fifth")
                .Prepend("sixth");

            var toStringVisitor = new ToStringVisitor<string>(_ => _);
            var builder = new StringBuilder();
            toStringVisitor.Traverse(list, (builder, true));

            Assert.AreEqual("[sixth, fifth, fourth]", builder.ToString());
        }
    }
}
