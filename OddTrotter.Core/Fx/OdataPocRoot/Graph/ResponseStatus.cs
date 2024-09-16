namespace Fx.OdataPocRoot.Graph
{
    using Fx.OdataPocRoot.Odata;

    public sealed class ResponseStatus
    {
        public ResponseStatus(OdataInstanceProperty<string> response, OdataInstanceProperty<string> time)
        {
            Response = response;
            Time = time;
        }

        [PropertyName("response")]
        public OdataInstanceProperty<string> Response { get; }

        [PropertyName("time")]
        public OdataInstanceProperty<string> Time { get; }
    }
}
