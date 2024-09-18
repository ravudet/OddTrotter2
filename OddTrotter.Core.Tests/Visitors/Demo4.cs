namespace Visitors.HomogeneousLinkedListNode
{
    using System;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public sealed class SelectVisitor<TElement, TResult> : HomogeneousLinkedListNode<TElement>.Visitor<HomogeneousLinkedListNode<TResult>, Void>
    {
        private readonly Func<TElement, TResult> selector;

        public SelectVisitor(Func<TElement, TResult> selector)
        {
            this.selector = selector;
        }

        public override HomogeneousLinkedListNode<TResult> Visit(HomogeneousLinkedListNode<TElement>.Terminal node, Void context)
        {
            return HomogeneousLinkedListNode<TResult>.Terminal.Instance;
        }

        public override HomogeneousLinkedListNode<TResult> Visit<TTheRest>(HomogeneousLinkedListNode<TElement>.Todo<TTheRest> node, Void context)
        {
            return new HomogeneousLinkedListNode<TResult>.Todo<>
        }
    }

    [TestClass]
    public sealed class Demo4
    {
    }
}
