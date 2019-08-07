using Gateway.Models.Requests;
using Gateway.Models.Responses;
using Gateway.Models.RouteInfo;
using System.Threading.Tasks;

namespace Gateway.Http.PublicInterfaces
{
    public interface IHttpClient
    {
        Task<ExtractedResponse> SendRequest(ExtractedRequest request, Route route);
    }
}
