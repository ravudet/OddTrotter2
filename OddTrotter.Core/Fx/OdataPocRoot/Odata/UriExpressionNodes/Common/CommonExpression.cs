////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Fx.OdataPocRoot.Odata.UriExpressionNodes.Common
{
    using System.Collections.Generic;

    public abstract class CommonExpression
    {
        public CommonExpression()
        {
            //// TODO implement
            ////throw new System.Exception("TODO");
        }

        public sealed class Todo : CommonExpression
        {
            public Todo(OdataIdentifier identifier, CommonExpression commonExpression)
            {
                Identifier = identifier;
                CommonExpression = commonExpression;
            }

            public OdataIdentifier Identifier { get; }

            public CommonExpression CommonExpression { get; }
        }

        public sealed class TodoTerminal : CommonExpression
        {
            public TodoTerminal(OdataIdentifier identifier)
            {
                Identifier = identifier;
            }

            public OdataIdentifier Identifier { get; }
        }

        public sealed class Empty : CommonExpression
        {
            public Empty()
            {
            }
        }
    }
}
