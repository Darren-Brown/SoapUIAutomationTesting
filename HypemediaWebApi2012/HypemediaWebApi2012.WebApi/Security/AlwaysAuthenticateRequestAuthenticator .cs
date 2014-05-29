using System.Net.Http;
using HypemediaWebApi2012.IApiServices.Security;

namespace HypemediaWebApi2012.WebApi.Security
{
    public class AlwaysAuthenticateRequestAuthenticator : IRequestAuthenticator<NullAccessData>
    {
        public NullAccessData Verify(HttpRequestMessage request)
        {
            return new NullAccessData();
        }
    }
}
