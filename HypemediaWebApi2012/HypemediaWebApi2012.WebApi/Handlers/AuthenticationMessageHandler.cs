using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using HypemediaWebApi2012.IApiServices.Security;
using HypemediaWebApi2012.WebApi.Security;

namespace HypemediaWebApi2012.WebApi.Handlers
{
    /// <summary>
    /// Reads and verifies authentication header from a message. This is temporary implementation. It will be replaced by Single-Sigh-On / OAuth handler.
    /// </summary>
    class AuthenticationMessageHandler : DelegatingHandler
    {

        readonly IRequestAuthenticator<NullAccessData> _authenticator;
        public AuthenticationMessageHandler(IRequestAuthenticator<NullAccessData> authenticator)
        {
            if (authenticator == null)
                throw new ArgumentNullException("authenticator");
            _authenticator = authenticator;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            return Task<HttpResponseMessage>.Factory.StartNew(() =>
            {
                try
                {
                    // TODO: here we need to call OAuth module that verifies the message header and returns access token and some more information
                    var accessInfo = _authenticator.Verify(request);
                    SetAccessInfo(request, accessInfo);
                    return base.SendAsync(request, cancellationToken).Result;

                }
                catch (AuthenticationFailedException ex)
                {
                    return request.CreateErrorResponse(HttpStatusCode.Unauthorized, ex.Message);
                }
            });
        }

        static void SetAccessInfo(HttpRequestMessage request, NullAccessData accessInfo)
        {
            request.Properties.Add(RequestMessagePropertyKeys.Security.AuthorizationInfoKey, accessInfo);
        }
    }
}
