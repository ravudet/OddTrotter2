namespace Fx.OdataPocRoot.Graph
{
    public sealed class ResponseStatus
    {
        public ResponseStatus(string response, string time)
        {
            Response = response;
            Time = time;
        }

        public string Response { get; }

        public string Time { get; }
    }
}
