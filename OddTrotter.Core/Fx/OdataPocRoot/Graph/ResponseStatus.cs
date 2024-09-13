namespace Fx.OdataPocRoot.Graph
{
    public sealed class ResponseStatus
    {
        public ResponseStatus(OdataProperty<string> response, OdataProperty<string> time)
        {
            Response = response;
            Time = time;
        }

        public OdataProperty<string> Response { get; }

        public OdataProperty<string> Time { get; }
    }
}
