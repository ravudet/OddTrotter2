////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Fx.OdataPocRoot.Odata.Odata
{
    using System;
    using System.Collections.Generic;

    public abstract class OdataResponse<T>
    {
        private OdataResponse()
        {
        }

        public sealed class GetInstance
        {
            public GetInstance(T value, InstanceControlInformation controlInformation)
            {
                Value = value;
                ControlInformation = controlInformation;
            }

            public T Value { get; }

            public InstanceControlInformation ControlInformation { get; }

            public sealed class InstanceControlInformation
            {
                //// TODO put this somewhere commmn
                //// TODO control information here
            }
        }

        public sealed class GetCollection
        {
            public GetCollection(IReadOnlyList<T> value, CollectionControlInformation controlInformation)
            {
                Value = value;
                ControlInformation = controlInformation;
            }

            public IReadOnlyList<T> Value { get; }

            public CollectionControlInformation ControlInformation { get; }

            public sealed class CollectionControlInformation
            {
                //// TODO put this somewhere commmn
                
                public CollectionControlInformation(AbsoluteUri? nextLink, int? count)
                {
                    NextLink = nextLink;
                    Count = count;
                }

                public AbsoluteUri? NextLink { get; } //// TODO indicate if provided or not using something other than nullable?

                public int? Count { get; } //// TODO indicate if provided or not

                //// TODO other control information here
            }
        }
    }
}
