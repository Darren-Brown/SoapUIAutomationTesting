using System.Net.Http;
using HypemediaWebApi2012.IApiServices.Security;

namespace HypemediaWebApi2012.WebApi.Security
{
    interface IRequestAuthenticator<out TAccessData>
    {
        TAccessData Verify(HttpRequestMessage request);
    }
}