////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Fx.OdataPocRoot.V2.Odata.UriQueryOptions.Filter.Visitors
{
    using System;
    using global::System.Text;

    using Fx.OdataPocRoot.V2.Odata.UriQueryOptions.Filter;

    public sealed class CommonExpressionToStringVisitor : CommonExpression.Visitor<Void, StringBuilder>
    {
        private readonly CommonExpressionPart1ToStringVisitor commonExpressionPart1ToStringVisitor;
        private readonly CommonExpressionPart2ToStringVisitor commonExpressionPart2ToStringVisitor;
        private readonly CommonExpressionPart3ToStringVisitor commonExpressionPart3ToStringVisitor;
        private readonly CommonExpressionPart4ToStringVisitor commonExpressionPart4ToStringVisitor;

        public CommonExpressionToStringVisitor(
            CommonExpressionPart1ToStringVisitor commonExpressionPart1ToStringVisitor,
            CommonExpressionPart2ToStringVisitor commonExpressionPart2ToStringVisitor,
            CommonExpressionPart3ToStringVisitor commonExpressionPart3ToStringVisitor,
            CommonExpressionPart4ToStringVisitor commonExpressionPart4ToStringVisitor)
        {
            //// TODO this is a concrete type; is there really value in constructor injection here? and can you really take in a generic visitor? can you trust that the type signature of the generic actually "means" what this visitor expects it to? this question applies to all of the visitors involved in this "tostring" chain
            this.commonExpressionPart1ToStringVisitor = commonExpressionPart1ToStringVisitor;
            this.commonExpressionPart2ToStringVisitor = commonExpressionPart2ToStringVisitor;
            this.commonExpressionPart3ToStringVisitor = commonExpressionPart3ToStringVisitor;
            this.commonExpressionPart4ToStringVisitor = commonExpressionPart4ToStringVisitor;
        }

        //// TODO implement this

        public sealed override Void Visit(CommonExpression.Part1Only node, StringBuilder context)
        {
            commonExpressionPart1ToStringVisitor.Traverse(
                node.Part1,
                context);

            return default;
        }

        public sealed override Void Visit(CommonExpression.Part1Part2 node, StringBuilder context)
        {
            commonExpressionPart1ToStringVisitor.Traverse(
                node.Part1,
                context);
            commonExpressionPart2ToStringVisitor.Traverse(
                node.Part2,
                context);

            return default;
        }

        public sealed override Void Visit(CommonExpression.Part1Part2Part3 node, StringBuilder context)
        {
            commonExpressionPart1ToStringVisitor.Traverse(
                node.Part1,
                context);
            commonExpressionPart2ToStringVisitor.Traverse(
                node.Part2,
                context);
            commonExpressionPart3ToStringVisitor.Traverse(
                node.Part3,
                context);

            return default;
        }

        public sealed override Void Visit(CommonExpression.Part1Part2Part3Part4 node, StringBuilder context)
        {
            commonExpressionPart1ToStringVisitor.Traverse(
                node.Part1,
                context);
            commonExpressionPart2ToStringVisitor.Traverse(
                node.Part2,
                context);
            commonExpressionPart3ToStringVisitor.Traverse(
                node.Part3,
                context);
            commonExpressionPart4ToStringVisitor.Traverse(
                node.Part4,
                context);

            return default;
        }

        public sealed override Void Visit(CommonExpression.Part1Part2Part4 node, StringBuilder context)
        {
            commonExpressionPart1ToStringVisitor.Traverse(
                node.Part1,
                context);
            commonExpressionPart2ToStringVisitor.Traverse(
                node.Part2,
                context);
            commonExpressionPart4ToStringVisitor.Traverse(
                node.Part4,
                context);

            return default;
        }

        public sealed override Void Visit(CommonExpression.Part1Part3 node, StringBuilder context)
        {
            commonExpressionPart1ToStringVisitor.Traverse(
                node.Part1,
                context);
            commonExpressionPart3ToStringVisitor.Traverse(
                node.Part3,
                context);

            return default;
        }

        public sealed override Void Visit(CommonExpression.Part1Part3Part4 node, StringBuilder context)
        {
            commonExpressionPart1ToStringVisitor.Traverse(
                node.Part1,
                context);
            commonExpressionPart3ToStringVisitor.Traverse(
                node.Part3,
                context);
            commonExpressionPart4ToStringVisitor.Traverse(
                node.Part4,
                context);

            return default;
        }

        public sealed override Void Visit(CommonExpression.Part1Part4 node, StringBuilder context)
        {
            commonExpressionPart1ToStringVisitor.Traverse(
                node.Part1,
                context);
            commonExpressionPart4ToStringVisitor.Traverse(
                node.Part4,
                context);

            return default;
        }
    }
}
