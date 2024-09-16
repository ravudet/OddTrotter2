namespace Fx.OdataPocRoot.Graph
{
    using Fx.OdataPocRoot.Odata;

    public sealed class ResponseStatus
    {
        public ResponseStatus(OdataProperty<string> response, OdataProperty<string> time)
        {
            Response = response;
            Time = time;
        }

        [PropertyName("response")]
        public OdataProperty<string> Response { get; }

        [PropertyName("time")]
        public OdataProperty<string> Time { get; }
    }
}
