namespace Fx.OdataPocRoot.V2.System.Net.Http
{
    using global::System;
    using global::System.Net.Http;
    using global::System.Threading.Tasks;

    public interface IHttpClient
    {
        Task<HttpResponseMessage> GetAsync(RelativeUri uri);

        //// TODO FEATURE GAP other http methods here
    }
}
