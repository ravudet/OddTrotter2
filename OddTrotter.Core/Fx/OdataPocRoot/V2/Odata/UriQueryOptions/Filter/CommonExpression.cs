////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Fx.OdataPocRoot.V2.Odata.UriQueryOptions.Filter
{
    public abstract class CommonExpressionPart1
    {
        //// TODO should these "parts" be nested in CommonExpression?
    }

    public abstract class CommonExpressionPart2
    {
    }

    public abstract class CommonExpressionPart3
    {
    }

    public abstract class CommonExpressionPart4
    {
    }

    public abstract class CommonExpression
    {
        private CommonExpression()
        {
        }

        public sealed class Part1Only : CommonExpression
        {
            public Part1Only(CommonExpressionPart1 part1)
            {
                this.Part1 = part1;
            }

            public CommonExpressionPart1 Part1 { get; }
        }

        public sealed class Part1Part2 : CommonExpression
        {
            public Part1Part2(CommonExpressionPart1 part1, CommonExpressionPart2 part2)
            {
                this.Part1 = part1;
                this.Part2 = part2;
            }

            public CommonExpressionPart1 Part1 { get; }
            public CommonExpressionPart2 Part2 { get; }
        }

        public sealed class Part1Part2Part3 : CommonExpression
        {
            public Part1Part2Part3(CommonExpressionPart1 part1, CommonExpressionPart2 part2, CommonExpressionPart3 part3)
            {
                this.Part1 = part1;
                this.Part2 = part2;
                this.Part3 = part3;
            }

            public CommonExpressionPart1 Part1 { get; }

            public CommonExpressionPart2 Part2 { get; }

            public CommonExpressionPart3 Part3 { get; }
        }

        public sealed class Part1Part2Part3Part4 : CommonExpression
        {
            public Part1Part2Part3Part4(CommonExpressionPart1 part1, CommonExpressionPart2 part2, CommonExpressionPart3 part3, CommonExpressionPart4 part4)
            {
                this.Part1 = part1;
                this.Part2 = part2;
                this.Part3 = part3;
                this.Part4 = part4;
            }

            public CommonExpressionPart1 Part1 { get; }

            public CommonExpressionPart2 Part2 { get; }

            public CommonExpressionPart3 Part3 { get; }

            public CommonExpressionPart4 Part4 { get; }
        }

        public sealed class Part1Part3 : CommonExpression
        {
            public Part1Part3(CommonExpressionPart1 part1, CommonExpressionPart3 part3)
            {
                this.Part1 = part1;
                this.Part3 = part3;
            }

            public CommonExpressionPart1 Part1 { get; }

            public CommonExpressionPart3 Part3 { get; }
        }

        public sealed class Part1Part3Part4 : CommonExpression
        {
            public Part1Part3Part4(CommonExpressionPart1 part1, CommonExpressionPart3 part3, CommonExpressionPart4 part4)
            {
                this.Part1 = part1;
                this.Part3 = part3;
                this.Part4 = part4;
            }

            public CommonExpressionPart1 Part1 { get; }

            public CommonExpressionPart3 Part3 { get; }

            public CommonExpressionPart4 Part4 { get; }
        }

        public sealed class Part1Part4 : CommonExpression
        {
            public Part1Part4(CommonExpressionPart1 part1, CommonExpressionPart4 part4)
            {
                this.Part1 = part1;
                this.Part4 = part4;
            }

            public CommonExpressionPart1 Part1 { get; }

            public CommonExpressionPart4 Part4 { get; }
        }
    }
}
