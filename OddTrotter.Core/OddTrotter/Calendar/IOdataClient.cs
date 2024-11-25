////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace OddTrotter.Calendar
{
    public interface IOdataClient
    {
        Task<HttpResponseMessage> GetAsync(RelativeUri relativeUri);

        Task<HttpResponseMessage> GetAsync(AbsoluteUri absoluteUri);
    }
}
