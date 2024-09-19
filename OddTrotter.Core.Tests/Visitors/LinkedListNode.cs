namespace Visitors.LinkedList
{    
    using System;
    using System.Text;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public abstract class LinkedListNode<TElement>
    {
        private LinkedListNode()
        {
        }

        protected abstract TResult Accept<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);

        public abstract class Visitor<TResult, TContext>
        {
            public TResult Dispatch(LinkedListNode<TElement> node, TContext context)
            {
                return node.Accept(this, context);
            }

            public abstract TResult Visit(Terminal node, TContext context);

            public abstract TResult Visit(Todo node, TContext context);
        }

        public sealed class Terminal : LinkedListNode<TElement>
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

        public sealed class Todo : LinkedListNode<TElement>
        {
            public Todo(TElement element, LinkedListNode<TElement> theRest)
            {
                Element = element;
                TheRest = theRest;
            }

            public TElement Element { get; }

            public LinkedListNode<TElement> TheRest { get; }

            protected sealed override TResult Accept<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Visit(this, context);
            }
        }
    }

    public static class LinkedListNodeExtensions
    {
        public static LinkedListNode<TElement> Prepend<TElement>(this LinkedListNode<TElement> node, TElement element)
        {
            return new LinkedListNode<TElement>.Todo(element, node);
        }
    }

    public struct Void
    {
    }

    public sealed class ToStringVisitor<TElement> : LinkedListNode<TElement>.Visitor<Void, (StringBuilder Builder, bool First)>
    {
        private readonly Func<TElement, string> transcriber;

        public ToStringVisitor(Func<TElement, string> transcriber)
        {
            this.transcriber = transcriber;
        }

        public override Void Visit(LinkedListNode<TElement>.Terminal node, (StringBuilder Builder, bool First) context)
        {
            context.Builder.Append("]");
            return default;
        }

        public override Void Visit(LinkedListNode<TElement>.Todo node, (StringBuilder Builder, bool First) context)
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

            this.Dispatch(node.TheRest, (context.Builder, false));
            return default;
        }
    }

    [TestClass]
    public sealed class Demo2
    {
        [TestMethod]
        public void Test()
        {
            var list = LinkedListNode<string>.Terminal.Instance
                .Prepend("first")
                .Prepend("second")
                .Prepend("third");

            var toStringVisitor = new ToStringVisitor<string>(_ => _);
            var builder = new StringBuilder();
            toStringVisitor.Dispatch(list, (builder, true));

            Assert.AreEqual("[third, second, first]", builder.ToString());
        }
    }
}
