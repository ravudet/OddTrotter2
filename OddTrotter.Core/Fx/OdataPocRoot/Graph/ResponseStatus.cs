namespace Fx.OdataPocRoot.Graph
{
    public sealed class ResponseStatus
    {
        public ResponseStatus(string response, string time)
        {
            Response = new OdataProperty<string>(response);
            Time = new OdataProperty<string>(time);
        }

        public OdataProperty<string> Response { get; }

        public OdataProperty<string> Time { get; }
    }
}
