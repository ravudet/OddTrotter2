////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System.Threading.Tasks;

namespace OddTrotter.Calendar
{
    public interface IOdataUnstructuredContext
    {
        Task Get(object requst); //// TODO structure the request as an AST

        Task Post(object request);
        
         //// TODO other verbs
    }
}
